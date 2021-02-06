#!/usr/bin/python3
# File name   : hs1xxSmartPlug.py
# Description : Hs1xxSmartPlug is a wrapper for Kasa HS1xx smart plug

from piServices.piConstants import *
from piServices.piUtils import timePrint
from tplink_smartplug import SmartPlug

class Hs1xxSmartPlug():
    """ Implements direct interface to Kasa HS1xx SmartPlug 
    the class is depended on the SmartPlug API by https://github.com/vrachieru/tplink-smartplug-api
    """
    def __init__(self, id, host, port=9999, timeout=5):
        """ constructor
        . str id: the id for the device
        . str host: host name or ip address on which the device listens
        . int port: port on which the device listens (default: 9999)
        . int timeout: socket timeout (default: 5)
        """
        self.id = id
        self.smartPlug = SmartPlug(host, port, timeout)

    def get(self, node, dataId, args):
        """ get data from the smart plug
        . str node: one of ['system', 'time', 'emeter']
        . str dataId: the data ID of the node
        . dict args: key-value for get emeter
        returns a tuple (statusCode, dict) where the dict contains response or data items
        """
        nodeLowercase = node.lower()
        if len(nodeLowercase) == 0 or nodeLowercase == 'system':
            sysinfo = self.smartPlug.command(('system', 'get_sysinfo'))
            if len(dataId) == 0:
                return (200, sysinfo)
            if dataId.lower() in sysinfo:
                return (200, {dataId: sysinfo[dataId]})
            else:
                return (400, { KeyResponse: 'InvalidDataId' })

        elif nodeLowercase == 'time':
            if len(dataId) == 0:
                return (200, {'time': self.smartPlug.time, 'timezone': self.smartPlug.timezone})
            if dataId.lower() == 'time':
                return (200, {'time': self.smartPlug.time})
            if dataId.lower() == 'timezone':
                return (200, {'timezone': self.smartPlug.timezone})
            else:
                return (400, { KeyResponse: 'InvalidDataId' })

        elif nodeLowercase == 'emeter':
            if 'month' in args and 'year' in args:
                return (200, {'daystat': self.smartPlug.command(('emeter', 'get_daystat', {'month': args['month'], 'year': args['year']}))})
            elif 'year' in args:
                return (200, {'monthstat': self.smartPlug.command(('emeter', 'get_monthstat', {'year': args['year']}))})
            else:
                return (200, {'realtime': self.smartPlug.command(('emeter', 'get_realtime'))})
        else:
            return (400, { KeyResponse: 'InvalidNodeId' })

    def post(self, node, dataId, args):
        """ post data or cmd to the smart plug
        . str node: one of ['system', 'cmd']
        . str dataId: the data ID of the node
        . dict args: key-value to update data property
        returns a tuple (statusCode, dict) where the dict contains response or data items
        """

        # post is only valid for node in ['system', 'cmd']
        # valid post to 'cmd': reboot, reset
        # valid post to 'system' with dataId: ['deviceId', 'hwId', 'mac', 'alias', 'led_off', 'relay_state', 'latitude_i', 'longitude_i']
        statusCode = 200
        response = {}
        nodeLowercase = node.lower()
        if nodeLowercase == 'system':
            errorCount = 0
            validDataIds = {'deviceId': 'set_device_id', 'hwId': 'set_hw_id', 'mac': 'set_mac_addr', 'alias': 'set_dev_alias'}
            #validDataIds2 = {'led_off': 'set_led_off', 'relay_state': 'set_relay_state'}
            for key, value in args.items():
                if key in validDataIds:
                    cmd = validDataIds[key]
                    self.smartPlug.command(('system', cmd, {key: value}))
                    timePrint('%s %s: %s' %(self.id, cmd, str(value)))
                    response[key] = value
                elif key == 'led_off':
                    self.smartPlug.command(('system', 'set_led_off', {'off': int(value)}))
                    timePrint('%s set_led_off: %s' %(self.id, str(value)))
                    response[key] = int(value)
                elif key == 'relay_state':
                    self.smartPlug.command(('system', 'set_relay_state', {'state': int(value)}))
                    timePrint('%s set_relay_state: %s' %(self.id, str(value)))
                    response[key] = int(value)
                else:
                    errorCount =+ 1
            if errorCount > 0:
                if len(response) > 0:
                    statusCode = 206
                else:
                    statusCode = 400
                    response[KeyResponse] = 'NoValidDataId'

        elif nodeLowercase == 'cmd':
            delay = 1
            if 'delay' in args:
                delay = args['delay']
            if dataId == 'reset':
                self.smartPlug.command(('system', 'reset', {'delay': int(delay)}))
                timePrint('%s reset: %s' %(self.id, str(delay)))
            elif dataId == 'reboot':
                self.smartPlug.command(('system', 'reboot', {'delay': int(delay)}))
                timePrint('%s reboot: %s' %(self.id, str(delay)))
            else:
                statusCode = 400
                response[KeyResponse] = 'InvalidCommand'
        else:
            statusCode = 400
            response[KeyResponse] = 'InvalidNodeId'

        return (statusCode, response)

