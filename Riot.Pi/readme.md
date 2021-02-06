# Riot.Pi
The Riot.Pi wraps the Raspberry Pi's system and gpio in simple RIOT HTTP protocol. Riot.Pi contains the data and nodes for Raspberry Pi's system and gpio.
It includes followings.

## Data - Riot.Pi
Defines all the data from server.
* SystemData - the data for Pi system that contains cpu and memory data
    * CpuData - defines the cpu temperature and overall usage data with optional usage data for the cpu cores
    * MemoryData - defines the memory usage data
* GpioData - the data for Pi's GPIO. It contains all the pin data.
    * GpioPinData - the data for an individual pin

## Client Nodes - Riot.Pi.Client
Implements the client nodes for getting data from server using RIOT Http protocol.
* PiClient - the composite node for Pi client. It sends a discover request to server to initialize supported client nodes.
* SystemClient - the composite client node for corresponding SystemData. It contains following nodes.
    * CpuClient - the client node for getting CpuData
    * MemoryClient - the client node for getting MemoryData
* GpioClient - the composite client node for getting GpioData. It is more efficient for GpioClient to issue single request to server to get data for all pins.
    * GpioPinClient - the client node for getting GpioPinData. Should be used to get data for a single pin.
* PiClientFactory - the factory that creates all client nodes based on discovered endpoints from server

## Service Nodes - Riot.Pi.Service
Currently not implemented. Please use the Python version for RIOT services under piServices folder.
