# Riot.IoDevice
The Riot.IoDevice implements the client to access/control IO sensors/devices through web service using RIOT HTTP protocol.
It includes followings.

## Data - Riot.IoDevice
Defines all the data from server.
* HydroThermoData - the DHT sensor data from server
* UltrasonicData - the ultrasonic sensor data from server
* MotorData - the DC motor speed data from server
* RGBLedData - the  RGB LED data from server
* ServoData - the servo data from server
* StripLedData - the strip LED data from server
* DistanceScanData - the distance scan data from server (for a PiCar)

## Client Nodes - Riot.IoDevice.Client
Implements the client nodes to access and control sensors/devices using RIOT Http protocol.
* HydroThermoClient - the client node to access DHT sensor data
* UltrasonicClient - the client node to access ultrasonic sensor data from server
* MotorClient - the client node to access/control DC motor speed
* RGBLedClient - the client node to access/control RGB LED
* ServoClient - the client node to access/control servo on server
* StripLedClient - the client node to control strip LED
* DistanceScanClient - the client node to issue distance scan and get result data from server

## Service Nodes - Riot.IoDevice.Service
Currently not implemented. Please use the Python version for RIOT services under pyCarServer folder that provides most of the sensors/devices.
