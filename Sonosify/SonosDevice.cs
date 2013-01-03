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

        public string Location { get; set; }
        public string Name { get; set; }

        private string GenerateSoapEnvelope(string soapBody)
        {
            return string.Format("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>{0}</s:Body></s:Envelope>", soapBody);
        }

        public void Queue(string mediaURL)
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#SetAVTransportURI";
            string body = "<u:SetAVTransportURI xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><CurrentURI>" + mediaURL + "</CurrentURI><CurrentURIMetaData></CurrentURIMetaData></u:SetAVTransportURI>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(TRANSPORT_ENDPOINT, action, soap);
        }

        public void Play()
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#Play";
            string body = "<u:Play xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><Speed>1</Speed></u:Play>";
            string soap = GenerateSoapEnvelope(body);

            WebResponse cmdResponse = SendCommand(TRANSPORT_ENDPOINT, action, soap);
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
