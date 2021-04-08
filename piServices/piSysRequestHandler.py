#!/usr/bin/python3
# File name   : piSysRequestHandler.py
# Description : PiSysRequestHandler is the request handler for Pi system resources

import os
import threading
import tempfile
from piServices.piConstants import *
from piServices.piUtils import timePrint, startThread
from piServices.piStats import PiStats
from piServices.flaskUtil import makeJsonResponse
from piServices.baseRequestHandler import BaseRequestHandler

class PiSysRequestHandler(BaseRequestHandler):
    """ PiSysRequestHandler is the request handler for Pi system resources """
    def __init__(self, name='PiSystem', rootPaths=['sys', 'system'], authorizationList=None, logFile=None):
        """ constructor for PiSysRequestHandler 
        name: the name for the handler
        rootPaths: list of root paths that will be handled by the instance
        authorizationList: a list of strings for authorized users
        logFile: file to log cpu stats
        """
        super(PiSysRequestHandler, self).__init__(name=name, type='PiSystem', rootPaths=rootPaths, authorizationList=authorizationList)
        self._startUp(logFile)

    def doGet(self, request):
        """ handle GET request for Pi system resources
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        component = request.paths[1]
        httpStatusCode = 200
        if len(component) == 0:
            response = self.pistats.get_all_info()
        elif KeyComponentCpu in component:
            response = self.pistats.get_cpu_info()
        elif KeyComponentMemmory in component:
            response = self.pistats.get_memory_info()
        elif KeyComponentStorage in component:
            httpStatusCode = 400
            response = { KeyResponse: 'NotYetSupported' }
        else:
            httpStatusCode = 400
            response = { KeyResponse: 'InvalidSysComponent' }
        return makeJsonResponse(httpStatusCode, response)

    def doPost(self, request):
        """ handle POST request for Pi system resources
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        bodyJson = request.json
        response = []
        httpStatusCode = 200
        for key, value in bodyJson.items():
            try:
                timePrint('POST system command: %s=%s' %(key, value))
                # use tempfile for output
                tempFile = tempfile.NamedTemporaryFile(prefix='pisys-')
                tempFileName = tempFile.name
                tempFile.close()
                command = '%s > %s 2>&1 \n' %(value, tempFileName)
                os.system(command)
                f = open(tempFileName, "r")
                text = f.read()
                f.close()
                os.remove(tempFileName)
                itemStatusCode = 200
                itemResponse = { key: value, 'result': text }
            except:
                itemStatusCode = 400
                itemResponse = { KeyResponse: 'InvalidCommand', KeyPropertyValue: value }
            if itemStatusCode != 200:
                httpStatusCode = itemStatusCode
            response.append(itemResponse)
        return makeJsonResponse(httpStatusCode, response)

    def _startUp(self, logFile=None):
        """ internal method to startup the handler:
        - create an instance of PiStats
        - create a thread to run PiStats's updateStatusForever() that updates CPU usage every 1.0 second
        """
        self.pistats = PiStats()
        startThread('PiStats.updateStatusForever', target=self.pistats.updateStatusForever, front=True, args=(1.0, logFile))



