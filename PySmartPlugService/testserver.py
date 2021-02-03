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
from .kasaHs1xxRequestHandler import KasaHs1xxRequestHandler

app = Flask(__name__)
app.request_class = RequestContext

# RIOT url: /<root>/<device>/<node>/<data>/<property>?<arg1=val1>&<arg2=val2> ...
#  root - the root or device path default to 'dev'
#  device - the device id registered via addSmartPlug()
#  node - the node id should be one of ['system', 'time', 'emeter', 'cmd']
#  data - the data id in the node
#  property - the name of the property
# Examples:
#  /dev/dev01/system              get the system data from smart plug dev01
#  /dev/dev01/time/timezone       get the timezone from smart plug dev01
#  /dev/dev02/emeter?year=2020    get 2020 monthly statistic data from smart plug dev02
# post examples:
#  /dev/dev01/system body: {"deviceId": "sm01", "relay_state":1}    set deviceId & turn on the plug
#  /dev/dev01/cmd/reboot  body: {"delay": 2}        reboot the smart plug dev01 after 2 second delay
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
    kasaHandler = KasaHs1xxRequestHandler(authorizationList=authorizationList)
    # add smart plug to kasaHandler
    kasaHandler.addSmartPlug('sp01', '192.168.3.85')

    try:
        app.run(host='0.0.0.0', port=port, debug = True, threaded=True, use_reloader=True)
    except KeyboardInterrupt:
        timePrint("PiServer stopped")


