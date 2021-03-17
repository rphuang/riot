using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    public class GpsLocationService : BaseServiceNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public GpsLocationService()
            : this(nameof(GpsLocationService), null, GeolocationAccuracy.Medium, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public GpsLocationService(IotNode parent)
            : this(nameof(GpsLocationService), parent, GeolocationAccuracy.Medium, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public GpsLocationService(string id, IotNode parent, GeolocationAccuracy accuracy, TimeSpan timeout) : base(id, parent) // , accuracy, timeout
        {
            Location = new LocationData { Id = nameof(Location) };
            _request = new GeolocationRequest(accuracy, timeout);
        }

        /// <summary>
        /// latest location value
        /// </summary>
        public LocationData Location
        {
            get { return GetData<LocationData>(nameof(Location)); }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        protected override bool IsOn { get { return _gpsStarted; } }

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed)
        {
            if (!_gpsStarted)
            {
                _gpsStarted = true;
                _request.DesiredAccuracy = (GeolocationAccuracy)(int)speed;
                _gpsTask = Task.Run(async () =>
                {
                    while (_gpsStarted)
                    {
                        try
                        {
                            Location oldLocation = Location.Value;
                            // get stock price
                            Location location = await GetLocationAsync();
                            if (_gpsStarted)
                            {
                                // todo: add check for min distance change
                                Location.SendNotification();
                            }
                        }
                        catch
                        {
                        }
                        await Task.Delay(_gpsTaskDelayInMilliSeconds[(int)speed]);
                    }
                });
            }
            return true;
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor()
        {
            _gpsStarted = false;
            return true;
        }

        /// <summary>
        /// Get Location Async
        /// </summary>
        public async Task<Location> GetLocationAsync()
        {
            try
            {
                Location location = await Geolocation.GetLocationAsync(_request);
                LocationData data = Location;
                data.Value = location;
                data.TimeStamp = DateTime.UtcNow;

                return location;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            return null;
        }

        /// <summary>
        /// Get last known Location Async
        /// </summary>
        public async Task<Location> GetLastKnownLocationAsync()
        {
            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();
                Location.Value = location;

                if (location != null)
                {
                    Console.WriteLine(Location.ToString());
                }
                return location;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            return null;
        }

        private GeolocationRequest _request;
        private bool _gpsStarted;
        private Task _gpsTask;
        private int[] _gpsTaskDelayInMilliSeconds = { 5000, 30000, 10000, 5000, 2000, 1000 };
    }
}
