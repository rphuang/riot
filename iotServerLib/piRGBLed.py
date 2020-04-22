#!/usr/bin/python3
# File name   : piRGBLed.py
# Description : encapsulates a RGB LED for Raspberry Pi

import RPi.GPIO as GPIO
from iotServerLib.iotCommon import *
from iotServerLib import piIotNode

# todo: support pwm
class PiRGBLed(piIotNode.PiIotNode):
    """ encapsulates an on/off RGB LED for Raspberry Pi
    Note: 0 is off otherwise on
    todo: support pwm
    """
    def __init__(self, name, parent, redPin, greenPin, bluePin, gpioLowToTurnOn=True):
        """ construct a PiRGBLed
        name: the name of the node
        parent: parent IotNode object. None for root node.
        redPin: the pin number for the red color
        greenPin: the pin number for the green color
        bluePin: the pin number for the blue color
        gpioLowToTurnOn: physical IO to low voltage to turn on the LED
        """
        super(PiRGBLed, self).__init__(name, parent)
        self.redPin = redPin
        self.greenPin = greenPin
        self.bluePin = bluePin
        self.gpioLowToTurnOn = gpioLowToTurnOn
        GPIO.setwarnings(False)
        GPIO.setmode(GPIO.BOARD)
        GPIO.setup(redPin, GPIO.OUT)
        GPIO.setup(greenPin, GPIO.OUT)
        GPIO.setup(bluePin, GPIO.OUT)
        self.set(OFF, OFF, OFF)

    def get(self):
        """ return list of RGB values"""
        return self.rgb.toRGBList()

    def getRGBStr(self):
        """ return list of RGB values in string """
        return self.rgb.toRGBStr()

    def on(self):
        """ turn on the RGB Led """
        self.set(ON, ON, ON)

    def off(self):
        """ turn off the RGB Led """
        self.set(OFF, OFF, OFF)

    def set(self, red, green, blue):
        """ set value with RGB on/off values. 0 is off otherwise on. """
        self._output(self.redPin, red)
        self._output(self.greenPin, green)
        self._output(self.bluePin, blue)
        self.rgb = RGB(red, green, blue)

    def setRGB(self, rgb):
        """ set value by RGB object. 0 is off otherwise on. Ex: 0,255,0 """
        red, green, blue = rgb.toRGBList()
        self.set(red, green, blue)

    def setRGBStr(self, rgbStr):
        """ set value by a comma separated RGB string. 0 is off otherwise on. Ex: 0,255,0 """
        red, green, blue = getOnOffFromRGBStr(rgbStr)
        self.set(red, green, blue)

    def _output(self, pin, value):
        """ output to the pin and reverse the HIGH/LOW if gpioLowToTurnOn is true """
        if self.gpioLowToTurnOn:
            if value == OFF:
                value = GPIO.HIGH     # 1
            else:
                value = GPIO.LOW      # 0
        GPIO.output(pin, value)







