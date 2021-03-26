using HttpLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Services
{
    /// <summary>
    /// defines the interface to receive the response for each request from AppServiceHost
    /// </summary>
    interface IResponseHandler
    {
        /// <summary>
        /// process the response after a request
        /// </summary>
        void Process(HttpServiceResponse response);
    }
}
