# Overview
The RIOT service provides remote access/control to TP-Link Kasa HS1xx smart plug. It allows clients send http requests to get status and control the plug's on/off and led.
Smart plug is a useful and powerful tool for home automation. However, it's overall security and "call home" features are concerns to many. 
This python code is intended to isolate smart plugs in a private WIFI hotspot with a bridge web service to access those devices from outside the private WIFI hotspot.

# Files
* kasaHs1xxRequestHandler.py - Python class inherits from piServices.BaseRequestHandler to handles all the requests for smart plug
* hs1xxSmartPlug.py - Python class that wraps https://github.com/vrachieru/tplink-smartplug-api
* testserver.py - simple test server based on Flask

# Setup and test:
1. Setup Raspberry Pi with the followings. Skip this if you don't want use private hotspot (and make sure to change the IP adress in following steps).
   * Connect to internet via ethernet (static IP address: 192.168.0.33; main router IP range 192.168.0.*)
   * Use hostapd and dnsmasq to setup a private WIFI access point (with different IP range: 192.168.1.*).
2. Setup Kasa smart plug HS103 mini to connect to the private WIFI access point
3. Setup the python software modules. These steps assume that the code is under /home/pi/riot.
   1. download/clone the respository
   2. Install the Python packages with pip3
```
cd /home/pi/riot
sudo python3 pip3 install .
```
   3. install from https://github.com/vrachieru/tplink-smartplug-api. Alternatively, just download the code and copy the api.py to /home/pi/riot/pySmartPlugService and rename to tplink_smartplug.py
4. Find out the IP address of the smart plugs and change/add the smart plugs to the testserver.py 
```
    kasaHandler.addSmartPlug('plug01', '192.168.1.85')
```
5. Run testserver from the Raspberry Pi with port 3333: 
```
python3 testserver.py 3333
```
6. Send http requests to Raspberry Pi via the main router. From web browser type: http://192.168.0.33:3333/dev/plug01/system. Or, using httpie:
```
http 192.168.0.33:3333/dev/plug01/system
```

# Test commands using httpi from a Windows machine
```
http 192.168.0.33:3333/dev/plug01/system
http 192.168.0.33:3333/dev/plug01/system/alias
http 192.168.0.33:3333/dev/plug01/time
http 192.168.0.33:3333/dev/plug01/time/time
http 192.168.0.33:3333/dev/plug01/system relay_state=0 led_off=0
http "192.168.0.33:3333/dev/plug01/emeter?year=2021&month=1"
```
