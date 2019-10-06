using System;
using System.IO;
using System.Net;
using System.Text;

namespace AcronisCyberCloudAPI
{
    public class InstanceUser
    {
        public class AdminUser
        {
            public string language { get; set; }
            public string idp_id { get; set; }
            public bool activated { get; set; }
            public string tenant_id { get; set; }
            public int version { get; set; }
            public bool terms_accepted { get; set; }
            public string login { get; set; }
            public string[] notifications { get; set; }
            public object[] business_types { get; set; }
            public object personal_tenant_id { get; set; }
            public DateTime created_at { get; set; }
            public bool enabled { get; set; }
            public Contact contact { get; set; }
            public string id { get; set; }

            public string GetAdminUserInfo(string username, string password)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/users/me";
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

        public class Contact
        {
            public object city { get; set; }
            public object state { get; set; }
            public object country { get; set; }
            public object address1 { get; set; }
            public object address2 { get; set; }
            public string firstname { get; set; }
            public string email { get; set; }
            public object zipcode { get; set; }
            public object phone { get; set; }
            public string lastname { get; set; }
        }
    }
}