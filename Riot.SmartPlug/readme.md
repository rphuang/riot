# Riot.SmartPlug
The Riot.SmartPlug implements the client to access/control smart plugs through web service using RIOT HTTP protocol.
Currently, only the Kasa HS1xx smart plugs are supported.
It includes followings.

## Data - Riot.SmartPlug
Defines all the data from server.
* KasaHs1xxEmeterData - the emeter data from HS1xx
* KasaHs1xxSystemData - the system information data from HS1xx
* KasaHs1xxTimeData - the time and timezone data from HS1xx

## Client Nodes - Riot.SmartPlug.Client
Implements the client nodes to access and control smart plugs using RIOT Http protocol.
* KasaHs1xxClient - the composite client node for Kasa HS1xx it contains the following nodes
    * KasaHs1xxEmeterClient - the client node for getting emeter data
    * KasaHs1xxSystemClient - the client node for getting system data
    * KasaHs1xxTimeClient - the client node for getting time data
* SmartPlugClient - the composite client node that hosts multiple smart plugs
* SmartPlugClientFactory - the factory that creates all client nodes based on discovered endpoints from server

## Service Nodes - Riot.SmartPlug.Service
Currently not implemented. Please use the Python version for RIOT services under pySmartPlugService folder.
