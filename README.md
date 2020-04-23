# Overview
RIOT (REST IOT) is intended to simplify and componentize the code to build robotic kits around Raspberry Pi for both server (backend) and clients. 
It supports the Adeept Mars PiCar. However, it is designed with flexibility and extensibility and is relatively easy to extend to other kits and for general purpose IOT.

There followings are in the repository.
* piServices package - an HTTP service with simple REST IOT protocol for client-server communication. It provides basic gpio, cpu, and memory data via HTTP. Extensibility example can be found in the picarServer.
* iotServerLib package - a set of classes that encapsulates sensor/drive compoennts (for now mostly limited to what in Adeept Mars PiCar). Usage example can be found in the picarServer.
* videoStreaming package - an HTTP video streming service based on Raspberry Pi camera. Usage example can be found in the picarServer.
* picarServer module - this is the server for Adeept Mars PiCar. Besides the otiginal TCP support it added HTTP support. It is (mostly) backward compatible with existing TCP client.
* Client IOT lib (.NET C#) - a set of classes that encapsulates sensor/device compoennts on the server via HTTP. For now, mostly limited to what in Adeept Mars PiCar.
* Simple Android app (Xamarin based) - to control Adeept Mars PiCar via HTTP 
* examples - contains simple example code for using the packages

There bounds to be errors and things to improve, so please don't hesitate to send corrections and feedbacks.

# Getting Started
1. download/clone the respository
2. Install the Python packages with pip3

    cd path-to-installed-folder
    sudo python3 pip3 install .

3. Install and run PiCar server
    1. create folders data & templates under path-to-installed-folder/picarServer
    2. copy files from path-to-installed-folder/videoStreaming/data to path-to-installed-folder/picarServer/data
    3. copy files from path-to-installed-folder/videoStreaming/templates to path-to-installed-folder/picarServer/templates
    4. adjust config file path-to-installed-folder/picarServer/picarconfig.txt
    5. run picar server

        python3 path-to-installed-folder/picarServer/server.py path-to-installed-folder/picarServer/picarconfig.txt
    
# Coming 
* ServiceStack based HTTP server for RIOT that supports connected Arduino devices with REST-ish non-HTTP communication.

# Details

## REST Protocol
RIOT (REST for IOT) uses a simple HTTP REST protocol for client-server communication. For current version, the server always response with JSON.

### HTTP Url
The url path is the unique identification to the target.

    <scheme>://<host>/<device>/<component>/<property>[?parameters]
    
* Scheme: http (will support https in the future)
* Host: the server’s IP address or name and corresponding port specification
* Device: the device ID. The device is required in the url.
* [Optional] Component: the component ID. Component may contains multiple layers separated with ‘/’. 
* [Optional] Property: value | mode | property of the component (default to value)
* parameters: reserved for future use

### HTTP Methods
* Get – to retrieve information/data from the target server device. In general this does not alter the state of target server device.
* Post and Put – to set value or send command to a target server device.

### HTTP Body
The HTTP body is for PUT and POST only. It contains the value or content that is sent to the server target.

## piServices
The Python piServices package implements the base classes to support RIOT HTTP protocol plus basic services for Raspberry Pi. It has dependency on Flask.
* BaseRequestHandler - the base class for handling HTTP request. The class supports custom handler by implementing a handler derived from this base class. All instances of derived classes will be automatically registered (via __init__) with root paths (the device in the protocol) to handle. 
    * doGet/doPut/doPost/doDelete - Derived classes implement these to handle corresponding request. By default, doPut calls doPost so they behave the same.
    * checkAuth - check authorization. Override to implement custom authorization.
    * processRequest - a class method that should be called to handle http requests. It finds the handler instances corresponding to the request's root path (device) from registered instances. The instance will be invoked to handle the requests for the root path (device). 
* PiGpioRequestHandler - this class is derived from BaseRequestHandler to handle requests for GET and POST from/to Raspberry Pi GPIO.
* PiSysRequestHandler - this class is derived from BaseRequestHandler to handle requests for GET information from Raspberry Pi system (cpu, memory). The POST allows executing commands on the Pi.

## iotServerLib
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

## videoStreaming
This package supports an HTTP video streming service based on Flask. The main code is from https://blog.miguelgrinberg.com/post/video-streaming-with-flask with addition of face tracking.
* FaceTracker - detect and track faces with input image frames
videoStreaming works in either Windows and Pi:
Windows: just run python videoStreamingService.py
Pi: just run python3 videoStreamingService-pi.py

## picarServer
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

## IotClientLib
This is a C# client lib that can be used to build client app to control/communicate RIOT HTTP based server. It provides reusable building blocks and a standard interface that hides the details of the hardware. Initial version supports the components of Adeept Mars PiCar.
The lib has two parts - one set of interfaces for each component and a second set of concrete classes that implement those interfaces.
* HttpNode - the base class for all http implementation. As the base node for composite pattern, the base encapsulates both leaf and composite nodes.
* HttpMotor - represent a motor
* HttpServo - represent a servo
* HttpUltrasonic - represent an ultrasonic sensor
* HttpRGBLed - represent an RGB LED
* HttpStripLed - represent a strip LED
* HttpGpio - represent the GPIO on a Raspberry Pi that contains HttpGpioPins
* HttpGpioPin - represent a GPIO pin on Raspberry Pi
* HttpSystem - represent a server system that contains CPU & Memory
* HttpCpu - CPU information on the server
* HttpMemory - memory information on the server

## PiCar Android App
The PiCar Android App is built with the IotClientLib and Xamarin Forms. It should be relative easy to make it work on iOS.
* Control Adeept Mars PiCar via HTTP
* Video streaming via HTTP. Known Issue: video doesn't fit the screen.
* Display server CPU memory information
* Shutdown/reboot and user commands

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


    
