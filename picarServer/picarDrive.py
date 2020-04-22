#!/usr/bin/python3
# File name   : picarDrive.py
# Description : encapsulate the drive system for a Robotic car

from iotServerLib import piDcMotor, piServoI2c, piRGBLed, piIotNode
from iotServerLib.iotCommon import ON, OFF, RGB

class PiCarDrive(piIotNode.PiIotNode):
    """ encapsulate the drive system for a Robotic car """
    def __init__(self, name, parent, config):
        """ construct a PiCarDrive """
        super(PiCarDrive, self).__init__(name, parent)
        self.motor = piDcMotor.PiDcMotor('drive', self, enable=config.getOrAddInt('motor.enable', 7),
                                        in1=config.getOrAddInt('motor.in1', 10), in2=config.getOrAddInt('motor.in2', 8))
        self.steering = piServoI2c.PiServoI2c('Steering', self, channel=config.getOrAddInt('steering.channel', 2),
                                             min=config.getOrAddInt('steering.min', 200), max=config.getOrAddInt('steering.max', 510),
                                             minAngle=config.getOrAddInt('steering.minAngle', 45), maxAngle=config.getOrAddInt('steering.maxAngle', -30),
                                             centerAngle=config.getOrAddInt('steering.centerAngle', 0))
        self.maxLeftAngle = min(self.steering.minAngle, self.steering.maxAngle)
        self.maxRightAngle = max(self.steering.minAngle, self.steering.maxAngle)
        self.leftLed = piRGBLed.PiRGBLed('Left LED', self, config.getOrAddInt('leftLED.redPin', 15), config.getOrAddInt('leftLED.greenPin', 16), config.getOrAddInt('leftLED.bluePin', 18))
        self.rightLed = piRGBLed.PiRGBLed('Right LED', self, config.getOrAddInt('rightLED.redPin', 19), config.getOrAddInt('rightLED.greenPin', 21), config.getOrAddInt('rightLED.bluePin', 22))
        self.turnSignal = False

    def stop(self):
        """ stop the motor, straight steering, and turn off led lights """
        self.motor.stop()
        self.leftLed.off()
        self.rightLed.off()
        self.steering.moveToCenter()

    def stopMotor(self, turnOffLeds=False):
        """ stop the motor """
        self.motor.stop()
        if turnOffLeds:
            self.leftLed.off()
            self.rightLed.off()

    def run(self, speed, steeringAngle=0):
        """ move car with specified speed and steering angle. 
        The speed ranges from -100 (backward) to 0 (stop) to 100 (forward).
        The steering angle ranges from -90 (left) to 0 (straight) to 90 (right) """
        self.steering.moveToAngle(steeringAngle)
        self.motor.run(speed)

    def forward(self, speed):
        """ move car forward with specified speed. The speed ranges from 0 (stop) to 100 (forward). """
        self.motor.run(speed)

    def backward(self, speed):
        """ move car backward with specified speed. The speed ranges from 0 (stop) to 100 (backward). """
        self.motor.run(-speed)

    def turnSteering(self, angle, turnSignal=True):
        """ turn the steering to the specified angle
        the angle range (in degree): -90 (max left) - 0 (straight) - +90 (max right)
        however, this angle will be clamped to the physical limitation as defined in constructor.
        the method return the achieved angle position.
        """
        if angle == 0:
            value = self.turnStraight()
        elif angle < 0:
            value = self.turnLeft(-angle, turnSignal)
        else:
            value = self.turnRight(angle, turnSignal)
        return value

    def turnStraight(self):
        """ turn the steering to straight position and turn off led signal
        the method return the achieved angle position.
        """
        value = self.steering.moveToCenter()
        if self.turnSignal:
            self.leftLed.off()
            self.rightLed.off()
            self.turnSignal = False
        return value

    def turnLeft(self, angle, turnSignal=True):
        """ turn the steering to left and turn on left led signal. angle should be 0 to 90.
        the method return the achieved angle position.
        """
        value = self.steering.moveToAngle(-angle)
        if turnSignal:
            self.leftLed.set(ON, ON, OFF)
            self.rightLed.set(OFF, OFF, OFF)
            self.turnSignal = True
        return value

    def turnRight(self, angle, turnSignal=True):
        """ turn the steering to right and turn on right led signal
        the method return the achieved angle position.
        """
        value = self.steering.moveToAngle(angle)
        if turnSignal:
            self.rightLed.set(ON, ON, OFF)
            self.leftLed.set(OFF, OFF, OFF)
            self.turnSignal = True
        return value

    def setLeds(self, red, green, blue):
        """ turn both LEDs with the same RGB value """
        self.rightLed.set(red, green, blue)
        self.leftLed.set(red, green, blue)

    def setLedsRGB(self, rgb):
        """ turn both LEDs with the same RGB value """
        self.rightLed.setRGB(rgb)
        self.leftLed.setRGB(rgb)





