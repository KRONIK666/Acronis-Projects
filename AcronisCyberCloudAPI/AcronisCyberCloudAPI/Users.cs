using System;
using System.IO;
using System.Net;
using System.Text;

namespace AcronisCyberCloudAPI
{
    public class Users
    {

        public class User
        {
            public object tenant_id { get; set; }
            public string login { get; set; }
            public Contact contact { get; set; }
            public bool enabled { get; set; }
            public string language { get; set; }
            public string[] business_types { get; set; }
            public string[] notifications { get; set; }

            public string PostUser(string username, string password, string postData)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/users";
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
            public string address1 { get; set; }
            public string address2 { get; set; }
            public string country { get; set; }
            public string state { get; set; }
            public string zipcode { get; set; }
            public string city { get; set; }
            public string phone { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
        }

        public class Roles
        {
            public Item[] items { get; set; }

            public void PutAccessPolicies(string username, string password, string id, string putData)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/users/" + id + "/access_policies";
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

        public class Item
        {
            public string id { get; set; }
            public string issuer_id { get; set; }
            public object trustee_id { get; set; }
            public string trustee_type { get; set; }
            public object tenant_id { get; set; }
            public object role_id { get; set; }
            public Resource resource { get; set; }
            public int version { get; set; }
        }

        public class Resource
        {
            public string resource_id { get; set; }
            public string resource_server_id { get; set; }
            public string scope_type { get; set; }
        }
    }
}