#!/usr/bin/python3
# File name   : piDcMotor.py
# Description : encapsulates a DC motor connected via Raspberry Pi's GPIO 

import RPi.GPIO as GPIO

from iotServerLib import piIotNode

class PiDcMotor(piIotNode.PiIotNode):
    """ encapsulates a DC motor connected via Raspberry Pi's GPIO """
    def __init__(self, name, parent, enable, in1, in2):
        """ construct a PiIotNode
        name: the name of the node
        parent: parent IotNode object. None for root node.
        enable: pin number for enable
        in1: pin number for in1
        in2: pin number for in2
        """
        super(PiDcMotor, self).__init__(name, parent)
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
        self.stop()

    def stop(self):
        """ stop the motor """
        GPIO.output(self.in1, GPIO.LOW)
        GPIO.output(self.in2, GPIO.LOW)
        GPIO.output(self.enable, GPIO.LOW)
        self.speed = 0
        return self.speed

    def run(self, speed):
        """ run the motor with specified speed 
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
        return self.speed



