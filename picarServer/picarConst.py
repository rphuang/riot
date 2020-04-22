#!/usr/bin/python3
# File name   : picarConst.py
# Description : define constants for the PiCar

from iotServerLib.iotCommon import ON, OFF, RGB

# constants for RGB colors
RGBON = RGB(ON, ON, ON)
RGBOFF = RGB(OFF, OFF, OFF)
RED = RGB(ON, OFF, OFF)
GREEN = RGB(OFF, ON, OFF)
BLUE = RGB(OFF, OFF, ON)
YELLOW = RGB(ON, ON, OFF)
PINK = RGB(ON, OFF, ON)
CYAN = RGB(OFF, ON, ON)

# constants for PiCar operation modes
ManualMode = 0
FollowDistanceMode = 1      # follow a fix distance to target (using ultrasonic sensor)
FollowLineMode = 2          # follow line on the ground
FollowObjectMode = 3        # follow object with drive and/or head
FaceTrackingMode = 4        # track face and mve head to see the tracked face

# constants for strip led modes
StripModeManual = 0         # strip leds controlled manually
StripModeRainbowCycle = 1   # display rainbow cycle
StripModeChaseRainbow = 2   # display chase rainbow
StripModeAuto = 3           # display during forward, backward, and turning


