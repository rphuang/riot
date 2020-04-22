#!/usr/bin/python3
# File name   : piConstants.py
# Description : common module for constants

# constants commponly used in response dictionary
KeyStatusCode = 'statusCode'
KeyResponse = 'response'

# constants for common properties in response dictionary
KeyPropertyKey = 'key'
KeyPropertyValue = 'value'

# constants used in url path and for response dictionary
KeyGpioPropertyPin = 'pin'
KeyGpioPropertyMode = 'mode'

# constants for url path
KeyDeviceSystem = 'sys'
KeyDeviceGpio = 'gpio'
KeyDeviceCamera = 'camera'
KeyComponentCpu = 'cpu'
KeyComponentMemmory = 'memory'
KeyComponentStorage = 'storage'

# constants used in response dictionary from system component cpu
KeyCpuUsageProperty = 'usage'
KeyCpuUserUsageProperty = 'userUsage'
KeyCpuSystemUsageProperty = 'systemUsage'
KeyCpuIdleProperty = 'idle'
KeyCpuTemperatureProperty = 'temperature'
KeyCoresProperty = 'cores'

# constants used in response dictionary from system component memory
KeyMemoryTotalProperty = 'total'
KeyMemoryCachedProperty = 'cached'
KeyMemoryUsedProperty = 'used'
KeyMemoryFreeProperty = 'free'
KeyMemoryAvailableProperty = 'available'
KeyMemoryUsedPercentProperty = 'usedPercent'

