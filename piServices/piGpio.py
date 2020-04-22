#!/usr/bin/python3
# File name   : piGpio.py
# Description : gpio utility module 

import RPi.GPIO as GPIO
from piServices.piConstants import *
from piServices.piUtils import *

# constant defines all the pins are not valid
# note: pin 1, 2 are not valid
InvalidPins = [4, 6, 9, 14, 17, 20, 25, 27, 28, 30, 34, 39]

def setup():
	GPIO.setwarnings(False)
	GPIO.setmode(GPIO.BOARD)

def get(pin, property):
    """ get the mode and value of the specified pin
    pin: the physical pin #
    property: whether to get mode or valur for the pin. by default, both will be returned.

    returns a dict of pin, mode, value
    pin: the pin number
    mode: the pin's function mode. possible value: GPIO.IN, GPIO.OUT, GPIO.SPI, GPIO.I2C, GPIO.HARD_PWM, GPIO.SERIAL, GPIO.UNKNOWN
    value: the current value of the pin.
    response: contains the error when error occurs
    """
    #timePrint('gpioModule.get pin: ' + str(pin) + ' property: ' + property)
    if not isValidPin(pin):
        return {KeyStatusCode: 400, KeyResponse: 'InvalidPin'}

    try:
        #GPIO.setup(pin, GPIO.IN)
        mode = GPIO.gpio_function(pin)
        GPIO.setup(pin, mode)
        if KeyGpioPropertyMode in property:
            response = { KeyStatusCode: 200, KeyGpioPropertyPin: pin, KeyGpioPropertyMode: mode }
        elif KeyPropertyValue in property:
            value = GPIO.input(pin)
            response = { KeyStatusCode: 200, KeyGpioPropertyPin: pin, KeyPropertyValue: value }
        elif len(property) == 0:
            value = GPIO.input(pin)
            response = { KeyStatusCode: 200, KeyGpioPropertyPin: pin, KeyGpioPropertyMode: mode, KeyPropertyValue: value }
        else:
            response = { KeyStatusCode: 400, KeyResponse: 'InvalidProperty' }
    except Exception as e:
        # todo: classify different exceptions and http status codes
        timePrint('Exception gpioModule.get: ' + str(e))
        response = { KeyStatusCode: 500, KeyResponse: 'Exception' }
    return response

def getAll():
    """ get all the pins mode/value

    returns [statusCode, list of dict of pin, mode, value]
    statusCode: the overall http status code
    pin: the pin number
    mode: the pin's function mode. possible value: GPIO.IN, GPIO.OUT, GPIO.SPI, GPIO.I2C, GPIO.HARD_PWM, GPIO.SERIAL, GPIO.UNKNOWN
    value: the current value of the pin.
    response: contains the error when error occurs
    """
    response = []
    goodCount = 0
    badCount = 0
    statusCode = 200
    for pin in range(3, 41):
        if pin not in InvalidPins:
            try:
                mode = GPIO.gpio_function(pin)
                GPIO.setup(pin, mode)
                value = GPIO.input(pin)
                pinResponse = { KeyGpioPropertyPin: pin, KeyGpioPropertyMode: mode, KeyPropertyValue: value }
                goodCount += 1
            except Exception as e:
                # todo: classify different exceptions and http status codes
                timePrint('Exception gpioModule.get: ' + str(e))
                pinResponse = { KeyStatusCode: 500, KeyResponse: 'Exception', KeyGpioPropertyPin: pin }
                badCount += 1
            response.append(pinResponse)
    if badCount == 0:
        statusCode = 200
    elif goodCount == 0:
        statusCode = 500
    else:
        statusCode = 206
    return [statusCode, response]

def set(pin, property, value):
    """ set the pin's property to the specified value
    pin: the physical pin #
    property: whether to set mode or valur for the pin. The default is value.
    value: the value to set

    returns a dict of followings:
    response: contains the action or error
    pin: the pin number
    mode: the mode value to set. possible value: GPIO.IN, GPIO.OUT, GPIO.SPI, GPIO.I2C, GPIO.HARD_PWM, GPIO.SERIAL, GPIO.UNKNOWN
    value: the value of the pin to set.
    """
    timePrint('gpioModule.set pin: %s value: %s property: %s' %(str(pin), str(value), property))
    if not isValidPin(pin):
        return {KeyStatusCode: 400, KeyResponse: 'InvalidPin'}
    try:
        GPIO.setup(pin, GPIO.OUT)
        if KeyGpioPropertyMode in property:
            GPIO.setup(pin, value)
            response = { KeyStatusCode: 200, KeyResponse: 'SetMode', KeyGpioPropertyPin: pin, KeyGpioPropertyMode: value }
        elif KeyPropertyValue in property:
            GPIO.output(pin, value)
            response = { KeyStatusCode: 200, KeyResponse: 'SetValue', KeyGpioPropertyPin: pin, KeyPropertyValue: value }
        elif len(property) == 0:
            GPIO.output(pin, value)
            response = { KeyStatusCode: 200, KeyResponse: 'SetValue', KeyGpioPropertyPin: pin, KeyPropertyValue: value }
        else:
            response = { KeyStatusCode: 400, KeyResponse: 'InvalidProperty' }
    except Exception as e:
        # todo: classify different exceptions and http status codes
        timePrint('Exception gpioModule.get: ' + str(e))
        response = { KeyStatusCode: 500, KeyResponse: 'Exception', KeyGpioPropertyPin: pin }
    return response

def isValidPin(pin):
    """ validate the pin # 
    returns True if the pin is valid
    """
    if pin > 2 and pin < 41 and pin not in InvalidPins:
        return True
    else:
        return False