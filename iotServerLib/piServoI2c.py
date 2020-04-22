#!/usr/bin/python3
# File name   : piServoI2c.py
# Description : encapsulates a servo connected via Adafruit_PCA9685 for Raspberry Pi

import Adafruit_PCA9685

from iotServerLib import piIotNode

class PiServoI2c(piIotNode.PiIotNode):
    """ encapsulates a servo connected via Adafruit_PCA9685 for Raspberry Pi """
    def __init__(self, name, parent, channel, min, max, minAngle=0, maxAngle=180, centerAngle=90, freq=60):
        """ construct a servo
        name: the name of the node
        parent: parent IotNode object. None for root node.
        channel: the channel in the controller (0 to 15)
        min: min position
        max: max position
        minAngle: min angular position
        maxAngle: max angular position
        """
        super(PiServoI2c, self).__init__(name, parent)
        self.pwm = Adafruit_PCA9685.PCA9685()
        self.pwm.set_pwm_freq(freq)
        self.channel = channel
        self.min = min
        self.max = max
        self.minAngle = minAngle
        self.maxAngle = maxAngle
        self.centerAngle = centerAngle
        self._convert = (max - min) / (maxAngle - minAngle)
        self.angle = centerAngle
        #self.moveToAngle(centerAngle)

    def moveTo(self, position):
        """ move the servo to position. Return the achieved servo position. """
        if position < self.min:
            position = self.min
        elif position > self.max:
            position = self.max
        self.pwm.set_pwm(self.channel, 0, position)
        self.position = position
        self.angle = int((self.position - self.min) / self._convert) + self.minAngle
        return self.position

    def moveToAngle(self, angle):
        """ move the servo to angle. Return the achieved angle position. """
        self.moveTo(int((angle - self.minAngle) * self._convert) + self.min)
        return self.angle

    def moveToCenter(self):
        """ move the servo to center position. Return the achieved angle position. """
        self.moveToAngle(self.centerAngle)
        return self.angle






