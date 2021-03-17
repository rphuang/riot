using HttpLib;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Riot.Phone.Service
{
    /// <summary>
    /// Request handler to process /phone/subscribe requests
    /// </summary>
    public class SubscribeRequestHandler : HttpServiceRequestHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public SubscribeRequestHandler(PhoneService service, string name, string path, string credentials)
            : base(name, "SubscriptionData", path, credentials)
        {
            _service = service;
        }

        /// <summary>
        /// derived class must implement to handle Post request
        /// </summary>
        protected override HttpServiceResponse ProcessPostRequest(HttpServiceContext context)
        {
            SubscriptionData subscriptionData = null;
            string json = GetFromRequestStream(context.Context.Request);
            try
            {
                subscriptionData = JsonConvert.DeserializeObject<SubscriptionData>(json);
            }
            catch (Exception err)
            {
                return CreateResponseForBadRequest(context, Name, $"InvalidSubscription: {json}");
            }

            if (!ValidateSubscriptionData(subscriptionData))
            {
                return CreateResponseForBadRequest(context, Name);
            }

            HttpServiceResponse response = null;
            HttpTargetSite target = subscriptionData.Target;
            string invalidList = string.Empty;
            int okCount = 0;
            foreach (SubscriptionItem item in subscriptionData.Items)
            {
                bool ok = Subscribe(item, target);
                if (ok) okCount++;
                else
                {
                    if (!string.IsNullOrEmpty(invalidList)) invalidList += ", ";
                    invalidList += $"{item.Node}.{item.Data}";
                }
            }
            if (string.IsNullOrEmpty(invalidList)) response = CreateSuccessResponse(context, Name, 200, null);
            else
            {
                string invalidJson = $"{{\"subscriptionError\": \"{invalidList}\"}}";
                if (okCount > 0) response = CreateSuccessResponse(context, Name, 206, invalidJson);
                else CreateResponse(context, Name, false, 400, invalidJson);
            }
            return response;
        }

        private bool Subscribe(SubscriptionItem item, HttpTargetSite target)
        {
            bool ok = false;
            WireData data = null;
            BaseServiceNode serviceNode = _service.Children
                .Where((IIotNode node) => string.Equals(node.Id, item.Node, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()
                as BaseServiceNode;
            if (serviceNode != null)
            {
                if (string.IsNullOrEmpty(item.Data))
                {
                    serviceNode.Subscribe(target);
                    ok = true;
                }
                else
                {
                    data = serviceNode.GetData<WireData>(item.Data);
                    if (data != null)
                    {
                        data.Subscribe(target);
                        ok = true;
                    }
                }
            }
            return ok;
        }

        private bool ValidateSubscriptionData(SubscriptionData subscriptionData)
        {
            // todo
            return true;
        }

        private PhoneService _service;
    }
}
