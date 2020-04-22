#!/usr/bin/python3
# File name   : server.py
# Description : the server for an Adeept Robotic PiCar

import sys
import time
import threading
from flask import Flask, request

from piServices.requestContext import RequestContext
from piServices.baseRequestHandler import BaseRequestHandler
from piServices.piUtils import timePrint
from piServices.piSysRequestHandler import PiSysRequestHandler
from piServices.piGpioRequestHandler import PiGpioRequestHandler

import picar
import picarCommandHandler
import picarTcpService
import config
from picarRequestHandler import PiCarRequestHandler

app = Flask(__name__)
app.request_class = RequestContext

@app.route('/', defaults={'root': '', 'path1': '', 'path2': '', 'path3': ''}, methods=['GET', 'POST', 'PUT'])
@app.route('/<root>', defaults={'path1': '', 'path2': '', 'path3': ''}, methods=['GET', 'POST', 'PUT'])
@app.route('/<root>/<path1>', defaults={'path2': '', 'path3': ''}, methods=['GET', 'POST', 'PUT'])
@app.route('/<root>/<path1>/<path2>', defaults={'path3': ''}, methods=['GET', 'POST', 'PUT'])
@app.route('/<root>/<path1>/<path2>/<path3>', methods=['GET', 'POST', 'PUT'])
def router(root, path1, path2, path3):
    request.setPaths([root, path1, path2, path3])
    return BaseRequestHandler.processRequest(request)

if __name__ == '__main__':
    defaultHttpPort = 8688
    configFile='picarconfig.txt'
    if len(sys.argv) > 1:
        configFile = sys.argv[1]

    try:
        # load config file and disable autoSave (to avoid save multiple times)
        picarConfig = config.Config(configFile, autoSave=False)

        httpPort = picarConfig.getOrAddInt('httpserver.httpPort', defaultHttpPort)

        car = picar.PiCar('Erikar', None, picarConfig)
        car.startUp()

        commandHandler = picarCommandHandler.PiCarCommandHandler(car)

        # start tcp service thread
        ap_threading=threading.Thread(target=picarTcpService.run, args=(commandHandler, picarConfig))
        ap_threading.setDaemon(True)                          #'True' means it is a front thread,it would close when the mainloop() closes
        ap_threading.start()

        # save any changes/updates and enable autoSave
        picarConfig.autoSave = True
        picarConfig.save()

        authorizationList = picarConfig.getOrAddInt('PiCar.AuthorizationList', '')
        # create proper RequestHandlers
        sysHandler = PiSysRequestHandler(authorizationList=authorizationList)
        gpioHandler = PiGpioRequestHandler(authorizationList=authorizationList)
        picarHandler = PiCarRequestHandler(commandHandler, authorizationList=authorizationList)

        app.run(host='0.0.0.0', port=httpPort, debug=True, threaded=True, use_reloader=False)

    except KeyboardInterrupt:
        car.shutDown()
