#!/usr/bin/python3
# File name   : piGpioRequestHandler.py
# Description : PiGpioRequestHandler is the request handler for Pi GPIO

from piServices.piConstants import *
from piServices.piUtils import timePrint, splitAndTrim
from piServices import piGpio
from piServices.flaskUtil import makeJsonResponse
from piServices.baseRequestHandler import BaseRequestHandler

class PiGpioRequestHandler(BaseRequestHandler):
    """ PiGpioRequestHandler is the request handler for Pi GPIO """
    def __init__(self, name='PiGpio', rootPaths=['gpio'], authorizationList=None):
        """ constructor for PiGpioRequestHandler 
        name: the name for the handler
        rootPaths: list of root paths that will be handled by the instance
        authorizationList: a list of strings for authorized users
        """
        super(PiGpioRequestHandler, self).__init__(name, rootPaths=rootPaths, authorizationList=authorizationList)
        piGpio.setup()

    def doGet(self, request):
        """ handle GET request for Pi GPIO
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        component = request.paths[1]
        if len(component) == 0:
            httpStatusCode, response = piGpio.getAll()
        else:
            response = piGpio.get(int(component), request.paths[2])
            httpStatusCode = response.pop(KeyStatusCode, 400)
        return makeJsonResponse(httpStatusCode, response)

    def doPost(self, request):
        """ handle POST request for Pi GPIO
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        bodyJson = request.json
        # todo: handle json with list of pin objects
        response = []
        httpStatusCode = 200
        component = request.paths[1]
        for key, value in bodyJson.items():
            # case 1: url="/gpio" body="7/value": 0     // OK
            # case 2: url=/gpio/7 body="value": 0       // OK
            # case 3: url=/gpio/7 body="7/value": 0     // invalid for now
            # case 4: url=/gpio/7 body="8/value": 0     // invalid
            # prepend the component to form full path such as 7/value
            fullPath = component + '/' + key
            parts = splitAndTrim(fullPath, '/')
            try:
                pin = int(parts[0])
                intValue = int(value)
                property = ''
                if len(parts) > 1:
                    property = parts[1]
                pinResponse = piGpio.set(pin, property, intValue)
                pinStatusCode = pinResponse.pop(KeyStatusCode, 400)
            except:
                pinStatusCode = 400
                pinResponse = { KeyResponse: 'InvalidPinOrValue', KeyGpioPropertyPin: pin, KeyPropertyValue: value }
            if pinStatusCode != 200:
                httpStatusCode = pinStatusCode
            response.append(pinResponse)
        return makeJsonResponse(httpStatusCode, response)




