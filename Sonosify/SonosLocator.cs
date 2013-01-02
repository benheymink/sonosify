using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sonosify
{
    public class SonosLocator
    {
        // SSDP Multicast address: (See http://en.wikipedia.org/wiki/Simple_Service_Discovery_Protocol )
        readonly IPAddress multicastAddress = IPAddress.Parse("239.255.255.250");
        const int multicastPort = 1900;

        // Time to wait for a response from multicast discovery
        const int searchTimeOut = 3000;

        const string messageHeader = "M-SEARCH * HTTP/1.1";
        const string messageHost = "HOST: 239.255.255.250:1900";
        const string messageMan = "MAN: \"ssdp:discover\"";
        const string messageMx = "MX: 8";
        const string messageSt = "ST: urn:schemas-upnp-org:device:ZonePlayer:1";

        readonly byte[] broadcastMessage = Encoding.UTF8.GetBytes(
            string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{0}",
                          "\r\n",
                          messageHeader,
                          messageHost,
                          messageMan,
                          messageMx,
                          messageSt));

        // The list of devices we'll discover
        public ObservableCollection<SonosDevice> Devices { get; set; }

        public SonosLocator()
        {
            Devices = new ObservableCollection<SonosDevice>();
        }

        public void CreateSonosListener()
        {
            // SSDP uses UDP transport:
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, 1901));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddress, IPAddress.Any));
                var thread = new Thread(() => GetSocketResponse(socket));
                socket.SendTo(broadcastMessage, 0, broadcastMessage.Length, SocketFlags.None, new IPEndPoint(multicastAddress, 1900));
                thread.Start();
                Thread.Sleep(searchTimeOut);
                socket.Close();
            }
        }

        public string GetLocation(string str)
        {
            if (str.StartsWith("HTTP/1.1 200 OK"))
            {
                var reader = new StringReader(str);
                var lines = new List<string>();
                for (; ; )
                {
                    var line = reader.ReadLine();
                    if (line == null) break;
                    if (line != "") lines.Add(line);
                }
                var location = lines.Where(lin => lin.ToLower().StartsWith("location:")).First();
                if (!string.IsNullOrEmpty(location) &&
                        (
                            Devices.Count == 0 ||
                            (from d in Devices where d.Location == location select d).FirstOrDefault() == null)
                        )
                {
                    // return location.Replace("LOCATION: ", "");
                    if (!location.Contains(".66"))
                    {
                        string temp = location.Remove(0, 10);
                        return temp.Remove(temp.IndexOf("/xml/"), 27);
                    }
                    else
                        return "";

                }
            }
            return "";
        }

        public void GetSocketResponse(Socket socket)
        {
            try
            {
                while (true)
                {
                    var response = new byte[8000];
                    EndPoint ep = new IPEndPoint(IPAddress.Any, multicastPort);
                    socket.ReceiveFrom(response, ref ep);
                    var str = Encoding.UTF8.GetString(response);

                    var location = GetLocation(str);
                    if (!string.IsNullOrEmpty(location))
                        Devices.Add(new SonosDevice() { Location = location });
                }
            }
            catch
            {
                // TODO: Handle exception from when connection is closed? It's ignored for now.
            }

        }
    }
}
