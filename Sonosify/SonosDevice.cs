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
        public string Location { get; set; }
        public string Name { get; set; }

        public void Play(string mediaURL)
        {
            string action = "urn:schemas-upnp-org:service:AVTransport:1#SetAVTransportURI";
            string body = "<u:SetAVTransportURI xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><CurrentURI>" + mediaURL + "</CurrentURI><CurrentURIMetaData></CurrentURIMetaData></u:SetAVTransportURI>";

            string soap = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>" + body + "</s:Body></s:Envelope>";

            HttpWebRequest httpWReq =
                (HttpWebRequest)WebRequest.Create(Location + "/MediaRenderer/AVTransport/Control");

            httpWReq.ContentType = "string/xml";
            httpWReq.Headers["SOAPACTION"] = action;

            ASCIIEncoding encoding = new ASCIIEncoding();
            string postData = soap;
            byte[] data = encoding.GetBytes(postData);

            httpWReq.Method = "POST";
            httpWReq.ContentLength = data.Length;

            using (Stream newStream = httpWReq.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
            }

            WebResponse resp = httpWReq.GetResponse();


            HttpWebRequest httpWReq2 =
                (HttpWebRequest)WebRequest.Create(Location + "/MediaRenderer/AVTransport/Control");
            string action2 = "urn:schemas-upnp-org:service:AVTransport:1#Play";
            string body2 = "<u:Play xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID><Speed>1</Speed></u:Play>";
            string soap2 = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>" + body2 + "</s:Body></s:Envelope>";
            httpWReq2.Headers["SOAPACTION"] = action2;

            ASCIIEncoding encoding2 = new ASCIIEncoding();
            string postData2 = soap2;
            byte[] data2 = encoding.GetBytes(postData2);

            httpWReq2.Method = "POST";
            httpWReq2.ContentLength = data2.Length;

            using (Stream newStream = httpWReq2.GetRequestStream())
            {
                newStream.Write(data2, 0, data2.Length);
            }

            WebResponse resp2 = httpWReq2.GetResponse();
        }
    }
}
