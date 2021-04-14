#!/usr/bin/python3
# File name   : picar.py
# Description : encapsulate an Adeept Robotic PiCar

import time
import threading

from iotServerLib import piStripLed, piIotNode, piLineTracking
from iotServerLib.iotCommon import ON, OFF, RGB
from piServices.piUtils import timePrint, startThread
import picarDrive
import picarHead
import picarConst



class PiCar(piIotNode.PiIotNode):
    """ encapsulate an Adeept Robotic PiCar """
    def __init__(self, name, parent, config):
        """ construct a PiCar """
        super(PiCar, self).__init__(name, parent)
        self.config = config
        self.drive = picarDrive.PiCarDrive('drive', self, self.config)
        self.head = picarHead.PiCarHead('head', self, self.config)
        self.strip = piStripLed.PiStripLed('StripLed', self, ledCount=self.config.getOrAddInt('stripLed.ledCount', 12), ledPin=self.config.getOrAddInt('stripLed.ledPin', 12))
        self.lineTracking = piLineTracking.PiLineTracking('lineTracking', self, self.config.getOrAddInt('lineTracking.leftPin', 38),
                                          self.config.getOrAddInt('lineTracking.middlePin', 36), self.config.getOrAddInt('lineTracking.rightPin', 35))
        self.httpVideoPort = self.config.getOrAddInt('picar.httpVideoPort', 8000)
        # initialize 
        self._resetModes()
        self.distance = 0

    def startUp(self):
        """ start up all PiCar functions """
        # modes initialization
        self._resetModes()
        # start threads for picar
        self.ws2812Thread=startThread('ws2812', target=self._ws2812Worker)                  # thread for stripled lights (ws_2812 leds)
        self.scanThread=startThread('DistanceScan', target=self._distanceScanWorker)        # thread for distance scan (ultrasonic)
        self.httpvideoThread=startThread('HttpVideoStream', target=self._httpVideoWorker)   # thread for http video streaming (flask)
        self.modeThread=startThread('ModeManager', target=self._modeWorker)                 # thread for managing PiCar operation modes
        # stop motor, move servos to center, 
        self.stop()
        # turn on green lights
        self.drive.setLedsRGB(picarConst.GREEN)

    def shutDown(self):
        """ stop all components """
        # todo: stop all threads
        self._resetModes()
        self.stop()

    def stop(self):
        """ stop PiCar """
        self.drive.stop()
        self.head.stop()
        self.stopStrip()

    def setOperationMode(self, mode):
        """ set PiCar's operation mode """
        if mode >= picarConst.ManualMode and mode <= picarConst.FaceTrackingMode:
            self.mode = mode
            return True
        else:
            timePrint('Invalid operation mode: %i' %mode)
            return False

    def setStrip(self, valueStr):
        """ set value for strip valid values (in string)
        - off - stop lightshow (StripModeManual)
        - cycle - rainbowCycle lightshow (StripModeRainbowCycle)
        - chase - theaterChaseRainbow lightshow (StripModeChaseRainbow)
        - auto - auto mode (StripModeAuto)
        - color value in the format of "rrr, ggg, bbb" separated by comma
        """
        if 'off' in valueStr:
            self.stripMode = picarConst.StripModeManual
        elif 'cycle' in valueStr:
            self.stripMode = picarConst.StripModeRainbowCycle
        elif 'chase' in valueStr:
            self.stripMode = picarConst.StripModeChaseRainbow
        elif 'auto' in valueStr:
            self.stripMode = picarConst.StripModeAuto
        else:
            try:
                self.stripMode = picarConst.StripModeManual
                self.strip.setAllPixelsRGBStr(valueStr, delay_ms=0)
            except:
                print('Invalid RGB value: ' + valueStr)

    def stopStrip(self):
        """ stop strip and turn off all LEDs """
        if self.stripMode == picarConst.StripModeManual:
            # todo: handling strip to complete its cycle
            self.stripMode = picarConst.StripModeManual
        self.strip.setAllPixels(0, 0, 0, delay_ms=0)

    def _resetModes(self):
        """ initialize PiCar's modes """
        self.mode = picarConst.ManualMode
        self.stripMode = picarConst.StripModeManual
        self.distanceScan = True

    def _stopAuto(self):
        """ internal method to stop auto modes """
        self.mode = picarConst.ManualMode
        self.stop()

    def _ws2812Worker(self, interval=0.1):
        """ internal thread for handling strip led operations """
        oldMode = self.stripMode
        while True:
            if self.stripMode == picarConst.StripModeManual:
                if oldMode != picarConst.StripModeManual:
                    self.strip.setAllPixels(0, 0, 0, delay_ms=0)
            elif self.stripMode == picarConst.StripModeRainbowCycle:
                self.strip.rainbowCycle(delay_ms=50, iterations=1)
            elif self.stripMode == picarConst.StripModeChaseRainbow:
                self.strip.theaterChaseRainbow(delay_ms=50)
            elif self.stripMode == picarConst.StripModeAuto:
                if self.drive.motor.speed > 10:     # moving forward
                    self.strip.rainbowCycle(delay_ms=50, iterations=1)
                elif self.drive.motor.speed < -10:  # moving backward
                    self.strip.setAllPixels(255, 0, 0, delay_ms=0)
                else:
                    if self.drive.steering.angle < -10: # turn left
                        self.strip.setPixelColor(0, Color(255,255,0))
                        self.strip.setPixelColor(1, Color(255,255,0))
                        self.strip.setPixelColor(2, Color(255,255,0))
                    elif self.drive.steering.angle > 10:    # turn right
                        self.strip.setPixelColor(3, Color(255,255,0))
                        self.strip.setPixelColor(4, Color(255,255,0))
                        self.strip.setPixelColor(5, Color(255,255,0))
                    else:
                        self.strip.setAllPixels(0, 0, 0, delay_ms=0)
            oldMode = self.stripMode
            time.sleep(interval)

    def _httpVideoWorker(self):          # video via http
        """ internal thread for video streaming """
        self.head.camera.httpVideoStreaming(self.httpVideoPort)

    def _distanceScanWorker(self):
        """ internal thread for measuring distance at specified interval """
        interval = self.config.getOrAddFloat('distanceScan.scanCycleInSecond', 0.2)             # delay interval for the worker
        stopDistance = self.config.getOrAddFloat('distanceScan.stopDistanceInMeter', 0.2)       # the distance to stop forward movement
        slowDistance = self.config.getOrAddFloat('distanceScan.slowDistanceInMeter', 1.0)       # the distance to stop forward movement
        headingAngleLimit = self.config.getOrAddInt('distanceScan.headingAngleLimit', 20)       # the angle limit considered as measuring straight ahead
        while True:
            slowdown = 0
            if self.distanceScan and not self.head.scanning:
                distance = self.head.ultrasonic.pingDistance()
                if distance < 0:
                    # give it one more try
                    distance = self.head.ultrasonic.pingDistance()
                self.distance = distance
                # check distance to stop drive
                if stopDistance > 0 and distance > 0:
                    # check heading and forward (speed > 0) before stopping
                    hAngle, vAngle = self.head.heading
                    if abs(hAngle) < headingAngleLimit and abs(vAngle) < headingAngleLimit and self.drive.motor.speed > 0:
                        if distance < stopDistance:
                            timePrint('Stopping drive at distance: %f' %distance)
                            self.drive.stop()
                            slowdown=0
                        elif distance < slowDistance:
                            slowdown = -int(30 * (slowDistance - distance) / (slowDistance - stopDistance))
                            #timePrint('Slowing down %i at distance: %f' %(slowdown, distance))

            self.drive.extraSpeed(slowdown)

            time.sleep(interval)

    def _initMode(self, mode):
        """ initialization of the mode - will be called only when first time swith to the mode """
        if mode == picarConst.ManualMode:
            # stop auto mode when switching from auto to manual mode
            self._stopAuto()
        elif mode == picarConst.FollowDistanceMode:
            self.head.lookStraight()
            self.drive.turnStraight()
            self.distanceScan = True    # make sure distance scan worker thread to stop before hitting obstacle
        elif mode == picarConst.FollowLineMode:
            pass
        elif mode == picarConst.AutoWanderMode:
            self._wanderState = picarConst.WanderStateInit   # wander states: 0-init, 1-move, 2-stop, 3-scan, 4-turn, 5-back
            self._wanderOldState = picarConst.WanderStateInit
            self._wanderCounter = 0
            self.distanceScan = True    # make sure distance scan worker thread to stop before hitting obstacle
        elif mode == picarConst.FaceTrackingMode:
            self._faceId = -1       # valid face ID should be >= 0

    def _stopMode(self, mode):
        """ initialization of the mode - will be called only when first time swith to the mode """
        if mode == picarConst.ManualMode:
            pass
        elif mode == picarConst.FollowDistanceMode:
            self.stop()
        elif mode == picarConst.FollowLineMode:
            self.stop()
        elif mode == picarConst.AutoWanderMode:
            self.stop()
        elif mode == picarConst.FaceTrackingMode:
            pass

    def _modeWorker(self, interval=0.2):
        """ internal thread for handling PiCar's operation modes """
        oldMode = self.mode
        while True:
            if oldMode != self.mode:
                # mode change - stop old mode and init new mode
                self._stopMode(oldMode)
                self._initMode(self.mode)
            if self.mode == picarConst.ManualMode:
                pass
            elif self.mode == picarConst.FollowDistanceMode:
                self._followByDistance()
            elif self.mode == picarConst.FollowLineMode:
                self._followLine()
            elif self.mode == picarConst.AutoWanderMode:
                self._wander(interval)
            elif self.mode == picarConst.FaceTrackingMode:
                self._faceTracking()
            else:
                self._stopAuto()
            oldMode = self.mode
            time.sleep(interval)

    def _followByDistance(self):
        """ internal function for followDistance mode
        follow with Ultrasonic by keeping the same distance to target
        this function leverage _distanceScanWorker to stop 
        """
        maxDistance = self.config.getOrAddFloat('follow.maxFollowDistance', 2.0)
        dis = self.distance
        if dis < maxDistance:             #Check if the target is in diatance range
            distanceToFollow = self.config.getOrAddFloat('distanceScan.stopDistanceInMeter', 0.2)        # keep the distance to the target
            distanceOffset = self.config.getOrAddFloat('follow.distanceOffset', 0.1)    # controls the sensitivity
            if dis > (distanceToFollow + distanceOffset) :   #If the target is in distance range and out of distanceToFollow, then move forward
                if self.drive.motor.speed > 0:
                    pass
                else:
                    self.drive.setLedsRGB(picarConst.CYAN)
                    self.drive.forward(self.config.getOrAddInt('auto.forwardSpeed', 60))
                    timePrint('followByDistance - move forward. distance: %s' %dis)
            elif dis < (distanceToFollow - distanceOffset) : #Check if the target is too close, if so, the car move back to keep distance at distance
                if self.drive.motor.speed < 0:
                    pass
                else:
                    self.drive.setLedsRGB(picarConst.PINK)
                    self.drive.backward(self.config.getOrAddInt('auto.backwardSpeed', 60))
                    timePrint('followByDistance - move backward. distance: %s' %dis)
            else:                            #If the target is at distance, then the car stay still
                self.drive.setLedsRGB(picarConst.GREEN)
                if abs(self.drive.motor.speed) > 5:
                    self.drive.stopMotor()
        else:
            if abs(self.drive.motor.speed) > 5:
                self.drive.stopMotor()

    def _followLine(self):
        left, middle, right = self.lineTracking.status()
        if middle:
            self.drive.run(speed=self.config.getOrAddInt('auto.forwardSpeed', 60), steeringAngle=0)
            self.drive.setLedsRGB(picarConst.YELLOW)
        elif left:
            self.drive.forward(speed=self.config.getOrAddInt('auto.forwardSpeed', 60))
            self.drive.turnLeft(angle=45, turnSignal=True)
        elif right:
            self.drive.forward(speed=self.config.getOrAddInt('auto.forwardSpeed', 60))
            self.drive.turnRight(angle=45, turnSignal=True)
        else:
            self.drive.backward(speed=self.config.getOrAddInt('auto.backwardSpeed', 60))
            self.drive.setLedsRGB(picarConst.CYAN)

    def _wander(self, interval):
        """ autonomous wander around mindlessly
        wander states: 
        0 - WanderStateInit - this is the initial state.
        1 - WanderStateMove - the piCar is moving forward.
        2 - WanderStateStop - the picar is stopped due to obstacle
        3 - WanderStateScan - scan distance for surroundings and pick the direction with farest distance
        4 - WanderStateTurn - turning to the direction with the best distance. then go to init state.
        5 - WanderStateBack - move the car backward if failed to find the best distance from the scan then repeat scan
        """
        if self._wanderState == picarConst.WanderStateInit:
            # start move forward
            self.head.lookStraight()
            self.drive.forward(speed=self.config.getOrAddInt('auto.forwardSpeed', 60))
            self._wanderState = picarConst.WanderStateMove
            timePrint('Wander state %i' %self._wanderState)
        elif self._wanderState == picarConst.WanderStateMove:
            # check whether the drive stopped
            if abs(self.drive.motor.speed) < 5:
                self._wanderState = picarConst.WanderStateStop
                timePrint('Wander state %i' %self._wanderState)
        elif self._wanderState == picarConst.WanderStateStop:
            # for now, just move to scan
            self._wanderState = picarConst.WanderStateScan
            timePrint('Wander state %i' %self._wanderState)
        elif self._wanderState == picarConst.WanderStateScan:
            # scan distance
            value = []
            posh = []
            posv = []
            starth = self.config.getOrAddInt('wander.scan.starth', -90)
            startv = self.head.servoV.angle     # use currently vertical angle
            endh = self.config.getOrAddInt('wander.scan.endh', 90)
            endv = startv
            inch = self.config.getOrAddInt('wander.scan.inc', 10)
            incv = inch
            value, posh, posv = self.head.scan(starth, startv, endh, endv, inch, incv)
            timePrint('Scan: %s' %(str(value)))
            # find max
            max = 0
            maxindex = -1
            index = 0
            stopDistance = self.config.getOrAddFloat('distanceScan.stopDistanceInMeter', 0.2)
            for val in value:
                if val > max and val > stopDistance:
                    max = val
                    maxindex = index
                index += 1
            if maxindex > -1:
                # found good one and turning to that direction
                angle = posh[maxindex]
                timePrint('maxindex %i value %f posh %i' %(maxindex, max, angle))
                if angle > 0:
                    angle = -self.config.getOrAddInt('wander.turnAngle', 30)
                else:
                    angle = self.config.getOrAddInt('wander.turnAngle', 30)
                self.drive.turnSteering(angle)
                self.drive.backward(self.config.getOrAddInt('wander.turnSpeed', 60))
                self._wanderTimer = int(self.config.getOrAddFloat('wander.turningTime', 2) / interval)
                self._wanderState = picarConst.WanderStateTurn
                timePrint('Wander state %i' %self._wanderState)
            else:
                # cannot find good one so move back
                self.drive.backward(self.config.getOrAddInt('auto.backwardSpeed', 60))
                self._wanderTimer = int(self.config.getOrAddFloat('wander.backwardTime', 1) / interval)
                self._wanderState = picarConst.WanderStateBack
                timePrint('Wander state %i' %self._wanderState)
        elif self._wanderState == picarConst.WanderStateTurn:
            # count down the timer then stop and go to init state
            self._wanderTimer -= 1
            if self._wanderTimer == 0:
                self.drive.stop()
                self._wanderState = picarConst.WanderStateInit
                timePrint('Wander state %i' %self._wanderState)
        elif self._wanderState == picarConst.WanderStateBack:
            # count down the timer then stop
            self._wanderTimer -= 1
            if self._wanderTimer == 0:
                self.drive.stop()
                self._wanderState = picarConst.WanderStateScan
                timePrint('Wander state %i' %self._wanderState)
        self._wanderCounter += 1
        if self._wanderOldState != self._wanderState:
            self._wanderCounter = 0
        self._wanderOldState = self._wanderState
        if self._wanderCounter > int(self.config.getOrAddFloat('wander.stateTimeout', 10) / interval):
            self.drive.stop()
            timePrint('Wander timeout go to scan state')
            self._wanderState = picarConst.WanderStateScan
        pass

    def _followObject(self):
        pass

    def _faceTracking(self):
        """ tracking a face by moving head to follow the face """
        faceTracker = self.head.camera.faceTracker
        trackedFaces = faceTracker.getTrackedFaces()
        if len(trackedFaces) == 0:
            if self._faceId < 0:
                # todo: searching faces by looking left/right
                pass
            return
        if self._faceId in trackedFaces:
            faceTrackingData = trackedFaces[self._faceId]
        #elif self._faceId < 0:
        else:
            # get the first face tracked
            self._faceId = next(iter(trackedFaces))
            faceTrackingData = trackedFaces[self._faceId]
            timePrint('Start tracking face ID %i' %self._faceId)
            # lost the tracked face
            #timePrint('Lost tracked face ID %i' %self._faceId)
            #return
        # find the center of the tracked face
        x, y, w, h = faceTrackingData.getPosition()
        x = int(x + w / 2)
        y = int(y + h / 2)
        # center of the image
        imageHeight, imageWidth, c = faceTracker.getImageShape()
        xc = int(imageWidth / 2)
        yc = int(imageHeight / 2)
        # calculate angles to move
        xd = int(((x - xc) / imageWidth) * self.config.getOrAddInt('faceTracking.horizontalViewAngle', 54))
        yd = int(((yc - y) / imageHeight) * self.config.getOrAddInt('faceTracking.verticalViewAngle', 42))
        #timePrint('x: %i y: %i xc: %i yc: %i xd: %i yd: %i' %(x, y, xc, yc, xd, yd))
        xAngle, yAngle = self.head.heading
        if abs(xd) > 5:
            angle = xAngle + xd
            #timePrint('Move head horizontal by %i degree to %i degree' %(xd, angle))
            self.head.moveHorizontal(angle)
        if abs(yd) > 5:
            angle = yAngle + yd
            #timePrint('Move head vertical by %i degree to %i degree' %(yd, angle))
            self.head.moveVertical(angle)
        

    @staticmethod
    def _getMovingTime(distance):
        moving_time = distance / 0.38
        if moving_time > 1:
            moving_time = 1
        return moving_time



