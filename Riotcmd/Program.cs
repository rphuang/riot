using Riot;
using Riot.Pi.Client;
using Riot.SmartPlug.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pi
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = null;
            string credential = string.Empty;
            bool discover = false;
            bool helpOnly = false;
            bool test = false;
            int jj;
            for (jj = 0; jj < args.Length; jj++)
            {
                string arg = args[jj];
                if (arg[0] == '/' || arg[0] == '-')
                {
                    switch (Char.ToUpper(arg[1]))
                    {
                        case 'A':
                            jj++;
                            credential = args[jj];
                            break;
                        case 'D':
                            discover = true;
                            break;
                        case 'S':
                            jj++;
                            server = args[jj];
                            break;
                        case 'T':
                            test = true;
                            break;
                        case '?':
                        case 'H':
                            helpOnly = true;
                            break;
                        default:
                            Console.WriteLine("Oops! the switch is not supported {0}", arg);
                            helpOnly = true;
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            if (args.Length == 0 || helpOnly)
            {
                ShowHelp();
            }
            else
            {
                for (; jj < args.Length; jj++)
                {
                    if (discover) Discover(args[jj], credential);
                    else if (test) TestAllEndpoints(args[jj], credential);
                    else ProcessRequest(server, credential, args[jj]);
                }
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Riot command and test program");
            Console.WriteLine("pi [/a user:password] /d server:port [server:port ...]");
            Console.WriteLine("pi [/a user:password] server:port[/urlpath] [server:port[/urlpath] ...]");
            Console.WriteLine("pi [/a user:password] /s server:port urlpath [urlpath ...]");
            Console.WriteLine("/d - discover endpoints from server without creating client nodes");
            Console.WriteLine("/a user:password - user and password for the http requests");
            Console.WriteLine("/s server:port - server and port for the http requests");
            Console.WriteLine("/t - test by going through all endpoints discovered");

            Console.WriteLine("Examples:");
            Console.WriteLine(@"pi /a user1:password1 192.168.0.33:8008/sys");
            Console.WriteLine(@"pi /a user1:password1 /s 192.168.0.33:8008 sys gpio");
            Console.WriteLine(@"pi /a user1:password1 /d 192.168.0.33:8008 192.168.0.110:8008");
            Console.WriteLine(@"pi /a user1:password1 /t 192.168.0.33:8008");
        }

        static void Discover(string server, string credential)
        {
            IotHttpClient httpIotClient = new IotHttpClient();
            httpIotClient.SetServer(server);
            httpIotClient.SetCredential(credential);

            // discover server endpoints and paths
            IList<HttpEndpoint> endpoints = httpIotClient.DiscoverAvailableEndpoints();
            if (endpoints?.Count > 0)
            {
                Console.WriteLine($"Server {server} endpoints:");
                foreach (HttpEndpoint endpoint in endpoints)
                {
                    Console.WriteLine($" name:{endpoint.Name} type:{endpoint.Type} path:{endpoint.Path} parent:{endpoint.Parent} url:{endpoint.Url}");
                }
            }
            else
            {
                Console.WriteLine($"Server {server} does not support REST IOT.");
            }
        }

        static void ProcessRequest(string server, string credential, string serverPath)
        {
            if (string.IsNullOrEmpty(server) && string.IsNullOrEmpty(serverPath))
            {
                Console.WriteLine("Missing command path");
                return;
            }

            string path = serverPath;
            if (!string.IsNullOrEmpty(serverPath) && string.IsNullOrEmpty(server))
            {
                // path format is "192.168.0.10:8000/p1/p2/p3"
                string[] parts = serverPath.Split(SlashDelimiter, 2);
                server = parts[0];
                if (parts.Length > 1) path = parts[1];
                else path = null;
            }

            if (string.IsNullOrEmpty(path))
            {
                Discover(server, credential);
                return;
            }

            // create client factories
            PiClientFactory pi = new PiClientFactory();
            SmartPlugClientFactory plug = new SmartPlugClientFactory();

            // discover server endpoints and create client nodes
            IotClientNode rootNode = IotClientFactory.Discover(server, credential);
            if (rootNode?.Children.Count <= 0)
            {
                Console.WriteLine($"Server {server} does not support REST IOT.");
                return;
            }

            HttpResponse response = rootNode.GetResponse(path);
            if (response.Success) LogUtil.WritePassed("Server {0} response for endpoint {1}:\n{2}", server, path, response.Result);
            else Console.WriteLine("Error from Server {0} response for endpoint {1}:\n{2}\n{3}", server, path, response.Result, response.ErrorMessage);
        }

        static void TestAllEndpoints(string server, string credential)
        {
            // create client factories
            PiClientFactory pi = new PiClientFactory();
            SmartPlugClientFactory plug = new SmartPlugClientFactory();

            // discover server endpoints and create client nodes
            IotClientNode rootNode = IotClientFactory.Discover(server, credential);
            foreach (IotClientNode node in rootNode.Children)
            {
                TestNode(node);
            }
        }

        static void TestNode(IotClientNode node)
        {
            LogUtil.WriteAction($"==== Test: {node.FullPath}");
            HttpResponse response = node.GetResponse();
            if (response.Success) LogUtil.WritePassed("Server response:\n{0}", response.Result);
            else LogUtil.WriteFailed("Error response:\n{0}\n{1}", response.Result, response.ErrorMessage);
            foreach (IotClientNode child in node.Children)
            {
                TestNode(child);
            }
        }

        private static readonly char[] SlashDelimiter = { '/' };
    }
}
