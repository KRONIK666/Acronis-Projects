using System;
using System.IO;
using System.Net;
using System.Text;

namespace AcronisCyberCloudAPI
{
    public class Instance
    {
        public class RootTenant
        {
            public Item[] items { get; set; }

            public string GetRootTenant(string username, string password, string id)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/tenants?uuids=" + id;
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

            public class Item
            {
                public string parent_id { get; set; }
                public string language { get; set; }
                public bool ancestral_access { get; set; }
                public object owner_id { get; set; }
                public int version { get; set; }
                public object internal_tag { get; set; }
                public bool has_children { get; set; }
                public Update_Lock update_lock { get; set; }
                public string name { get; set; }
                public int brand_id { get; set; }
                public object customer_id { get; set; }
                public bool enabled { get; set; }
                public string kind { get; set; }
                public string customer_type { get; set; }
                public string default_idp_id { get; set; }
                public Contact contact { get; set; }
                public string id { get; set; }
            }

            public class Update_Lock
            {
                public bool enabled { get; set; }
                public object owner_id { get; set; }
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
}