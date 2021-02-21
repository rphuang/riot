# Overview
RIOT (REST IOT) is intended to simplify and componentize the code to build robotic kits around Raspberry Pi for both server (backend) and clients. 
It supports the Adeept Mars PiCar. However, it is designed with flexibility and extensibility and is relatively easy to extend to other kits and for general purpose IOT.

The followings are in the repository.
* piServices package - an HTTP service with simple REST IOT protocol for client-server communication. It provides basic gpio, cpu, and memory data via HTTP. Extensibility example can be found in the picarServer.
* iotServerLib package - a set of classes that encapsulates sensor/drive compoennts (for now mostly limited to what in Adeept Mars PiCar). Usage example can be found in the picarServer.
* pySmartPlugService module - a RIOT service that provide remote access/control to TP-Link Kasa HS1xx smart plug.
* videoStreaming package - an HTTP video streming service based on Raspberry Pi camera. Usage example can be found in the picarServer.
* picarServer module - this is the server for Adeept Mars PiCar. Besides the otiginal TCP support it added HTTP support. It is (mostly) backward compatible with existing TCP client.
* Client IOT lib (.NET C#) - a set of classes that encapsulates sensor/device compoennts on the server via HTTP. For now, mostly limited to what in Adeept Mars PiCar.
* Simple Android app (Xamarin based) - to control Adeept Mars PiCar via HTTP 
* examples - contains simple example code for using the packages

# Getting Started
1. download/clone the respository
2. Install the Python packages with pip3
```
    cd path-to-installed-folder
    sudo python3 pip3 install .
```
3. Install and run PiCar server
    1. create folders data & templates under path-to-installed-folder/picarServer
    2. copy files from path-to-installed-folder/videoStreaming/data to path-to-installed-folder/picarServer/data
    3. copy files from path-to-installed-folder/videoStreaming/templates to path-to-installed-folder/picarServer/templates
    4. adjust config file path-to-installed-folder/picarServer/picarconfig.txt
    5. run picar server
```
        python3 path-to-installed-folder/picarServer/server.py path-to-installed-folder/picarServer/picarconfig.txt
```
4. Install smart plug RIOT service (see readme.md in the folder for more)
    1. copy this folder pySmartPlugService to the Raspberry Pi
    2. install from https://github.com/vrachieru/tplink-smartplug-api. Alternatively, just download the code and copy the api.py to above folder and rename to tplink_smartplug.py
    
5. Setup RiotDevices Android app
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
    7. Config smart plug by following the same steps as 4.

# Coming 
* ServiceStack based HTTP server for RIOT that supports connected Arduino devices with REST-ish non-HTTP communication.

# Details

## RIOT Protocol
RIOT (REST for IOT) uses a simple HTTP REST protocol for client-server communication. For current version, the server always response with JSON.

### HTTP Url
The url path is the unique identification to the target.

    <scheme>://<host>/<root>/<device>/<node>/<data>[?parameters]
    
* Scheme: http (will support https in the future)
* Host: the server’s IP address or name and corresponding port specification
* Root: the root path. In the Python RequestHandler, the root path is used to determine the instance of handler.
* Device: the device ID or the path to the device if it is different than the root. In general, the /<root>/<device> determines where the device is.
* [Optional] node: the node ID or the path to the node separated with ‘/’ if it contains multiple layers.
* [Optional] data: the data to get/post. the data is depended on the service.

### HTTP Methods
* Get – to retrieve information/data from the target server device. In general this does not alter the state of target server device.
* Post and Put – to set value or send command to a target server device.

### HTTP Body
The HTTP body is for PUT and POST only. It contains the value or content that is sent to the server target.

## piServices - Python package
The Python piServices package implements the base classes to support RIOT HTTP protocol plus basic services for Raspberry Pi. It has dependency on Flask.
* BaseRequestHandler - the base class for handling HTTP request. The class supports custom handler by implementing a handler derived from this base class. All instances of derived classes will be automatically registered (via __init__) with root paths (the device in the protocol) to handle. 
    * doGet/doPut/doPost/doDelete - Derived classes implement these to handle corresponding request. By default, doPut calls doPost so they behave the same.
    * checkAuth - check authorization. Override to implement custom authorization.
    * processRequest - a class method that should be called to handle http requests. It finds the handler instances corresponding to the request's root path (device) from registered instances. The instance will be invoked to handle the requests for the root path (device). 
* PiGpioRequestHandler - this class is derived from BaseRequestHandler to handle requests for GET and POST from/to Raspberry Pi GPIO.
* PiSysRequestHandler - this class is derived from BaseRequestHandler to handle requests for GET information from Raspberry Pi system (cpu, memory). The POST allows executing commands on the Pi.

## pySmartPlugService - Python module
This is a RIOT service that provide remote access/control to TP-Link Kasa HS1xx smart plug. It allows clients send http requests to get status and control the plug's on/off and led. See readme.md in the folder for more. 

## iotServerLib - .NET class library
iotServerLib implements a set of class to encapsulate hardware components connected with Raspberry Pi's GPIO such as motor, servo, and others. It provides reusable building blocks and hide the details of the hardware. It is also intended to establish a standard interface that can encapsulate different hardware components/drivers with the same or similar functions.
* IotNode - the base class for all IOT nodes (as in a composite pattern)
* PiIotNode - the base class for all IOT nodes in Raspberry Pi
* PiDcMotor - encapsulates a DC motor that connected via controller similar to L293D
* PiServoI2c - encapsulates a servo connected via Adafruit_PCA9685
* PiRGBLed - encapsulates an RGB LED connected through GPIO
* PiUltrasonic - encapsulates an Ultrasonic sensor connected through GPIO
* PiStripLed - encapsulates the RGB led strip using ws281x
* PiLineTracking - encapsulates a line tracking sensor connected through GPIO
* PiHygroThermoDHT - encapsulate the DHT sensor for humidity & temperature (dependency: Freenove_DHT.py) 

## videoStreaming - Python package
This package supports an HTTP video streming service based on Flask. The main code is from https://blog.miguelgrinberg.com/post/video-streaming-with-flask with addition of face tracking.
* FaceTracker - detect and track faces with input image frames

videoStreaming works in either Windows and Pi:
* Windows: just run python videoStreamingService.py
* Pi: just run python3 videoStreamingService-pi.py

## picarServer - - Python app
This is the server for Adeept Mars PiCar. By leveraging the above Python packages, it supports both HTTP RIOT protocol (based on Flask) and the original Adeept TCP implementation. It also added face tracking for the video streaming.
* PiCarRequestHandler - derived from piServices.BaseRequestHandler to handle requests for the PiCar.
* PiCar - the overall control for the Adeept Mars PiCar that contains child nodes:
    * PiCarDrive - controls the "drive train" for the picar - DC motor, drive direction servo, and both left/right RGB LEDs
    * PiCarHead - controls the "head" for the picar - ultrasonic sensor, camera, and both horizontal/vertical servos
    * line tracker using iotServerLib.PiLineTracking
    * strip LED using iotServerLib.PiStripLed
* PiCarCamera - encapsulates the PI camera and video streaming with face tracking
* PiCarCommandHandler - component to handle all the commands for both PiCarRequestHandler (HTTP) and picarTcpService (TCP)
* picarTcpService - the original TCP server
* Config - simple csv based configuration 
* server - the server module that loads configuration and starts all necessary threads for the picar

## Rito and Riot.* - .NET class libraries for Riot
This is a set of C# libraries that can be used to build server and client app to control/communicate via RIOT HTTP protocol.
It provides reusable building blocks and a standard interfaces that hides the details of the hardware.
### Riot
This project defines all the base interfaces and classes for RIOT. Riot uses composite pattern to form the base interfaces and classes.
Refer to other projects to use and extend the base classes.

### Riot.IoDevice
The library implements RIOT clients for some IO devices/sensors such as DC motor, servo, DHT, and ultrasonic sensors.

### Riot.Pi
The Riot.Pi wraps the Raspberry Pi's system and gpio in simple RIOT HTTP protocol. Riot.Pi contains the data and nodes for Raspberry Pi's system and gpio.

### Riot.SmartPlug
The Riot.SmartPlug implements the client to access/control smart plugs through web service using RIOT HTTP protocol.
Currently, only the Kasa HS1xx smart plugs are supported.
It includes followings.

### RiotDevices
The RiotDevices is a simple Android app to monitor and control the followings:
* Monitor Raspberry Pi's CPU amd memory status. System commands, like reboot and shutdown, can also posted to the Pi.
* View Kasa smart plugs (model HS1xx) status and control on/off of the plugs.
* Control robotic PiCar (Adeept Mars PiCar)

## RiotDevices - Android App
The RiotDevices is a simple Android app to monitor and control devices via web service using RIOT HTTP protocol.
The Android App is built with the Riot libraries and Xamarin Forms. It should be relative easy to make it work on iOS but had not tried on any IOS devices!!
* Monitor Raspberry Pi's CPU amd memory status. 
* Send system commands, like reboot and shutdown, can also posted to the Pi.
* View Kasa smart plugs (model HS1xx) status 
* Control on/off of the switch, reboot the plug, and set led-off status.
* Control robotic PiCar (Adeept Mars PiCar)
* Video streaming via HTTP.

## Examples
* picarServer has examples for the followings:
    * custom request handler class - PiCarRequestHandler
    * usage of iotServerLib - PiCar, PiCarDrive, PiCarHead
    * process HTTP requests with Flask - server.py
    * video streaming - picar.py, picarCamera.py
* HTTP Request examples using HTTPie: httpieRequests.txt
* Simple Raspberry Pi HTTP server: under piserver folder
    * requires Freenove_DHT.py from github: https://github.com/Freenove/Freenove_Ultimate_Starter_Kit_for_Raspberry_Pi/tree/master/Code/Python_Code/21.1.1_DHT11
    * PiHygroThermoDHT - custom request handler class
    * server.py - simple HTTP server using Flask


    
