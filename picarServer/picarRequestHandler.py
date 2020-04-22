#!/usr/bin/python3
# File name   : picarRequestHandler.py
# Description : PiCarRequestHandler is the request handler to process http requests to control PiCar

import traceback
from piServices.piConstants import *
from piServices.piUtils import timePrint, splitAndTrim
from piServices.flaskUtil import makeJsonResponse
from piServices.baseRequestHandler import BaseRequestHandler

class PiCarRequestHandler(BaseRequestHandler):
    """ PiCarRequestHandler is the request handler to process http requests to control PiCar """
    def __init__(self, commandHandler, name='PiCar', rootPaths=['picar'], authorizationList=None):
        """ constructor for PiHygroThermoRequestHandler
        commandHandler: the PiCarCommandHandler that execute commands
        name: the name for the handler
        rootPaths: list of root paths that will be handled by the instance
        authorizationList: a list of strings for authorized users
        """
        super(PiCarRequestHandler, self).__init__(name=name, rootPaths=rootPaths, authorizationList=authorizationList)
        self.commandHandler = commandHandler

    def doGet(self, request):
        """ handle GET request for PiCar
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        component = request.paths[1]
        httpStatusCode = 200
        try:
            if 'ultra' in component:
                value = self.commandHandler.car.distance
                response = { KeyPropertyKey: 'ultra', KeyPropertyValue: value }
            elif 'motor' in component:
                value = self.commandHandler.drive.motor.speed
                response = { KeyPropertyKey: 'motor', KeyPropertyValue: value }
            elif 'servo' in component:
                value = self.commandHandler.drive.steering.angle
                response = { KeyPropertyKey: 'servo', KeyPropertyValue: value }
            elif 'servocamh' in component:
                value = self.commandHandler.head.servoH.angle
                response = { KeyPropertyKey: 'servocamh', KeyPropertyValue: value }
            elif 'servocamv' in component:
                value = self.commandHandler.head.servoV.angle
                response = { KeyPropertyKey: 'servocamv', KeyPropertyValue: value }
            elif 'ledright' in component:
                value = self.commandHandler.drive.rightLed.rgb.toRGBStr()
                response = { KeyPropertyKey: 'ledright', KeyPropertyValue: value }
            elif 'ledleft' in component:
                value = self.commandHandler.drive.leftLed.rgb.toRGBStr()
                response = { KeyPropertyKey: 'ledleft', KeyPropertyValue: value }
            elif 'ledstrip' in component:
                httpStatusCode = 400
                response = { KeyPropertyValue: 'NotYetSupported' }
            else:
                httpStatusCode = 400
                response = { KeyResponse: 'InvalidComponent' }
        except Exception as e:
            print('Exception handling request: ' + str(e))
            traceback.print_exc()
            httpStatusCode = 500
            response = { KeyResponse: 'Exception: ' + str(e) }
        return makeJsonResponse(httpStatusCode, response)

    def doPost(self, request):
        """ handle POST request for Pi system resources
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        component = request.paths[1]
        bodyDict = request.json
        response = []
        httpStatusCode = 200
        try:
            timePrint('Post to picar: ' + str(bodyDict))
            if 'scan' == component:
                # for scan, the bodyDict contains strt end and inc for the scan
                response = self.commandHandler.doScanCommand(bodyDict)
                httpStatusCode = response.pop(KeyStatusCode, 400)
            else:
                # process all the values in the body
                httpStatusCode = 200
                comma = ''
                for key, value in bodyDict.items():
                    fullPath = request.path + '/' + key
                    itemResponse = self.commandHandler.doCommand(fullPath, value)
                    #timePrint(response)
                    statusCode = itemResponse.pop(KeyStatusCode, 400)
                    if statusCode != 200:
                        httpStatusCode = statusCode
                    response.append(itemResponse)

        except Exception as e:
            print('Exception handling request: ' + str(e))
            traceback.print_exc()
            httpStatusCode = 500
            response = { KeyResponse: 'Exception: ' + str(e) }
        return makeJsonResponse(httpStatusCode, response)



