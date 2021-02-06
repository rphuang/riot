using System;
using System.Collections.Generic;
using System.Linq;

namespace Riot
{
    /// <summary>
    /// base class for client nodes via http
    /// </summary>
    public class IotClientNode : IotNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IotClientNode(string id, IotHttpClient client, IotNode parent)
            : base(id, parent)
        {
            Client = client;
        }

        /// <summary>
        /// HttpClient used to communicate to server
        /// </summary>
        public IotHttpClient Client { get; protected set; }

        /// <summary>
        /// get raw json string from the server by sending http request to the server node
        /// </summary>
        public virtual string Get()
        {
            HttpResponse httpResponse = GetResponse();
            return httpResponse.Result;
        }

        /// <summary>
        /// get HttpResponse from the server for the specified path 
        /// </summary>
        public virtual HttpResponse GetResponse(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (PathDictionary == null)
                {
                    PathDictionary = new Dictionary<string, IotClientNode>();
                    BuildPathDictionary(PathDictionary, FullPath);
                }
                IotClientNode node;
                if (PathDictionary.TryGetValue(path, out node))
                {
                    return node.GetResponse();
                }
                else
                {
                    string pathLowercase = path.ToLower();
                    foreach (string key in PathDictionary.Keys)
                    {
                        if (pathLowercase.StartsWith(key+"/"))
                        {
                            return node.GetResponse();
                        }
                    }
                }
            }
            return GetResponse();
        }

        /// <summary>
        /// get HttpResponse from the server by sending http request to the server node
        /// </summary>
        public virtual HttpResponse GetResponse()
        {
            HttpResponse response = Client.GetResponse(FullPath);
            if (response.Success)
            {
                try
                {
                    ProcessResponse(response);
                }
                catch (Exception err)
                {
                    // force response's Success to be false
                    response.Success = false;
                    if (string.IsNullOrEmpty(response.ErrorMessage)) response.ErrorMessage = $"Error processing {FullPath}:\n{err.ToString()}";
                }
            }
            return response;
        }

        /// <summary>
        /// all client nodes must implement this to process the response from server
        /// </summary>
        /// <returns>returns true if the processing is successful.</returns>
        protected virtual bool ProcessResponse(HttpResponse response)
        {
            return false;
        }

        /// <summary>
        /// build path-node dictionary that trim the start of trimPath
        /// </summary>
        protected void BuildPathDictionary(Dictionary<string, IotClientNode> dictionary, string trimPath)
        {
            foreach (IotClientNode node in Children)
            {
                string path = node.FullPath;
                if (!string.IsNullOrEmpty(path))
                {
                    if (!string.IsNullOrEmpty(trimPath) && path.StartsWith(trimPath)) path = path.Substring(trimPath.Length + 1);
                    dictionary.Add(path.ToLower(), node);
                }
                node.BuildPathDictionary(dictionary, trimPath);
            }
        }

        protected static readonly char[] SlashDelimiter = { '/' };
        protected Dictionary<string, IotClientNode> PathDictionary { get; set; }
    }
}
