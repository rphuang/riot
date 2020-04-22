#!/usr/bin/python3
# File name   : piUltrasonic.py
# Description : encapsulates an Ultrasonic sensor for Raspberry Pi

import RPi.GPIO as GPIO
import time

from iotServerLib import piIotNode

class PiUltrasonic(piIotNode.PiIotNode):
    """ encapsulates an Ultrasonic sensor for Raspberry Pi """
    def __init__(self, name, parent, triggerPin, echoPin, maxDistanceCm):
        """ construct a PiUltrasonic
        name: the name of the node
        parent: parent IotNode object. None for root node.
        triggerPin: the pin number for the trigger
        echoPin: the pin number for the echo
        maxDistanceCm: max distance to measure in cm
        """
        super(PiUltrasonic, self).__init__(name, parent)
        GPIO.setmode(GPIO.BOARD)
        GPIO.setup(triggerPin, GPIO.OUT, initial=GPIO.LOW)
        GPIO.setup(echoPin, GPIO.IN)
        self.triggerPin = triggerPin
        self.echoPin = echoPin
        self._timeout = 2.0 * maxDistanceCm / 100.0 / 340.0
        self.pingDistance()

    def pingDistance(self):
        """ trigger the ping and get distance returns -1 when error """
        GPIO.output(self.triggerPin, GPIO.HIGH)
        time.sleep(0.000015)
        GPIO.output(self.triggerPin, GPIO.LOW)
        value = self._measure()
        return value

    def _measure(self):
        t0 = time.time() 
        while not GPIO.input(self.echoPin):
            elapseTime = time.time() - t0
            if elapseTime  > self._timeout:
                #print(self.name + ' ultrasonic timeout: ' + str(elapseTime))
                return -1
        t1 = time.time()
        while GPIO.input(self.echoPin):
            elapseTime = time.time() - t0
            if elapseTime  > self._timeout:
                #print(self.name + ' ultrasonic timeout: ' + str(elapseTime))
                return -1
        t2 = time.time()
        # record last good distance
        self.distance = (t2-t1)*340/2
        return self.distance




