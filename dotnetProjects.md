# RIOT (REST IOT) .NET Projects

## Riot
This project defines all the base interfaces and classes for RIOT. Riot uses composite pattern to form the base interfaces and classes.
Refer to other projects to use and extend the base classes.

## Riot.IoDevice
The library implements RIOT clients for some IO devices/sensors such as DC motor, servo, DHT, and ultrasonic sensors.

## Riot.Pi
The Riot.Pi wraps the Raspberry Pi's system and gpio in simple RIOT HTTP protocol. Riot.Pi contains the data and nodes for Raspberry Pi's system and gpio.

## Riot.SmartPlug
The Riot.SmartPlug implements the client to access/control smart plugs through web service using RIOT HTTP protocol.
Currently, only the Kasa HS1xx smart plugs are supported.
It includes followings.

## RiotDevices
The RiotDevices is a simple Android app to monitor and control the followings:
* Monitor Raspberry Pi's CPU amd memory status. System commands, like reboot and shutdown, can also posted to the Pi.
* View Kasa smart plugs (model HS1xx) status and control on/off of the plugs.
* Control robotic PiCar (Adeept Mars PiCar)

## Riotcmd
This project implements a simple command line to get cpu/memory data from Raspberry Pi. This also serves as an example of using the RIOT packages.
Sample command: pi -a user:password 192.168.0.11:1234/gpio/38

## EntityLib
The EntityLib class library provides a simple PropertyBag and base Entity class.
PropertyBag is a dictionary based object - all properties are stored in dictionary and can be acesses via the property name.
Entity adds Id and Type properties on top of PropertyBag.

## SettingsLib
SettingsLib provides simple text based settings stored in local storage. Settings can be defined in a group that can be used like an object.
See RiotDevices on how to use SettingGroup.

## PlatformLib
PlatformLib encapsulates the platform specifics. For now, only storage is supported.
