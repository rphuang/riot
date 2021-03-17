# Riot.Phone
The Riot.Phone provides smartphone's sensor data via Riot web service. It also supports some actions that can be posted to the service to perform on the phone.
For now, Riot.Phone only supports simple web service to get (pull) sensor data and post a few actions. The client classes and push notification are not yet supported.

## Data - Riot.Phone
Defines sensor and action data for sensors in a smart phone.
* Phone and sensor data: BatteryData, BoolData, DeviceInfoData, LocalesData, LocationData, QuaternionData, SensorRate, SensorStatusData, Vector3Data
* Action data: CaptureActionData, EmailActionData, SmsActionData, SpeechActionData

## Sensor Service Nodes - Riot.Phone.Service
Implements the service nodes for serving data from phone with Xamarin.Essentials. BaseServiceNode is the base class for all sensor service nodes. BaseServiceNode contains sensor status data.
* AccelerometerService - serves acceleration and shack data from Accelerometer
* BarometerService - provides pressure data for Barometer
* BatteryService - provides battery status information
* CompassService - provides heading data for Compass
* GpsLocationService - provides location data for Gps sensor
* GyroscopeService - provides AngularVelocity from Gyroscope
* MagnetometerService - provides MagneticField from Magnetometer
* OrientationSensorService - provides orientation data for Orientation sensor
* PhoneService - service container for all the sensor and action service nodes

## Action Service Nodes - Riot.Phone.Service
Implements the service nodes for posting action to the phone using Xamarin.Essentials. BaseActionService is the base class for all action service nodes.
* CaptureActionService - turns on camera to capture picture or video
* EmailActionService - starts email client with received body text and recipients
* SmsActionService - starts email client with received message text and recipients
* TextToSpeechService - text to speech with received text
* VibrateActionService - vibrates the phone for specified duration

## Service Host and Request Handlers
These are the classes to provide RIOT web service for the phone's sensors and actions.
* PhoneServiceHost - derived from HttpServiceHost to construct request handlers for all sensor/action service nodes listed above
* ActionRequestHandler - derived from HttpServiceRequestHandler to handle request to action services
* PhoneRequestHandler - derived from HttpServiceRequestHandler to handle request to sensor services
* SubscribeRequestHandler - derived from HttpServiceRequestHandler to handle subscription request to sensor services

## Client Nodes - Riot.Phone.Client
TBD
