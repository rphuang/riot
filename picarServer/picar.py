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

# constants for follow distance
DistanceToFollow  = 0.4
MaxFollowDistance = 2


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
            timePrint('Invalid operation mode: ' + mode)
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
        interval = self.config.getOrAddFloat('distanceScan.scanCycleInSecond', 0.3)             # delay interval for the worker
        stopDistance = self.config.getOrAddFloat('distanceScan.stopDistanceInMeter', 0.2)       # the distance to stop forward movement
        slowDistance = self.config.getOrAddFloat('distanceScan.slowDistanceInMeter', 1.0)       # the distance to stop forward movement
        headingAngleLimit = self.config.getOrAddInt('distanceScan.headingAngleLimit', 20)       # the angle limit considered as measuring straight ahead
        while True:
            slowdown = 0
            if self.distanceScan and not self.head.scanning:
                self.distance = self.head.ultrasonic.pingDistance()
                # check distance to stop drive
                if stopDistance > 0 and self.distance > 0:
                    # check heading and forward (speed > 0) before stopping
                    hAngle, vAngle = self.head.heading
                    if abs(hAngle) < headingAngleLimit and abs(vAngle) < headingAngleLimit and self.drive.motor.speed > 0:
                        if self.distance < stopDistance:
                            timePrint('Stopping drive at distance: %f' %self.distance)
                            self.drive.stop()
                            self.drive.extraSpeed(0)
                        elif self.distance < slowDistance:
                            slowdown = -int(20 * (slowDistance - self.distance) / (slowDistance - stopDistance))
                            timePrint('Slowing down %i at distance: %f' %(slowdown, self.distance))

            self.drive.extraSpeed(slowdown)

            time.sleep(interval)

    def _modeWorker(self, interval=0.2):
        """ internal thread for handling PiCar's operation modes """
        oldMode = self.mode
        while True:
            if self.mode == picarConst.ManualMode:
                # stop auto mode when switching from auto to manual mode
                if oldMode != self.mode:
                    self._stopAuto()
            elif self.mode == picarConst.FollowDistanceMode:
                self._followByDistance(DistanceToFollow, MaxFollowDistance)
            elif self.mode == picarConst.FollowLineMode:
                self._followLine()
            elif self.mode == picarConst.FollowObjectMode:
                pass
            elif self.mode == picarConst.FaceTrackingMode:
                pass
            else:
                self._stopAuto()
            oldMode = self.mode
            time.sleep(interval)

    def _followByDistance(self, distance, maxDistance, distanceOffset=0.1):
        """ internal function for followDistance mode
        follow with Ultrasonic by keeping the same distance to target
        distance - keep the distance to the target
        maxDistance - the max distance 
        distanceOffset - controls the sensitivity
        """
        self.head.lookStraight()
        self.drive.turnStraight()
        dis = self.head.ultrasonic.pingDistance()
        if dis < maxDistance:             #Check if the target is in diatance range
            if dis > (distance + distanceOffset) :   #If the target is in distance range and out of distance stay, then move forward to track
                moving_time = PiCar._getMovingTime(dis - distance)
                print('followByDistance - forward: ' + str(moving_time))
                self.drive.setLedsRGB(picarConst.CYAN)
                self.drive.forward(90)
                time.sleep(moving_time)
                self.drive.stopMotor()
            elif dis < (distance - distanceOffset) : #Check if the target is too close, if so, the car move back to keep distance at distance
                moving_time = PiCar._getMovingTime(distance - dis)
                print('followByDistance - backward: ' + str(moving_time))
                self.drive.setLedsRGB(picarConst.PINK)
                self.drive.backward(90)
                time.sleep(moving_time)
                self.drive.stopMotor()
            else:                            #If the target is at distance, then the car stay still
                self.drive.stopMotor(turnOffLeds=True)
        else:
            self.drive.stopMotor()

    def _followLine(self):
        left, middle, right = self.lineTracking.status()
        if middle:
            self.drive.run(speed=90, steeringAngle=0)
            self.drive.setLedsRGB(picarConst.YELLOW)
        elif left:
            self.drive.forward(speed=90)
            self.drive.turnLeft(angle=45, turnSignal=True)
        elif right:
            self.drive.forward(speed=90)
            self.drive.turnRight(angle=45, turnSignal=True)
        else:
            self.drive.backward(speed=90)
            self.drive.setLedsRGB(picarConst.CYAN)

    def followObject(self):
        time.sleep(0.2)

    def _faceTracking(self):
        time.sleep(0.2)

    @staticmethod
    def _getMovingTime(distance):
        moving_time = distance / 0.38
        if moving_time > 1:
            moving_time = 1
        return moving_time



