using System;
using System.IO;
using System.Net;
using System.Text;

namespace AcronisCyberCloudAPI
{
    public class Tenants
    {
        public class Tenant
        {
            public string name { get; set; }
            public string parent_id { get; set; }
            public string kind { get; set; }
            public Contact contact { get; set; }
            public string customer_id { get; set; }
            public object internal_tag { get; set; }
            public string language { get; set; }

            public string PostTenant(string username, string password, string postData)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/tenants";
                string credentials = username + ":" + password;
                credentials = Convert.ToBase64String(Encoding.Default.GetBytes(credentials));

                WebRequest request = WebRequest.Create(url);
                request.Headers["Authorization"] = "Basic " + credentials;
                request.PreAuthenticate = true;
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseFromServer = reader.ReadToEnd();
                response.Close();

                return responseFromServer;
            }
        }

        public class Contact
        {
            public string email { get; set; }
            public string address { get; set; }
            public string phone { get; set; }
        }
    }
}