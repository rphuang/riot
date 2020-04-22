#!/usr/bin/python3
# File name   : piLineTracking.py
# Description : encapsulates a line tracking device for Raspberry Pi

import RPi.GPIO as GPIO
import time

from iotServerLib import piIotNode

class PiLineTracking(piIotNode.PiIotNode):
    """ encapsulates a line tracking sensor for Raspberry Pi """
    def __init__(self, name, parent, leftPin, middlePin, rightPin):
        """ construct a PiLineTracking
        name: the name of the node
        parent: parent IotNode object. None for root node.
        leftPin: the pin number for the left input
        middlePin: the pin number for the middle input
        rightPin: the pin number for the right input
        """
        super(PiLineTracking, self).__init__(name, parent)
        GPIO.setmode(GPIO.BOARD)
        GPIO.setup(leftPin, GPIO.IN)
        GPIO.setup(middlePin, GPIO.IN)
        GPIO.setup(rightPin, GPIO.IN)
        self.leftPin = leftPin
        self.middlePin = middlePin
        self.rightPin = rightPin

    def status(self):
        """ get current sensor status
        return input status of [left, middle, right] """
        right = GPIO.input(self.rightPin)
        middle = GPIO.input(self.middlePin)
        left = GPIO.input(self.leftPin)
        return [left, middle, right]

    def isLeft(self):
        """ get left input status """
        return GPIO.input(self.leftPin)

    def isRight(self):
        """ get right input status """
        return GPIO.input(self.rightPin)

    def isMiddle(self):
        """ get middle input status """
        return GPIO.input(self.middlePin)





