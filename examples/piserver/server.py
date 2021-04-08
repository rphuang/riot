#!/usr/bin/python3
# File name   : server.py
# Description : PiServer module to provide HTTP service

import sys
from flask import Flask, request
from piServices.requestContext import RequestContext
from piServices.baseRequestHandler import BaseRequestHandler
from piServices.piUtils import timePrint
from piServices.piSysRequestHandler import PiSysRequestHandler
from piServices.piGpioRequestHandler import PiGpioRequestHandler
from piHygroThermoRequestHandler import PiHygroThermoRequestHandler

app = Flask(__name__)
app.request_class = RequestContext

# RIOT url: /<root>/<node>/<data>/<property>
#  root - the root or device path
#  node - the node id
#  data - the data id in the node
#  property - the name of the property
# Examples:
#  /sys/cpu         get the cpu data
#  /gpio            get all gpio pins' data
#  /gpio/38         get pin #38 data
@app.route('/', defaults={'root': '', 'path1': '', 'path2': '', 'path3': ''}, methods=['GET', 'POST', 'PUT'])
@app.route('/<root>', defaults={'path1': '', 'path2': '', 'path3': ''}, methods=['GET', 'POST', 'PUT'])
@app.route('/<root>/<path1>', defaults={'path2': '', 'path3': ''}, methods=['GET', 'POST', 'PUT'])
@app.route('/<root>/<path1>/<path2>', defaults={'path3': ''}, methods=['GET', 'POST', 'PUT'])
@app.route('/<root>/<path1>/<path2>/<path3>', methods=['GET', 'POST', 'PUT'])
def router(root, path1, path2, path3):
    request.setPaths([root, path1, path2, path3])
    return BaseRequestHandler.processRequest(request)

if __name__ == '__main__':
    port = 8008
    if len(sys.argv) > 1:
        port = int(sys.argv[1])

    authorizationList = None
    # create proper RequestHandlers
    sysHandler = PiSysRequestHandler(authorizationList=authorizationList)
    gpioHandler = PiGpioRequestHandler(authorizationList=authorizationList)
    dhtHandler = PiHygroThermoRequestHandler(pin=11, authorizationList=None)

    try:
        app.run(host='0.0.0.0', port=port, debug = True, threaded=True, use_reloader=False)
    except KeyboardInterrupt:
        timePrint("PiServer stopped")

