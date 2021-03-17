using Newtonsoft.Json;
using System;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// the base class for a sensor in the phone
    /// </summary>
    public abstract class BaseServiceNode : IotNode, ISubscribe, INotify
    {
        /// <summary>
        /// get the status of the node
        /// </summary>
        public SensorStatusData Status
        {
            get
            {
                return UpdateSensorStatus();
            }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// change the status
        /// </summary>
        public void SetStatus(SensorRate rate, bool onOff)
        {
            SensorStatusData data = this.Status;
            if (data.SensorRate != rate || data.IsOn != onOff)
            {
                data.SensorRate = rate;
                if (data.IsOn && !onOff) StopSensor();
                else StartSensor(rate);
            }
        }

        /// <summary>
        /// whether the sensor is started
        /// </summary>
        public bool SensorStarted { get; private set; }

        /// <summary>
        /// start the sensor
        /// </summary>
        public virtual void Start()
        {
            try
            {
                if (!SensorStarted) SensorStarted = StartSensor(Status.SensorRate);
                UpdateSensorStatus();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        public virtual void Stop()
        {
            try
            {
                if (SensorStarted) SensorStarted = !StopSensor();
                UpdateSensorStatus();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        /// <summary>
        /// send notification to subscribed clients
        /// </summary>
        public void SendNotification()
        {
            foreach (INotify item in Children)
            {
                item.SendNotification();
            }
        }

        /// <summary>
        /// subscribe sensor data change notification
        /// </summary>
        /// <param name="targetAddress">the target http service's address to receive notification (ipaddress:port)</param>
        /// <param name="credential">the credential for http requests post to target service (user:password)</param>
        /// <param name="token">optional token to be included for every http requests</param>
        public void Subscribe(string targetAddress, string credential, string token)
        {
            HttpTargetSite endpoint = new HttpTargetSite() { Server = targetAddress, Credential = credential, Token = token };
            Subscribe(endpoint);
        }

        /// <summary>
        /// subscribe sensor data change notification
        /// </summary>
        public void Subscribe(HttpTargetSite endpoint)
        {
            foreach (ISubscribe item in Children)
            {
                item.Subscribe(endpoint);
            }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        [JsonIgnore]
        protected abstract bool IsOn { get; }

        /// <summary>
        /// start the sensor, return true if sensor started
        /// </summary>
        protected abstract bool StartSensor(SensorRate speed);

        /// <summary>
        /// stop the sensor, returns true if sensor stopped
        /// </summary>
        protected abstract bool StopSensor();

        /// <summary>
        /// constructor
        /// </summary>
        protected BaseServiceNode() : this(null, null) { }

        /// <summary>
        /// constructor
        /// </summary>
        protected BaseServiceNode(string id, IotNode parent, SensorRate speed = SensorRate.Medium)
            : base(id, parent)
        {
            Status = new SensorStatusData() { Id = "Status", Parent = this, SensorRate = speed, IsOn = false };
            UpsertData(Status);
        }

        /// <summary>
        /// update SensorStatusData with on/off state and timestamp
        /// </summary>
        /// <returns></returns>
        protected SensorStatusData UpdateSensorStatus()
        {
            SensorStatusData data = Data[nameof(Status)] as SensorStatusData;
            data.IsOn = IsOn;
            data.TimeStamp = DateTime.UtcNow;
            return data;
        }

        protected SensorSpeed ConvertSensorRate(SensorRate rate)
        {
            switch (rate)
            {
                case SensorRate.Lowest:
                case SensorRate.Low:
                case SensorRate.Medium:
                    return SensorSpeed.UI;
                case SensorRate.Default:
                    return SensorSpeed.Default;
                case SensorRate.High:
                    return SensorSpeed.Game;
                case SensorRate.Best:
                    return SensorSpeed.Fastest;
            }
            return SensorSpeed.UI;
        }
    }
}
