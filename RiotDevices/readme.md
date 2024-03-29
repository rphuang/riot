# RiotDevices Overview
The RiotDevices is a simple Android app to monitor and control devices via web service using RIOT HTTP protocol.
The Android App is built with the Riot libraries and Xamarin Forms.
It should be relative easy to make it work on iOS but had not tried on any IOS device!!
Features:
* Monitor the phone's sensors and device info
* Start a web service on the phone to provide senor data remotely and some remote actions such as text to speech and vibrate
* Monitor Raspberry Pi's CPU amd memory status. 
* Send system commands, like reboot and shutdown, it can also posted custom commands to the Pi.
* View Kasa smart plugs (model HS1xx) status and control on/off of the switch, reboot the plug, and set led-off status.
* Control robotic PiCar (Adeept Mars PiCar)
* Video streaming via HTTP.

# Setup
We must setup both server and client.
## Setup the server on a Raspberry Pi.
1. download/clone the respository. These steps assume that the code is under /home/pi/riot.
2. Install the Python packages with pip3. This will install the Python packages required for PiCar Server.
```
cd /home/pi/riot
sudo python3 pip3 install .
```
3. Configure and run PiCar server. Please refer the the Configuration section in picarServer/readme.md for details. Command to run picar server
```
python3 /home/pi/riot/picarServer/server.py /home/pi/riot/picarServer/picarconfig.txt
```
4. Install and run smart plug RIOT service (see readme.md in the folder for more)
    1. install from https://github.com/vrachieru/tplink-smartplug-api. Alternatively, just download the code and copy the api.py to /home/pi/riot/pySmartPlugService and rename to tplink_smartplug.py.
    2. run smart plug service using one of these approaches
        * run as stand alone service - refer to readme.md under pySmartPlugService
        * run as part of piCarServer by adding the 2 lines to /home/pi/riot/picarServer/server.py as following
```
        picarHandler = PiCarRequestHandler(commandHandler, name='PiCar', authorizationList=authorizationList)
        kasaHandler = KasaHs1xxRequestHandler(name='PiServerSmartPlugs', authorizationList=authorizationList)
        kasaHandler.addSmartPlug('plug01', '192.168.1.85')  # replace 192.168.1.85 with the smart plug's IP address'
```

## Setup Android app
1. Build and deploy to Android phone with VS
2. Grant storage permission for the Devices.Android app
3. Re-launch the Devices app
4. Set the Pi Server IP address and password.
    1. goto "Pi Server". There would be an error message after timeout.
    2. click "Edit" then update the server Address (address:port) and Credential (user:password) fields
    3. click Save. This would create a devicessettings.txt file under docs folder.
    4. close & restart the app
5. Setup the Pi Car server address and password following the same steps as above except by go to "Pi Car".
6. Configure the PiCar settings through the "Settings"
    * Motor Speed: the speed of the motor (0 to 100)
    * Steering Angle: the angle for left/right steering servo (0 to 90)
    * Delta Camera Angle: the delta angle to increment when holding the head's right/left/top/down buttons
    * Config distance scan with start/end angle position and angle increment
7. Config smart plug by following the same steps as 4 by goto Smart Plug 1.
8. Alternatively, edit the devicessettings.txt file directly to change server IP, credential, and adding new servers new smart plugs. Start by copying the file from the RiotDevices folder to the Android device's docs folder.
9. Change the commands by adding/updating the groups with Type = PiCommand.
10. Configure phone web service by updateing the following settings
    * whether to enable the phone web service: EnableWebService=true
    * the port number for the web service: ServerPrefix=http://*:5678
    * the root path for service: ServiceRootPath=
    * the command root path for the service: ServiceActionRootPath=/cmd
    * list of user:password: ServiceCredentials= user1:password1, user2:password2

# Code and Design
* The app uses Xamarin Form's app shell
* Dependencies
    * EntityLib - simple PropertyBag and base Entity class
    * SettingsLib - simple text based settings stored in local storage
    * PlatformLib - encapsulates the platform specifics, for now, only storage
    * FormsLib - utilities to display on Xamarin Forms
    * Riot - base interfaces and client classes for RIOT
    * Riot.Pi - data and clients for Pi's system and GPIO
    * Riot.IoDevice - data and clients for IO sensors/devices
    * Riot.Phone - data, services, server host for phone sensors
    * Riot.SmartPlug - data and clients for TPLink's Kasa HS1xx smart plug
* Supports Android only. Had not tried on any IOS devices!!
* Design
    * The flyout items in app's shell are created dynamically from devicessettings.txt file - based on the Type in Group. A sample settings file is included.
      Each group is represented by an instance of Topic (or derived classes). The Topics.TopicItems contains all topics. The supported topic types are: 
        * PiSystem - Topic - the system info for Pi
        * KasaHS1xx - DeviceTopic - TPLink Kasa HS1xx smart plug
        * PiCar - ControlTopic - robotic PiCar (Adeept Mars PiCar)
    * DiscoverService provides simple way to create client nodes that discovered from the server using RIOT.
      A static dictionary of instances of DiscoverService based on the server address:port is also provided to reduce requests.
    * PiCar & PiCarClientFactory provide the client and factory to encapsulate the PiCar. All controls are going through PiCar instance based on RIOT.
    * The view pages are:
        * PiStatsPage - display cpu and memory information of a Pi. Simple system command can also be sent.
        * Hs1xxPage - display time and system information from Kasa HS1xx smart plug. Plus turn the switch on/off, reboot, and set LED-OFF to true/false.
        * PiCarPage - simple UI to control PiCar
        * GpioPage - display GPIO data. Custom pin names are defined in the settings file.
        * SettingsPage - display and change settings

