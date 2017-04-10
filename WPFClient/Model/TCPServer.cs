using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WPFClient.WCFRepositorySoapServiceReference;
using WPFClient.WCFRepositorySoapServiceReferenceAzure;


namespace WPFClient.Model
{
    public class TCPServer
    {
        public string SharedFolder { get; set; }
        public int ServerPort { get; private set; }
        public IPAddress ServerIpAddress { get; private set; }

        public TCPServer(string localSharedFolder, int serverPort)
        {
            string localHostName = Dns.GetHostName();
            //IPHostEntry s = Dns.GetHostEntry(localHostName);
            //ServerIpAddress = s.AddressList[0];

            ServerIpAddress = Dns.GetHostEntry(localHostName)
                .AddressList
                .First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            // IPv4 http://stackoverflow.com/questions/2048132/net-ipaddress-ipv4
            
            ServerPort = serverPort;
            SharedFolder = localSharedFolder;

            SetUpServer();
        }

        private void SetUpServer()
        {
            TcpListener tcpListener = new TcpListener(ServerIpAddress, ServerPort);
            //Task.Factory.StartNew(() => tcpListener.Start());
            tcpListener.Start();
            Trace.WriteLine("TcpListener: " + tcpListener.LocalEndpoint);
            RegisterFiles(SharedFolder, ServerIpAddress, ServerPort);
            Task.Run(() => RunServerPart(tcpListener, SharedFolder));
        }

        private void RegisterFiles(string sharedFolder, IPAddress serverIpAddress, int serverPort)
        {
            string[] files = Directory.GetFiles(sharedFolder);
            Trace.WriteLine($"Looking for files to add in {sharedFolder}");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = GetFileName(files[i]);
            }
            Trace.WriteLine($"Files found in shared folder: {files.ToList().Count.ToString()}");
            Trace.WriteLine("Trying to registrar:\n* " + string.Join("\n* ", files));
            using (WCFRepositorySoapServiceReferenceAzure.Service1Client client = new WCFRepositorySoapServiceReferenceAzure.Service1Client("BasicHttpBinding_IService11"))
            //using (WCFRepositorySoapServiceReference.Service1Client client = new WCFRepositorySoapServiceReference.Service1Client())
            {
                // Todo alternative: Use localHostName
                //string serverIp = ServerIpAddress.ToString();
                int howMany = client.AddAll(files, ServerIpAddress.ToString(), serverPort);
                Trace.WriteLine("Files registred: " + howMany);
            }
        }

        private string GetFileName(string path)
        {
            int index = path.LastIndexOf("/");
            return path.Substring(index + 1);
        }

        public void GetFile(string downloadFolder, string fileName)
        {
            Trace.WriteLine($"Trying to find locations for '{fileName}'");
            using (WCFRepositorySoapServiceReferenceAzure.Service1Client client = new WCFRepositorySoapServiceReferenceAzure.Service1Client("BasicHttpBinding_IService11"))
            //using (WCFRepositorySoapServiceReference.Service1Client client = new WCFRepositorySoapServiceReference.Service1Client())
            {
                try
                {
                    Task.Run(() => // to make the GUi more responsive
                    {
                        WCFRepositorySoapServiceReferenceAzure.Destination[] destinations = client.Get(fileName);
                        Trace.WriteLine("We have some locations " + destinations.Length);
                        if (destinations.Length == 0)
                        {
                            Trace.WriteLine("No locations");
                            return;
                        }
                        foreach (WCFRepositorySoapServiceReferenceAzure.Destination destination in destinations)
                        {
                            Trace.WriteLine(destination.Host + " " + destination.Port);
                        }
                        WCFRepositorySoapServiceReferenceAzure.Destination location = destinations[0];
                        Trace.WriteLine("Chosen location " + location.Host + " " + location.Port);
                        //string substring = location.Host.Substring(0, location.Host.Length - 2);
                        //Trace.WriteLine(substring);
                        using (TcpClient tcpClient = new TcpClient(location.Host, location.Port))
                        {
                            Stream stream = tcpClient.GetStream();
                            StreamWriter writer = new StreamWriter(stream);
                            writer.WriteLine(fileName);
                            writer.Flush();
                            //writer.Close();
                            FileStream fileStream = new FileStream(downloadFolder + "copy" + fileName, FileMode.Create);
                            stream.CopyTo(fileStream);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        private void RunServerPart(TcpListener tcpListener, string sharedFolder)
        {
            while (true)
            {
                Trace.WriteLine("waiting for client " + tcpListener.LocalEndpoint);
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Trace.WriteLine("new client connection " + tcpClient.Client.RemoteEndPoint);

                Stream stream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(stream);

                string request = reader.ReadLine();
                string filename = sharedFolder + request;

                try
                {
                    using (FileStream fileStream = new FileStream(filename, FileMode.Open))
                    {
                        string response = "200 OK\n";
                        byte[] buffer = Encoding.ASCII.GetBytes(response);
                        stream.Write(buffer, 0, buffer.Length);
                        fileStream.CopyTo(stream);
                        stream.Flush();
                    }
                }
                catch (FileNotFoundException)
                {
                    string response = "404 Not found: " + request + "\n";
                    byte[] buffer = Encoding.ASCII.GetBytes(response);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
                tcpClient.Close();
            }
        }

        public void RemoveAllFromIndex()
        {
            using (WCFRepositorySoapServiceReferenceAzure.Service1Client client = new WCFRepositorySoapServiceReferenceAzure.Service1Client("BasicHttpBinding_IService11"))
            //using (WCFRepositorySoapServiceReference.Service1Client client = new WCFRepositorySoapServiceReference.Service1Client())
            {
                int howMany = client.RemoveAll(ServerIpAddress.ToString(), ServerPort);
                Trace.WriteLine("Files registred: " + howMany);
            }
        }
    }
}
