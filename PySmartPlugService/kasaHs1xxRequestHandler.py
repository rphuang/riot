#!/usr/bin/python3
# File name   : kasaHs1xxRequestHandler.py
# Description : KasaHs1xxRequestHandler is the request handler for Kasa Hs1xx smart plugs connected via WIFI

from piServices.piConstants import *
from piServices.piUtils import timePrint
from piServices.flaskUtil import makeJsonResponse
from piServices.baseRequestHandler import BaseRequestHandler
from hs1xxSmartPlug import Hs1xxSmartPlug

# RIOT url: /<root>/<device>/<node>/<data>/<property>?<arg1=val1>&<arg2=val2> ...
#  root - the root or device path default to 'dev'
#  device - the device id registered via addSmartPlug()
#  node - the node id should be one of ['system', 'time', 'emeter', 'cmd']
#  data - the data id in the node
#  property - the name of the property
# get examples:
#  /dev/dev01/system              get the system data from smart plug dev01
#  /dev/dev01/time/timezone       get the timezone from smart plug dev01
#  /dev/dev02/emeter?year=2020    get 2020 monthly statistic data from smart plug dev02
# post examples:
#  /dev/dev01/system body: {"deviceId": "sm01", "relay_state":1}    set deviceId & turn on the plug
#  /dev/dev01/cmd/reboot  body: {"delay": 2}        reboot the smart plug dev01 after 2 second delay
class KasaHs1xxRequestHandler(BaseRequestHandler):
    """ KasaHs1xxRequestHandler is the request handler for Kasa HS1xx smart plugs """
    def __init__(self, name='KasaSmartPlug', rootPaths=['dev'], authorizationList=None):
        """ constructor for KasaHs1xxRequestHandler 
        . name: the name for the handler
        . rootPaths: list of root paths that will be handled by the instance
        . authorizationList: a list of strings for authorized users
        """
        super(KasaHs1xxRequestHandler, self).__init__(name=name, rootPaths=rootPaths, authorizationList=authorizationList)
        self.deviceIds = []
        self.deviceList = {}

    def addSmartPlug(self, id, ipAddress):
        """ add a smart plug to the handler 
        . str id: the id for the smart plug
        . str ipAddress: host name or ip address for the smart plug
        """
        self.deviceIds.append(id)
        self.deviceList[id] = Hs1xxSmartPlug(id, ipAddress)
    
    def doGet(self, request):
        """ handle GET request for smart plug devices
        . request: the RequestContext data object 
        must return a Flask.Response object
        """
        deviceId = request.paths[1]
        if len(deviceId) == 0:
            return makeJsonResponse(400, { KeyResponse: 'MissingSmartDeviceId' })

        httpStatusCode = 200
        device = self.deviceList[deviceId]
        if device == None:
            return makeJsonResponse(400, { KeyResponse: 'InvalidSmartDeviceId' })

        node = request.paths[2]
        dataId = request.paths[3]
        httpStatusCode, response = device.get(node, dataId, request.args)
        return makeJsonResponse(httpStatusCode, response)

    def doPost(self, request):
        """ handle POST request for smart plug devices
        . request: the RequestContext data object 
        must return a Flask.Response object
        """
        deviceId = request.paths[1]
        if len(deviceId) == 0:
            return makeJsonResponse(400, { KeyResponse: 'MissingSmartDeviceId' })

        device = self.deviceList[deviceId]
        if device == None:
            return makeJsonResponse(400, { KeyResponse: 'InvalidSmartDeviceId' })

        bodyJson = request.json
        response = []
        httpStatusCode = 200
        node = request.paths[2]    # node should be one of ['system', 'cmd']
        dataId = request.paths[3]
        httpStatusCode, response = device.post(node, dataId, request.json)
        return makeJsonResponse(httpStatusCode, response)


