#!/usr/bin/python3
# File name   : piStripLed.py
# Description : encapsulates a RGB LED strip for Raspberry Pi

import time
from rpi_ws281x import Adafruit_NeoPixel, Color
from iotServerLib.iotCommon import RGB
from iotServerLib import piIotNode

class PiStripLed(piIotNode.PiIotNode):
    """ encapsulates the RGB led strip using ws281x """
    def __init__(self, name, parent, ledCount, ledPin, freq=800000, dmaChannel=10, invert=False, brightness=255, ledChannel=0, debug=False):
        """ construct a strip led """
        super(PiStripLed, self).__init__(name, parent)
        self.debug = debug
        # Create NeoPixel object with appropriate configuration.
        self.strip = Adafruit_NeoPixel(ledCount, ledPin, freq, dmaChannel, invert, brightness, ledChannel)
        # Intialize the library (must be called once before other functions).
        self.strip.begin()

    def setPixel(self, i, red, green, blue):
        """ set color for a single pixel with red, green, blue values"""
        self.setPixelColor(i, Color(red, green, blue))

    def setPixelRGB(self, i, rgb):
        """ set color for a single pixel with RGB object """
        red, green, blue = rgb.toRGBList()
        self.setPixelColor(i, Color(int(red), int(green), int(blue)))

    def setPixelRGBStr(self, i, rgbStr):
        """ set color for a single pixel with RGB string """
        red, green, blue = rgbStr.split(',')
        self.setPixelColor(i, Color(int(red), int(green), int(blue)))

    def setPixelColor(self, i, color):
        """ set color for a single pixel with rpi_ws281x Color object """
        self.strip.setPixelColor(i, color)
        self.strip.show()

    def setAllPixels(self, red, green, blue, delay_ms=5):
        """ set color across all pixels with delay time between pixel."""
        self.setAllPixelsColor(Color(red, green, blue), delay_ms)

    def setAllPixelsRGB(self, rgb, delay_ms=5):
        """ set color across all pixels with delay time between pixel."""
        red, green, blue = rgb.toRGBList()
        self.setAllPixelsColor(Color(red, green, blue), delay_ms)

    def setAllPixelsRGBStr(self, rgbStr, delay_ms=5):
        """ set color across all pixels with delay time between pixel."""
        red, green, blue = rgbStr.split(',')
        self.setAllPixelsColor(Color(int(red), int(green), int(blue)), delay_ms)

    def setAllPixelsColor(self, color, delay_ms=5):
        """ set color across all pixels with delay time between pixel."""
        for i in range(self.strip.numPixels()):
            self.strip.setPixelColor(i, color)
            self.strip.show()
            time.sleep(delay_ms/1000.0)

    @staticmethod
    def wheel(pos):
        """ generate rainbow colors across 0-255 positions """
        if pos < 85:
            return Color(pos * 3, 255 - pos * 3, 0)
        elif pos < 170:
            pos -= 85
            return Color(255 - pos * 3, 0, pos * 3)
        else:
            pos -= 170
            return Color(0, pos * 3, 255 - pos * 3)

    def rainbowCycle(self, delay_ms=20, iterations=5):
        """ Draw rainbow that uniformly distributes itself across all pixels """
        self._debugInfo('start rainbowCycle')
        for j in range(256*iterations):
            for i in range(self.strip.numPixels()):
                self.strip.setPixelColor(i, PiStripLed.wheel((int(i * 256 / self.strip.numPixels()) + j) & 255))
            self.strip.show()
            time.sleep(delay_ms/1000.0)
        self._debugInfo('end rainbowCycle')

    def theaterChaseRainbow(self, delay_ms=50):
        """Rainbow movie theater light style chaser animation."""
        self._debugInfo('start theaterChaseRainbow')
        for j in range(256):
            for q in range(3):
                for i in range(0, self.strip.numPixels(), 3):
                    self.strip.setPixelColor(i+q, PiStripLed.wheel((i+j) % 255))
                self.strip.show()
                time.sleep(delay_ms/1000.0)
                for i in range(0, self.strip.numPixels(), 3):
                    self.strip.setPixelColor(i+q, 0)
        self._debugInfo('end theaterChaseRainbow')

    def _debugInfo(self, msg):
        """ print debug info when debug is enabled """
        if self.debug:
            print(msg)

 