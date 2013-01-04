using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sonosify
{
    public class SonosDevice
    {
        private const string TRANSPORT_ENDPOINT = "/MediaRenderer/AVTransport/Control";
        private const string RENDERING_ENDPOINT = "/MediaRenderer/RenderingControl/Control";
        private const string DEVICE_ENDPOINT = "/DeviceProperties/Control";


        /// <summary>
        /// The location (IP address) of the device
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The friendly-name of the device
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Wraps the passed in body in a s:envelope wrapper
        /// </summary>
        /// <param name="soapBody">The soap body to wrap</param>
        /// <returns></returns>
        private string GenerateSoapEnvelope(string soapBody)
        {
            return string.Format("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>{0}</s:Body></s:Envelope>", soapBody);
        }

        /// <summary>
        /// Queues a song for playing. You must call Play after queueing a song to actually play the song however.
        /// </summary>
        /// <param name="mediaURL">The URL of the media to play</param>
        public void Queue(string mediaURL)
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#SetAVTransportURI";
            string body = "<u:SetAVTransportURI xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><CurrentURI>" + mediaURL + "</CurrentURI><CurrentURIMetaData></CurrentURIMetaData></u:SetAVTransportURI>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(TRANSPORT_ENDPOINT, action, soap);
        }

        /// <summary>
        /// Starts playing the currently queued track
        /// </summary>
        public void Play()
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#Play";
            string body = "<u:Play xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><Speed>1</Speed></u:Play>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(TRANSPORT_ENDPOINT, action, soap);
        }

        /// <summary>
        /// Pause the currently playing track
        /// </summary>
        public void Pause()
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#Pause";
            string body = "<u:Pause xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><Speed>1</Speed></u:Pause>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(TRANSPORT_ENDPOINT, action, soap);
        }

        /// <summary>
        /// Stops the currently playing track
        /// </summary>
        public void Stop()
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#Stop";
            string body = "<u:Stop xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><Speed>1</Speed></u:Stop>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(TRANSPORT_ENDPOINT, action, soap);
        }

        /// <summary>
        /// Move to next song
        /// </summary>
        public void Next()
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#Next";
            string body = "<u:Next xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><Speed>1</Speed></u:Next>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(TRANSPORT_ENDPOINT, action, soap);
        }

        /// <summary>
        /// Move to previous song
        /// </summary>
        public void Previous()
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#Previous";
            string body = "<u:Previous xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><Speed>1</Speed></u:Previous>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(TRANSPORT_ENDPOINT, action, soap);
        }

        /// <summary>
        /// Mute or unmute the device
        /// </summary>
        /// <param name="mute">True to mute, False to un-mute</param>
        public void Mute(bool mute)
        {
            string muteValue = mute ? "1" : "0";
            string action = "urn:schemas-upnp-org:service:RenderingControl:1#SetMute";
            string body = "<u:SetMute xmlns:u=\"urn:schemas-upnp-org:service:RenderingControl:1\"><InstanceID>0</InstanceID><Channel>Master</Channel><DesiredMute>" + muteValue + "</DesiredMute></u:SetMute>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(RENDERING_ENDPOINT, action, soap);
        }

        private WebResponse SendCommand(string endpoint, string action, string soapBody)
        {
            HttpWebRequest httpWReq =
                (HttpWebRequest)WebRequest.Create(Location + endpoint);

            httpWReq.ContentType = "string/xml";
            httpWReq.Headers["SOAPACTION"] = action;

            ASCIIEncoding encoding = new ASCIIEncoding();
            string postData = soapBody;
            byte[] data = encoding.GetBytes(postData);

            httpWReq.Method = "POST";
            httpWReq.ContentLength = data.Length;

            using (Stream newStream = httpWReq.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
            }

            return httpWReq.GetResponse();
        }
    }
}
