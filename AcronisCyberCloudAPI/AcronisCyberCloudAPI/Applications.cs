using System;
using System.IO;
using System.Net;
using System.Text;

namespace AcronisCyberCloudAPI
{
    public class Applications
    {
        public class Application
        {
            public Item[] items { get; set; }

            public string GetApplicationsInfo(string username, string password)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/applications";
                string credentials = username + ":" + password;
                credentials = Convert.ToBase64String(Encoding.Default.GetBytes(credentials));

                WebRequest request = WebRequest.Create(url);
                request.Headers["Authorization"] = "Basic " + credentials;
                request.PreAuthenticate = true;
                request.Method = "GET";
                request.ContentType = "application/json";

                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseFromServer = reader.ReadToEnd();
                response.Close();

                return responseFromServer;
            }
        }

        public class Item
        {
            public string type { get; set; }
            public string api_base_url { get; set; }
            public string name { get; set; }
            public string[] usages { get; set; }
            public string id { get; set; }
        }

        public class OfferingItems
        {
            public Offering_Items[] offering_items { get; set; }

            public void EnableOfferingItems(string username, string password, string id, string putData)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/tenants/" + id + "/offering_items";
                string credentials = username + ":" + password;
                credentials = Convert.ToBase64String(Encoding.Default.GetBytes(credentials));

                WebRequest request = WebRequest.Create(url);
                request.Headers["Authorization"] = "Basic " + credentials;
                request.PreAuthenticate = true;
                request.Method = "PUT";

                byte[] byteArray = Encoding.UTF8.GetBytes(putData);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                WebResponse response = request.GetResponse();
                response.Close();
            }
        }

        public class Offering_Items
        {
            public string application_id { get; set; }
            public string name { get; set; }
            public int status { get; set; }
        }
    }
}