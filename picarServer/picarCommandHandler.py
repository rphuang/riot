#!/usr/bin/python3
# File name   : picarCommandHandler.py
# Description : handles commands for PiCar

import time
import traceback
import requests
import json
import threading

from piServices.piUtils import timePrint
import picar
import picarDrive
import picarHead
import picarConst

# constants for http status strings
servoStatusMessage = ['Go Straight', 'Go Left', 'Go Right']
cameraHorizontalStatusMessage = ['Look Straight', 'Look Left', 'Look Right']
cameraVerticalStatusMessage = ['Look Straight', 'Look Down', 'Look Up']

class PiCarCommandHandler(object):
    """ handles commands for PiCar """
    def __init__(self, car):
        """ constructor with a PiCar object as command target """
        self.car = car
        self.drive = car.drive
        self.head = car.head
        self.strip = car.strip
        self.leftLed = car.drive.leftLed
        self.rightLed = car.drive.rightLed
        self.config = car.config
        self.commandToSpeechErrorCount = 0
        self.speechList = []
        self.commandToSpeechAll = False
        rawSpeechList = self.config.get('piCar.commandToSpeechList')
        if len(rawSpeechList) > 0:
            rawSpeechListLower = rawSpeechList.lower()
            if rawSpeechListLower.startswith('all'):
                self.commandToSpeechAll = True
            else:
                self.speechList = rawSpeechListLower.split(',')

    def doCommand(self, path, valueStr):
        """ execute the command specified by the path and value
        returns a dictionary of command result

        valid paths:
        - motor: set motor speed
        - servo: set steering servo position
        - servocamh: camera horizontal servo (left, right)
        - servocamv: camera vertical servo (up and down)
        - ledright: right LED with red, green, blue colors
        - ledleft: left LED with red, green, blue colors
        - ledstrip: the strip led lights with full RGB colors and lightshows
        """
        pathLowerCase = path.lower()
        commandStatus = 'Bad Request'
        httpStatusCode = 400
        #print(' value: ' + valueStr)
        if valueStr != None:
            try:
                # process based on path
                httpStatusCode = 200
                if 'motor' in pathLowerCase:
                    # motor speed range: -100 - 0 - +100 (in %)
                    value = int(valueStr)
                    if value == 0:
                        self.drive.stopMotor(turnOffLeds=True)
                        commandStatus = 'Stop'
                    elif value < 0:
                        self.drive.backward(-value)
                        commandStatus = 'Backward'
                    else:
                        self.drive.forward(value)
                        commandStatus = 'Forward'
                elif 'servocamh' in pathLowerCase:
                    # servo angle range (in degree): -90 (max left) - 0 (straight) - +90 (max right)
                    value = self.head.moveHorizontal(int(valueStr))
                    commandStatus = cameraHorizontalStatusMessage[getAngleStatus(value)]
                elif 'servocamv' in pathLowerCase:
                    # servo angle range (in degree): -90 (max down) - 0 (straight) - +90 (max up)
                    value = self.head.moveVertical(int(valueStr))
                    commandStatus = cameraVerticalStatusMessage[getAngleStatus(value)]
                elif 'servo' in pathLowerCase:
                    # servo angle range (in degree): -90 (max left) - 0 (straight) - +90 (max right)
                    # note that the turn_* functions have more restrictions for turning angles
                    value = self.drive.turnSteering(int(valueStr))
                    commandStatus = servoStatusMessage[getAngleStatus(value)]
                elif 'ledstrip' in pathLowerCase:
                    # stripled supports following values
                    # - off - stop lightshow
                    # - cycle - rainbowCycle lightshow
                    # - chase - theaterChaseRainbow lightshow
                    # - auto - auto mode
                    # - color value in the format of "rrr, ggg, bbb" separated by comma
                    try:
                        self.car.setStrip(valueStr)
                        commandStatus = 'Set Led Strip'
                    except:
                        print('Invalid LedStrip value: ' + valueStr)
                        commandStatus = 'Invalid Value'
                        httpStatusCode = 400
                elif 'led' in pathLowerCase:
                    # for each color: 0 is off non-0 is on
                    if 'right' in pathLowerCase:
                        self.rightLed.setRGBStr(valueStr)
                        commandStatus = 'Right Led'
                    elif 'left' in pathLowerCase:
                        self.leftLed.setRGBStr(valueStr)
                        commandStatus = 'Left Led'
                    else:
                        commandStatus = 'Unknown Device'
                elif 'scan' in pathLowerCase:
                    if len(valueStr) == 0:
                        # case: no parameter (empty valueStr) for scan the current distance is returned
                        distance = self.car.distance
                        commandStatus = 'Get Distance'
                    else:
                        # case: valueStr contains the json body
                        bodyDict = json.loads(valueStr)
                        commandStatus = 'Scan Distance'
                        fullResponse = self.doScanCommand(bodyDict)
                        return fullResponse
                elif 'mode' in pathLowerCase:
                    # setting mode
                    self.doSetModeCommand(valueStr)
                    commandStatus = 'Set Mode to %s' %valueStr
                else:
                    commandStatus = 'Unknown Device Component'
                    httpStatusCode = 400
            except Exception as e:
                # todo: classify different exceptions and http status codes
                print('Exception handling request: ' + str(e))
                traceback.print_exc()
                commandStatus = 'Exception'
                httpStatusCode = 400

        response = {'statusCode': httpStatusCode, 'response': commandStatus, 'device': pathLowerCase, 'value': valueStr}
        self.commandToSpeech(commandStatus)
            
        return response

    def doScanCommand(self, bodyDict):
        """ execute the scan command specified by the bodyDict
        bodyDict is a dict for the request body (the json payload)
        returns a dictionary of scan result
        - statusCode
        - value: an array of distance values
        - posh: an array of horizontal position
        - posv: an array of vrtical position
        """
        self.commandToSpeech('Start Scan')
        value = []
        posh = []
        posv = []
        starth = int(getFromDict(bodyDict, 'starth', -90))
        startv = int(getFromDict(bodyDict, 'startv', 0))
        endh = int(getFromDict(bodyDict, 'endh', 90))
        endv = int(getFromDict(bodyDict, 'endv', 0))
        inch = int(getFromDict(bodyDict, 'inch', 2))
        incv = int(getFromDict(bodyDict, 'incv', 2))
        value, posh, posv = self.head.scan(starth, startv, endh, endv, inch, incv)
        timePrint('scan completed with ' + str(len(value)) + ' values')
        response = {'statusCode': 200, 'response': 'scanResults', 'value': value, 'posh': posh, 'posv': posv}
        return response

    def doSetModeCommand(self, valueStr):
        """ command to set mode specified in valueStr
        available modes are: manual, follow, findline, speech, opencv
        returns a dictionary of scan result
        - statusCode
        - response
        """
        valueStrLower = valueStr.lower()
        if 'manual' in valueStrLower:
            self.car.setOperationMode(picarConst.ManualMode)
        elif 'follow' in valueStrLower:
            self.car.setOperationMode(picarConst.FollowDistanceMode)
        elif 'findline' in valueStrLower:
            self.car.setOperationMode(picarConst.FollowLineMode)
        elif 'opencv' in valueStrLower:
            self.car.setOperationMode(picarConst.FollowObjectMode)
        elif 'speech' in valueStrLower:
            self.car.setOperationMode(picarConst.ManualMode)
        else:
            return {'statusCode': 400, 'response': 'InvalidMode', 'value': valueStr}
        return {'statusCode': 200, 'response': 'SetMode', 'value': valueStr}

    def commandToSpeech(self, commandStatus):
        """ spin a thread to send commandStatus to CommandToSpeech service """
        thread = threading.Thread(target=self._commandToSpeechAsync, args=(commandStatus, ))
        thread.setDaemon(True)                            # 'True' for a front thread and would close when the mainloop() closes
        thread.start()

    def _commandToSpeechAsync(self, commandStatus):
        """ send commandStatus to CommandToSpeech service """
        speechMode = self.config.getOrAdd('piCar.commandToSpeechMode', 'off')
        if 'on' in speechMode.lower() and self.commandToSpeechErrorCount < 3:
            if self.commandToSpeechAll or commandStatus.lower() in self.speechList:
                speakService =  self.config.get('piCar.commandToSpeechService')
                credential =  self.config.get('piCar.commandToSpeechCredential')
                auth = None
                if len(credential) > 0:
                    user, password = credential.split(':')
                    auth=(user, password)
                try:
                    r = requests.post(speakService, data = json.dumps({'text': commandStatus}), auth=auth)
                    timePrint('Send command to speech %s' %commandStatus)
                    self.commandToSpeechErrorCount = 0
                except Exception as e:
                    print('Exception command to speech %s: %s' %(commandStatus, str(e)))
                    self.commandToSpeechErrorCount += 1
        else:
            # clear error count if commandToSpeechMode is off
            self.commandToSpeechErrorCount = 0

def getFromDict(dict, key, default):
    """ get value by key
    returns the value in dict or default value if no key in dict
    """
    if key in dict:
        return dict[key]
    else:
        return default

def getAngleStatus(angle):
    """ returns status based on the angle. Status codes:
    0 - straight (-10 to +10)
    1 - left or down (less than -10)
    2 - right or up (greater than 10)
    """
    if angle < -10:
        return 1
    elif angle > 10:
        return 2
    return 0



