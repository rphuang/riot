# PiCar Server
This is a server module that controls the Adeept PiCar-B. Besides controling with the TCP connections it also supports controling through HTTP web service using RIOT protocol.

## Major Features
* Backward compatible with the TCP client that was provided by Adeept (Version 1.1)
* HTTP web service that allows client to retrieve and control PiCar
* HTTP web service that streams the video output from the PiCar camera with face tracking
* HTTP web service that allows client to access the cpu/memory status for the hosting Raspberry Pi system
* Seding commands to Raspberry Pi, for example, reboot and shutdown
* Simple Dev-ish Android client app that controls the PiCar with live video stream
* Send command to speech to Android phone that runs the client app
* Smart motor and drive control - control the speed during start and use ultrasonic to avoid obstacles when heading straight

## Installation
1. download/clone the respository. These steps assume that the code is under /home/pi/riot.
2. Install the Python packages with pip3. This will install the Python packages required for PiCar Server.
```
cd /home/pi/riot
sudo python3 pip3 install .
```
3. Configure PiCar server. Basically this is to edit the config file /home/pi/riot/picarServer/picarconfig.txt that defines the hardware and software configurations for PiCar.
Please refer the the Configuration section for details.
4. run picar server
```
        python3 /home/pi/riot/picarServer/server.py /home/pi/riot/picarServer/picarconfig.txt
```
5. Autorun after boot. The existing Adeept autorun will still work except change the folder of server.py. A new init file is also included under /home/pi/riot/picarServer/init.d/picarServer.
    1. setup /etc/init.d/picarServer
```
sudo cp /home/pi/riot/picarServer/init.d/picarServer /etc/init.d
sudo chmod +x /etc/init.d/picarServer
```
    2. findout what runlevel is. 
```
runlevel
```
    3. create a link to picarServer (for runlevel 5)
```
cd /etc/rc5.d
sudo ln -s /etc/init.d/picarServer ./S99picarServer
```
6. Install Android Client App. Following the steps in RiotDevices to build and deploy the Andriod client app.

## Configuration
### Configure the PiCar services
This is to configure the ports for http web service and video streaming plus a list of authorized encoded credentials.
* picar.httpVideoPort=8002
* piCar.AuthorizationList=
* httpserver.httpPort=8001
### Servo Configuration
There are three servos in PiCar.
* steering - controls the direction of PiCar
* horizontal - controls the horizontal position of camera and ultrasonic sensor
* vertical - controls the vertical position of camera and ultrasonic sensor
The hardware configuration for a servo is very simple since the control is using PCA9685. With Adafruit_PCA9685, it only requires one config - the channel of the I2C for the servo. These are defined by *.channel in the config file.
There is a bit of tuneup effort for the software configuration to find out the limit of each servo. First, let's clarify how the position is controlled.
* Physical pulse length (physical position) - this is the PWM cycle to control the servo. The range is from 0 to 4095.
* Angular position - this is the angle of the servo with 180 degrees. For PiCar, all servos are using angular position that ranges from -90 degree (Down/Left) to +90 degree (Up/Right) with the center at 0 degree. 
The fowwling config settings are defines for each servo.
* min - the minimum physical pulse length between 0 to 4095
* max - the maximum physical pulse length between 0 to 4095
* minAngle - the minimum anglular position that corresponds to the min value
* maxAngle - the maximum anglular position that corresponds to the max value
* centerAngle - the center angular position
The tuneup effort is to findout different limits for each servo by changing the min and max values.
For example, for the horizontal servo, reduce the horizontalServo.min value and increase horizontalServo.minAngle to see whether the camera can move further right.
These are the config settings for all servos with the default values.
* Configure the PiCar steering servo
    * steering.min=200
    * steering.max=510
    * steering.minAngle=45
    * steering.maxAngle=-30
    * steering.centerAngle=0
* Configure the PiCar servo that controls the vertical position of camera and ultrasonic sensor
    * verticalServo.min=295
    * verticalServo.max=645
    * verticalServo.minAngle=-50
    * verticalServo.maxAngle=85
    * verticalServo.centerAngle=0
* Configure the PiCar servo that controls the horizontal position of camera and ultrasonic sensor
    * horizontalServo.min=130
    * horizontalServo.max=670
    * horizontalServo.minAngle=60
    * horizontalServo.maxAngle=-120
    * horizontalServo.centerAngle=0
### Camera Configuration
The settings can configure the image size, enable face tracking, location of the facrtracking classifier, and enable the crosshair in video.
* camera.width=320
* camera.height=240
* camera.enableFaceTracking=true
* camera.classifier=/home/pi/riot/picarServer/data/haarcascade_frontalface_alt.xml
* camera.drawCrosshair=true
### Ultrasonic Sensor Scan
    ultrasonic.maxDistanceCm=1500
### Hardware Configuration
This is to configure the IO pins and channels for the PiCar. Most likely, you don't have to touch these config settings.
* motor.enable=7
* motor.in1=10
* motor.in2=8
* steering.channel=2
* leftLED.redPin=15
* leftLED.greenPin=16
* leftLED.bluePin=18
* rightLED.redPin=19
* rightLED.greenPin=21
* rightLED.bluePin=22
* ultrasonic.triggerPin=23
* ultrasonic.echoPin=24
* verticalServo.channel=0
* horizontalServo.channel=1
* stripLed.ledCount=12
* stripLed.ledPin=12
* lineTracking.leftPin=38
* lineTracking.middlePin=36
* lineTracking.rightPin=35
### Configure the TCP port
* tcpserver.tcpPort=10223

## Feature Thoughts
* Outdoor Scenario - use Raspberry Pi to create WiFi hotspot so it can be run in outdoor that has no WiFi
* Outdoor Scenarios with phone sensor service
    * WiFi hotspot from Raspberry Pi
    * Attach a phone to PiCar and provide phone sensor service
    * Scenarios
        * provide PiCar location
        * provide PiCar speed and heading
        * control Picar to automatically move to a specific GPS location
* Home base scenarios
    * WiFi hotspot from Raspberry Pi
    * Attach a phone to PiCar and provide phone sensor service
    * Enable phone sensor service on the controller as "home"
    * Scenarios
        * findout distance between home and PiCar
        * ask PiCar to return to home base automatically
        * follow the controller or keep the same formation (distance and direction)
        * circle around the home base

