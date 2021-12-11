using CapaEntidad.PasarelaPago;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace CapaPresentacion.Api
{
    public class RestServicesApi
    {
        public TResult PostInvoque<T, TResult>(T obj, string detailUrlKey, string token, string typeRequest, string nameMethod)
        {
            TResult result;
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                var request = WebRequest.Create(detailUrlKey) as HttpWebRequest;
                request.Method = typeRequest;                
                ASCIIEncoding encoder = new ASCIIEncoding();

                request.Headers = GetHeaders(false ? ParameterHeaderOsb(obj, token) : ParameterHeader(obj, token));
                string data = JsonConvert.SerializeObject(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                request.ContentLength = byteArray.Length;
                request.Expect = "application/json";
                request.Accept = "application/json";
                request.ContentType = "application/json";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse; 
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    var responseString = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<TResult>(responseString);
                }
            }
            catch (Exception ex)
            {
                var exception = string.Empty;
                TResult expcetion_response = JsonConvert.DeserializeObject<TResult>(exception);
                return expcetion_response;
            }

            return result;
        }

        public TResult PostWebhooks<obj, TResult>(obj request)
        {
            TResult result;
            try
            {               
                string data = JsonConvert.SerializeObject(request);
                result = JsonConvert.DeserializeObject<TResult>(data);
            }
            catch (Exception ex)
            {
                var exception = string.Empty;
                TResult expcetion_response = JsonConvert.DeserializeObject<TResult>(exception);
                return expcetion_response;
            }

            return result;
        }

        public TResult Authentication<T, TResult>(T obj, string detailUrlKey, string token, string typeRequest, string nameMethod)
        {
            TResult result;
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                var request = WebRequest.Create(detailUrlKey) as HttpWebRequest;
                request.Method = typeRequest;
                ASCIIEncoding encoder = new ASCIIEncoding();

                string data = JsonConvert.SerializeObject(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                request.Timeout = Convert.ToInt32(50000);
                request.Expect = "application/json";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    var responseString = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<TResult>(responseString); 
                }
            }
            catch (Exception ex)
            {
                var exception = string.Empty;
                TResult expcetion_response = JsonConvert.DeserializeObject<TResult>(exception);
                return expcetion_response;
            }

            return result;
        }

        private WebHeaderCollection GetHeaders(Hashtable table)
        {
            WebHeaderCollection Headers = new WebHeaderCollection();
            foreach (DictionaryEntry entry in table)
            {
                Headers.Add(entry.Key.ToString(), entry.Value != null ? entry.Value.ToString() : null);
            }
            return Headers;
        }

        private Hashtable ParameterHeaderOsb(object Obj, string Token)
        {
            var paramHeaders = new Hashtable();
            paramHeaders.Add("Authorization", "Bearer " + Token);
            return paramHeaders;
        }

        private Hashtable ParameterHeader(object Obj, string Token)
        {
            var paramHeaders = new Hashtable();
            string data = JsonConvert.SerializeObject(Obj);
            paramHeaders.Add("Authorization", "Bearer  " + Token);
            return paramHeaders;
        }
    }
}