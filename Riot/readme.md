# Riot Base 
This project defines all the base interfaces and classes for RIOT. Riot uses composite pattern to form the base interfaces and classes.
Refer to other projects to use and extend the base classes.


* Interfaces
** IIotPoint - the base interface for all nodes and data.
** IIotNode - the base node interface that leverage composite pattern.
** ILog - the interface for logging
* Data and Node classes
** IotData - base class for all IOT data
** IotNode - base class for all IOT nodes
** IotPoint - base class for all IOT points
** IotLog - default log to console
** LogUtil - static utility class for loging
* HTTP utilities
** HttpEndpoint - defines the properties for an endpoint that can be discovere from RIOT service
** HttpRequest - convenient wrapper for http request that without throwing exception upon non-success status
** HttpResponse - convenient wrapper for http response
** HttpTargetSite - defines the properties for a target site that used for push notification
* HTTP client
** IotClientNode - an IotNode that implements HTTP protocol
** IotHttpClient - utility class for http request
** IotClientFactory - the base factory class and static members to discover and create client nodes
