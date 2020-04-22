#!/usr/bin/python3
# File name : piStats.py
# Description : Pi status class
import re
import time
import sys
import subprocess
from piServices.piConstants import *
from piServices.piUtils import *

# local constants for keys in dict
KeyCpuInfoTotal = 'total'
KeyCpuInfoActive = 'active'
KeyCpuInfoIdle = 'idle'
KeyCpuInfoPercent = 'percent'

# the dictionary for storing the last cpu data { cpuName: cpuValues }
#    key: cpuName
#    value: cpuValues is a list of values as described below foe corresponding
#    index
# example: { 'cpu': [100, 10, 90, 4, 6], 'cpu0': [100, 10, 90, 4, 6] }
# the index for values stored in cpu value - a list (self.lastCpuData)
IndexCpuTotal = 0
IndexCpuActive = 1
IndexCpuIdle = 2
IndexCpuUser = 3
IndexCpuSystem = 4

class PiStats(object):
    """ PiStats provides system statistics for cpu usages, memory
    Usage of the PiStats
    1. create an instance of PiStats
    2. run updateStatusForever in a thread
    3. use the followings to get data
       - get_all_info: get all data - cpu (without cores), memory
       - get_cpu_info: get cpu data with cores
       - get_memory_info: get memory data
    """
    def __init__(self):
        self.total_memory = None
        self.free_memory = None
        self.cached_memory = None
        self.temp_in_celsius = None

    def get_cpu_temperature(self):
        try:
            s = subprocess.check_output(['/usr/bin/vcgencmd','measure_temp'])
            return str(s).split('=')[1][:-3]
        except:
            return '0'

    def get_memory_info(self):
        # In linux the cached memory is available for program use so we'll
        # include it in the free amount when calculating the usage percent
        with open('/proc/meminfo', 'r') as mem_file:
            # Remove the text description, kB, and whitespace before
            # turning file lines into an int
            for i, line in enumerate(mem_file):
                if i == 0: # Total line
                    self.total_memory = int(line.strip("MemTotal: \tkB\n")) / 1024
                elif i == 1: # Free line 
                    self.free_memory = int(line.strip("MemFree: \tkB\n")) / 1024
                elif i == 4: # Cached line
                    self.cached_memory = int(line.strip("Cached: \tkB\n")) / 1024
        used_val = (self.total_memory - self.free_memory - self.cached_memory)
        available_val = (self.free_memory + self.cached_memory)
        percent_val = float(used_val) / float(self.total_memory)
        return {KeyMemoryTotalProperty: int(self.total_memory), KeyMemoryCachedProperty: int(self.cached_memory),
                KeyMemoryUsedProperty: int(used_val), KeyMemoryFreeProperty: int(self.free_memory), 
                KeyMemoryAvailableProperty: int(available_val), KeyMemoryUsedPercentProperty: round(percent_val, 3) * 100.00 }

    def get_all_info(self):
        cpuData = self.cpuStatsWithoutCores
        cpuData.update({KeyCpuTemperatureProperty: float(self.get_cpu_temperature().strip('\'C'))})
        return {KeyComponentCpu: cpuData,
                KeyComponentMemmory: self.get_memory_info()}

    def get_cpu_info(self):
        """ returns the current cpu statistics
        the updateStatusForever() must be called to run in a thread for valid return

        data returned is a dict for the cpu properties listed below:
        - usage - total usage percent of the cpu/core
        - user - total user usage percent of the cpu/core
        - system - total system usage percent of the cpu/core
        - idle - total idle percent of the cpu/core
        - temperature - the temperature of the cpu
        - cores - a list of cores that contain the above properties
        """
        data = self.cpuStatsWithCores
        data.update({KeyCpuTemperatureProperty: float(self.get_cpu_temperature().strip('\'C'))})
        return data

    def get_memory_data(self):
        with open('/proc/meminfo', 'r') as mem_file:
            # Remove the text description, kB, and whitespace before
            # turning file lines into an int
            for i, line in enumerate(mem_file):
                if i == 0: # Total line
                    self.total_memory = int(line.strip("MemTotal: \tkB\n")) / 1024
                elif i == 1: # Free line 
                    self.free_memory = int(line.strip("MemFree: \tkB\n")) / 1024
                elif i == 4: # Cached line
                    self.cached_memory = int(line.strip("Cached: \tkB\n")) / 1024

    def get_cpu_data(self, line):
        """ parse the line and returns a tuple [ cpuName, cpuValues ]
        the cpuValues is [total, active, idle, user, system]
        """
        # columns:
        #  0 : cpu name
        #  1 : user = normal processes executing in user mode
        #  2 : nice = niced processes executing in user mode
        #  3 : system = processes executing in kernel mode
        #  4 : idle = twiddling thumbs
        #  5 : iowait = waiting for I/O to complete
        #  6 : irq = servicing interrupts
        #  7 : softirq = servicing softirqs
        columns = re.findall('([cpu0-9]+)', line.strip())
        cpuTotal = 0
        index = 0
        for t in columns:
            if index > 0:
                cpuTotal += int(t)
            index += 1
        cpuIdle = int(columns[4])
        cpuActive = cpuTotal - cpuIdle
        return [ columns[0], [cpuTotal, cpuActive, cpuIdle, int(columns[1]), int(columns[3])]]

    def calc_cpu_percent(self, cpuTotal, newValues, lastValues, index):
        """ calculate the cpu percentage for the index """
        value = newValues[index] - lastValues[index]
        return round(float(value) / float(cpuTotal), 3) * 100.00 

    def updateStatusForever(self, delay_second):
        """ update cpu usage data for every delay_second
        """
        timePrint("PiStats updating status every " + str(delay_second) + " second")
        initValues = [0, 0, 0, 0, 0]
        self.lastCpuData = { 'cpu': initValues, 'cpu0': initValues, 'cpu1': initValues, 'cpu2': initValues, 'cpu3': initValues} 
        while True:
            with open('/proc/stat', 'r') as cpu_file:
                coresResponse = {}
                for i, line in enumerate(cpu_file):
                    if 'cpu' in line:
                        cpuName, cpuValues = self.get_cpu_data(line)
                        lastValues = self.lastCpuData[cpuName]
                        cpuTotal = cpuValues[IndexCpuTotal] - lastValues[IndexCpuTotal]
                        cpuActivePercent = self.calc_cpu_percent(cpuTotal, cpuValues, lastValues, IndexCpuActive) 
                        cpuIdlePercent = self.calc_cpu_percent(cpuTotal, cpuValues, lastValues, IndexCpuIdle) 
                        cpuUserPercent = self.calc_cpu_percent(cpuTotal, cpuValues, lastValues, IndexCpuUser) 
                        cpuSystemPercent = self.calc_cpu_percent(cpuTotal, cpuValues, lastValues, IndexCpuSystem) 
                        cpuResponse = {cpuName: 
                                       {KeyCpuUsageProperty: cpuActivePercent, KeyCpuUserUsageProperty: cpuUserPercent,
                                       KeyCpuSystemUsageProperty: cpuSystemPercent, KeyCpuIdleProperty: cpuIdlePercent}}
                        if i == 0:
                            allCpuResponse = cpuResponse
                            #timePrint(str(cpuResponse))
                        else:
                            coresResponse.update(cpuResponse)
                        self.lastCpuData.update({cpuName: cpuValues})
                    else:
                        break
            # update member var
            self.cpuStatsWithoutCores = {}
            self.cpuStatsWithoutCores.update(allCpuResponse[KeyComponentCpu])
            allCpuResponse[KeyComponentCpu].update({KeyCoresProperty: coresResponse})
            self.cpuStatsWithCores = allCpuResponse[KeyComponentCpu]
            #timePrint("PiStats updated")
            time.sleep(delay_second)

