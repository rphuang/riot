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
AutoWanderMode = 3          # autonomous wander around
FaceTrackingMode = 4        # track face and mve head to see the tracked face

# constants for strip led modes
StripModeManual = 0         # strip leds controlled manually
StripModeRainbowCycle = 1   # display rainbow cycle
StripModeChaseRainbow = 2   # display chase rainbow
StripModeAuto = 3           # display during forward, backward, and turning

# constants for wander states
WanderStateInit = 0         # this is the initial state.
WanderStateMove = 1         # the piCar is moving forward.
WanderStateStop = 2         # the picar is stopped due to obstacle
WanderStateScan = 3         # scan distance for surroundings and pick the direction with farest distance
WanderStateTurn = 4         # turning to the direction with the best distance. then go to init state.
WanderStateBack = 5         # move the car backward if failed to find the best distance from the scan then repeat scan

