using HttpLib;
using Newtonsoft.Json;
using System;

namespace Riot.Phone.Service
{
    /// <summary>
    /// Request handler to process phone's action requests
    /// </summary>
    public class ActionRequestHandler<T> : HttpServiceRequestHandler where T : IotData
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ActionRequestHandler(BaseActionService service, string name, string path, string credentials)
            : base(name, nameof(T), path, credentials)
        {
            _service = service;
        }

        /// <summary>
        /// derived class must implement to handle Post request
        /// </summary>
        protected override HttpServiceResponse ProcessPostRequest(HttpServiceContext context)
        {
            T data = null;
            string json = GetFromRequestStream(context.Context.Request);
            try
            {
                data = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception err)
            {
                return CreateResponseForBadRequest(context, Name, $"InvalidData: {json}");
            }

            if (_service.RequireUiThread)
            {
                UiActionBag.Instance.AddAction(_service, data);
                return CreateSuccessResponse(context, Name, 200, $"{Path} ActionScheduled");
            }
            if (_service.Act(data))
            {
                return CreateSuccessResponse(context, Name, 200, $"{Path} ActionProcessed");
            }

            return CreateResponseForBadRequest(context, Name, $"{Path} Action Failed");
        }

        protected BaseActionService _service;
    }
}
