#!/usr/bin/python3
# File name   : piHygroThermoRequestHandler.py
# Description : PiHygroThermoRequestHandler is the request handler for getting temperature and humidity from sensor on Pi

from piServices.piConstants import *
from piServices.piUtils import timePrint, splitAndTrim
from piServices.flaskUtil import makeJsonResponse
from piServices.baseRequestHandler import BaseRequestHandler
from piHygroThermoDHT import PiHygroThermoDHT

TemperatureKey = 'temperature'
HumidityKey = 'humidity'

class PiHygroThermoRequestHandler(BaseRequestHandler):
    """ PiHygroThermoRequestHandler is the request handler for getting temperature and humidity from sensor on Pi """
    def __init__(self, pin, name='HygroThermoSensor', rootPaths=['dht'], authorizationList=None):
        """ constructor for PiHygroThermoRequestHandler
        pin: the signal pin to the DHT sensor
        name: the name for the handler
        rootPaths: list of root paths that will be handled by the instance
        authorizationList: a list of strings for authorized users
        """
        super(PiHygroThermoRequestHandler, self).__init__(name=name, type='HygroThermoSensor', rootPaths=rootPaths, authorizationList=authorizationList)
        self.sensor = PiHygroThermoDHT('dht', None, pin)

    def doGet(self, request):
        """ handle GET request for temperature/humidity value
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        component = request.paths[1]
        httpStatusCode = 200
        temperature, humidity = self.sensor.read()
        if len(component) == 0:
            response = {TemperatureKey: temperature, HumidityKey: humidity}
        elif 'temp' in component:
            response = {TemperatureKey: temperature}
        elif 'hum' in component:
            response = {HumidityKey: humidity}
        else:
            httpStatusCode = 400
            response = { KeyResponse: 'InvalidPath' }
        return makeJsonResponse(httpStatusCode, response)



