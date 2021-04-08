#!/usr/bin/python3
# File name   : piDcMotor.py
# Description : encapsulates a DC motor connected via Raspberry Pi's GPIO 

from time import sleep
from piServices.piUtils import timePrint, startThread
import RPi.GPIO as GPIO

from iotServerLib import piIotNode

# motor states:
#  stop - the motor is stop
#  starting - the motor is starting and ramping up to requested speed
#  moving - the motor is moving at the requested speed
#  stopping - the motor is ramping down to stop
MotorStop = 0
MotorStarting = 1
MotorMoving = 2
MotorStopping = 3

# constants
MaxSpeed = 100
MinSpeed = 5                # the minimum speed that consider to be stop (in absolute value)

class PiDcMotor(piIotNode.PiIotNode):
    """ controller for a DC motor connected via Raspberry Pi's GPIO
    The motor speed control is under a separate thread that start the motor with higher speed (higher torque).
    Once started, it would reduce the speed gradually to the requested speed.
    There are 4 states: MotorStop, MotorStarting, MotorMoving, MotorStopping
    """
    def __init__(self, name, parent, enable, in1, in2, threadCycle=0.05, deltaSpeedPerCycle=5):
        """ construct a PiIotNode
        name: the name of the node
        parent: parent IotNode object. None for root node.
        enable: pin number for enable
        in1: pin number for in1
        in2: pin number for in2
        threadCycle: the delay time between the thread processing
        deltaSpeedPerCycle: increase/decrease the speed per cycle when starting motor
        """
        super(PiDcMotor, self).__init__(name, parent)
        self.threadCycleInSecond = threadCycle
        self.deltaSpeedPerCycle = deltaSpeedPerCycle
        self.initialize(enable, in1, in2)

    def initialize(self, enable, in1, in2):
        """ initialize the DC motor connected via controller similar to L293D
        input arguments:
        enable: pin number for enable
        in1: pin number for in1
        in2: pin number for in2
        """
        self.enable = enable
        self.in1 = in1
        self.in2 = in2
        GPIO.setwarnings(False)
        GPIO.setmode(GPIO.BOARD)
        GPIO.setup(enable, GPIO.OUT)
        GPIO.setup(in1, GPIO.OUT)
        GPIO.setup(in2, GPIO.OUT)
        try:
            self.pwm = GPIO.PWM(enable, 1000)
        except:
            pass
        # start thread for fine control of motor speed
        self._requestedSpeed = 0
        self._extraSpeed = 0
        self._extraSteeringSpeed = 0
        self.motorThread=startThread('Motor Control', target=self._motorControl) 

    def stop(self):
        """ stop the motor """
        self._requestedSpeed = 0
        return self.speed

    def run(self, speed):
        """ run the motor with specified speed 
        speed > 0 run forward max 100
        speed < 0 run reverse max -100
        speed = 0 stop
        return the running speed
        """
        self._requestedSpeed = speed
        return self.speed

    def extraSteeringSpeed(self, deltaSpeed):
        """ add extra torque speed for steering in addition to the run speed by run(speed) """
        self._extraSteeringSpeed = deltaSpeed

    def extraSpeed(self, deltaSpeed):
        """ request extra speed in addition to the run speed by run(speed) """
        self._extraSpeed = deltaSpeed

    def _stop(self):
        """ internal method to stop the motor """
        GPIO.output(self.in1, GPIO.LOW)
        GPIO.output(self.in2, GPIO.LOW)
        GPIO.output(self.enable, GPIO.LOW)
        self.speed = 0
        self._motorState = MotorStop
        timePrint('Stop motor %s' %self.name)
        return self.speed

    def _run(self, speed):
        """ internal method to run the motor with specified speed 
        speed > 0 run forward max 100
        speed < 0 run reverse max -100
        speed = 0 stop
        return the running speed
        """
        if speed == 0: # stop
            self.stop()
        elif speed > 0: # forward
            GPIO.output(self.in1, GPIO.HIGH)
            GPIO.output(self.in2, GPIO.LOW)
            self.pwm.start(100)
            self.pwm.ChangeDutyCycle(speed)
        else: # reverse
            GPIO.output(self.in1, GPIO.LOW)
            GPIO.output(self.in2, GPIO.HIGH)
            self.pwm.start(0)
            self.pwm.ChangeDutyCycle(abs(speed))
        self.speed = speed
        timePrint('Run motor %s at speed %i' %(self.name, self.speed))
        return self.speed

    def _motorControl(self):
        """ control motor speed must be run in separate thread """
        self._stop()
        while True:
            absRequestedSpeed = abs(self._requestedSpeed)
            absRunSpeed = abs(self._requestedSpeed) + self._extraSpeed + self._extraSteeringSpeed
            if absRequestedSpeed < MinSpeed or absRunSpeed < MinSpeed:
                if self._motorState != MotorStop:
                    self._stop()
            else:
                if self._motorState == MotorStop:
                    # starting the motor
                    if self._requestedSpeed > 0:
                        self._run(MaxSpeed)
                    else:
                        self._run(-MaxSpeed)
                    self._motorState = MotorStarting
                    timePrint('Motor %s State: %i extra: %i' %(self.name, self._motorState, self._extraSpeed))
                elif self._motorState == MotorStarting:
                    # send new speed for the motor
                    self._motorStarting(absRunSpeed)
                elif self._motorState == MotorMoving:
                    # motor is already running so just check the speed
                    if absRunSpeed != abs(self.speed):
                        self._motorState = MotorStarting
                        self._motorStarting(absRunSpeed)
                        timePrint('Motor %s State: %i extra: %i' %(self.name, self._motorState, self._extraSpeed))

            sleep(self.threadCycleInSecond)

    def _motorStarting(self, absRunSpeed):
        """ this is for the state MotorStarting to calculate new speed for the motor """
        absNewSpeed = abs(self.speed) - self.deltaSpeedPerCycle
        if absNewSpeed <= absRunSpeed:
            absNewSpeed = absRunSpeed
            self._motorState = MotorMoving
            timePrint('Motor %s State: %i extra: %i' %(self.name, self._motorState, self._extraSpeed))
        if self._requestedSpeed > 0:
            self._run(absNewSpeed)
        else:
            self._run(-absNewSpeed)

