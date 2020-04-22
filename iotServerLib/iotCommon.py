#!/usr/bin/python3
# File name   : iotCommon.py
# Description : defines utilities and constants

# on/off value
ON  = 1
OFF = 0

class RGB():
    """ encapsulates a color """
    def __init__(self, red, green, blue):
        """ construct a Color with red, green, blue """
        self.red = red
        self.green = green
        self.blue = blue

    def toRGBList(self):
        """ returns list of [red, green, blue] """
        return [self.red, self.green, self.blue]

    def toRGBStr(sef):
        """ return RGB in string format separated by common as 'red, green, blue' """
        return str(self.red) + ', ' + str(self.green) + ', ' + str(self.blue)

    @staticmethod
    def createRGBFromRGBStr(rgbStr):
        """ returns [red, green, blue] that represents whether each led is on or off """
        red, green, blue = rgbStr.split(',')
        return RGB(red, green, blue)

def getRGBListFromRGBStr(rgbStr):
    """ returns [red, green, blue] that represents whether each led is on or off """
    red, green, blue = rgbStr.split(',')
    return [red, green, blue]

def getOnOffFromRGBStr(rgbStr):
    """ returns [red, green, blue] that represents whether each led is on or off """
    red, green, blue = rgbStr.split(',')
    return [convertToOnOff(red), convertToOnOff(green), convertToOnOff(blue)]

def convertToOnOff(strval):
    """ converting none-zero value to ON (1) """
    if int(strval) == 0:
        return OFF
    else:
        return ON

