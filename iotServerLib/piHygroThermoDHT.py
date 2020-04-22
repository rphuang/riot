#!/usr/bin/python3
# File name   : piHygroThermoDHT.py
# Description : encapsulate the DHT sensor (humidity & temperature) in Raspberry Pi
# dependency  : Freenove_DHT.py from github 
#               https://github.com/Freenove/Freenove_Ultimate_Starter_Kit_for_Raspberry_Pi/tree/master/Code/Python_Code/21.1.1_DHT11

import time
import threading
from Freenove_DHT import DHT
from iotServerLib import piIotNode
from piServices.piUtils import timePrint

class PiHygroThermoDHT(piIotNode.PiIotNode):
    """ encapsulate the DHT sensor (humidity & temperature) in Raspberry Pi """
    def __init__(self, name, parent, pin):
        """ construct a PiHygroThermoDHT 
        name: the name of the node
        parent: parent IotNode object. None for root node.
        pin: the signal pin to the DHT sensor
        """
        super(PiHygroThermoDHT, self).__init__(name, parent)
        self.pin = pin
        self.dht = DHT(pin)
        self.humidity = DHT.DHTLIB_INVALID_VALUE
        self.temperature = DHT.DHTLIB_INVALID_VALUE

    def read(self):
        """ read the current temerature & humidity from sensor and returns [temerature, humidity] """
        timePrint('Start reading DHT')
        self.dht.readDHT11()
        timePrint('DHT sensor values temperature: %.2f humidity: %.2f' %(self.dht.temperature, self.dht.humidity))
        return [self.dht.temperature, self.dht.humidity]

    def startUp(self, interval=1):
        """ start thread to read sensor every interval second the result is stored in instance variable humidity & temerature """
        timePrint('Starting DHT sensor thread ')
        self.thread = threading.Thread(target=self._readWorker, args=(interval, ))
        self.thread.setDaemon(front)                            # 'True' for a front thread and would close when the mainloop() closes
        self.thread.start()

    def _readWorker(self, interval):
        """ worker to read DHT sensor """
        while True:
            status = self.dht.readDHT11()
            if status is 0:
                self.humidity = self.dht.humidity
                self.temperature = self.dht.temperature
            time.sleep(interval)





