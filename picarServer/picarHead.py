#!/usr/bin/python3
# File name   : picarHead.py
# Description : encapsulate the "head" for a Robotic car

import time
from piServices.piUtils import timePrint
from iotServerLib import piServoI2c, piUltrasonic, piIotNode
from picarCamera import PiCarCamera

class PiCarHead(piIotNode.PiIotNode):
    """ encapsulate the "head" for a Robotic car that contains:
    camera, ultrasonic, and servos for horizontal and vertical position """
    def __init__(self, name, parent, config):
        """ construct a PiCarHead """
        super(PiCarHead, self).__init__(name, parent)
        self.ultrasonic = piUltrasonic.PiUltrasonic('Ultrasonic', self, triggerPin=config.getOrAddInt('ultrasonic.triggerPin', 23),
                                                   echoPin=config.getOrAddInt('ultrasonic.echoPin', 24), maxDistanceCm=config.getOrAddInt('ultrasonic.maxDistanceCm', 1500))
        # servoH physical limits min=110 max=700 minAngle=67 maxAngle=-130
        #               clamp to min=130 max=670 minAngle=60 maxAngle=-120
        self.servoH = piServoI2c.PiServoI2c('HorizontalServo', self, channel=config.getOrAddInt('horizontalServo.channel', 1),
                                           min=config.getOrAddInt('horizontalServo.min', 130), max=config.getOrAddInt('horizontalServo.max', 670),
                                           minAngle=config.getOrAddInt('horizontalServo.minAngle', 60), maxAngle=config.getOrAddInt('horizontalServo.maxAngle', -120),
                                           centerAngle=config.getOrAddInt('horizontalServo.centerAngle', 0))
        self.maxLeftAngle = min(self.servoH.minAngle, self.servoH.maxAngle)
        self.maxRightAngle = max(self.servoH.minAngle, self.servoH.maxAngle)
        # servoV physical limits min=270 max=670 minAngle=-60 maxAngle=95 (center angle shift up by ~15 degree)
        #               clamp to min=295 max=645 minAngle=-50 maxAngle=85
        self.servoV = piServoI2c.PiServoI2c('VerticalServo', self, channel=config.getOrAddInt('verticalServo.channel', 0),
                                           min=config.getOrAddInt('verticalServo.min', 295), max=config.getOrAddInt('verticalServo.max', 645),
                                           minAngle=config.getOrAddInt('verticalServo.minAngle', -60), maxAngle=config.getOrAddInt('verticalServo.maxAngle', 95),
                                           centerAngle=config.getOrAddInt('verticalServo.centerAngle', 0))
        self.maxDownAngle = min(self.servoV.minAngle, self.servoV.maxAngle)
        self.maxUpAngle = max(self.servoV.minAngle, self.servoV.maxAngle)
        self.camera = PiCarCamera('Camera', self, config, debug=True)
        self.scanning = False   # flag indicating ultrasonic scan is in progress
        self.heading = (self.servoH.angle, self.servoV.angle)   # direction (horizontalAngle, verticalAngle)

    def stop(self):
        """ stop - just move to straight for both servos """
        self.lookStraight()

    def moveHorizontal(self, angle):
        """ turn the head horizontally to the specified angle
        the angle range (in degree): -90 (max left) - 0 (straight) - +90 (max right)
        however, this angle will be clamped to the physical limitation.
        the method return the achieved angle position.
        """
        val = self.servoH.moveToAngle(angle)
        self.heading = (self.servoH.angle, self.servoV.angle)
        return val

    def moveVertical(self, angle):
        """ turn the camera vertically to the specified angle
        the angle range (in degree): -90 (max down) - 0 (straight) - +90 (max up)
        however, this angle will be clamped to the physical limitation.
        the method return the achieved angle position.
        """
        val = self.servoV.moveToAngle(angle)
        self.heading = (self.servoH.angle, self.servoV.angle)
        return val

    def lookStraight(self):
        """ point the head to straight """
        self.servoH.moveToCenter()
        self.servoV.moveToCenter()
        self.heading = (self.servoH.angle, self.servoV.angle)

    def scan(self, starth, startv, endh, endv, inch, incv, delay=0.2):
        """ perform distance scan with inputs
        starth, startv - start position for horizontal and vertical
        endh, endv - end position for horizontal and vertical
        inch, incv - increment position for horizontal and vertical
        delay - delay time between each scan point

        returns arrays of the followings:
        - array of distance values
        - array of horizonal positions
        - array of vertical positions
        """
        hOriginalPos = self.servoH.angle
        vOriginalPos = self.servoV.angle
        self.scanning = True
        timePrint('Scan starth ' + str(starth) + ' startv ' + str(startv) + ' endh ' + str(endh) + ' endv ' + str(endv) + ' inch ' + str(inch) + ' incv ' + str(incv))
        values = []
        hValues = []
        vValues = []
        hPos = starth
        vPos = startv
        self.servoH.moveToAngle(hPos) # move to horizontal start position
        self.servoV.moveToAngle(vPos)   # move to vertical start position
        time.sleep(delay)          #Wait for the Ultrasonic to be in position
        while hPos<endh or vPos<endv:         #Scan,from start to end
            #timePrint('scan hpos: ' + str(hPos) + ' vpos: ' + str(vPos))
            new_scan_data=round(self.ultrasonic.pingDistance(),2)   #Get a distance of a certern direction
            values.append(new_scan_data)              #Put that distance value into a list,and save it as String-Type for future transmission 
            hValues.append(hPos)
            vValues.append(vPos)
            hPosNew = hPos + inch
            if hPosNew > endh:
                hPosNew = endh
            vPosNew = vPos + incv
            if vPosNew > endv:
                vPosNew = endv
            if hPos != hPosNew:
                hPos = hPosNew
                self.servoH.moveToAngle(hPos)
            if vPos != vPosNew:
                vPos = vPosNew
                self.servoV.moveToAngle(vPos)
            time.sleep(delay)
        # move back to original positions
        self.servoH.moveToAngle(hOriginalPos)
        self.servoV.moveToAngle(vOriginalPos)
        self.scanning = False
        return [values, hValues, vValues]





