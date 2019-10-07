using System;
using System.IO;
using System.Net;
using System.Text;

namespace AcronisCyberCloudAPI
{
    class TenantsInfo
    {
        public class TenantInfo
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
            public string customer_id { get; set; }
            public bool enabled { get; set; }
            public string kind { get; set; }
            public string customer_type { get; set; }
            public string default_idp_id { get; set; }
            public Contact contact { get; set; }
            public string id { get; set; }

            // GET method that gets a tenant's information by specified id and returns the response output.
            public string GetTenant(string username, string password, string id)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/tenants/" + id;
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

            // POST method that enables an application specified by application and tenants ids.
            public void EnableApplication(string username, string password, string appId, string id)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/applications/" + appId + "/bindings/tenants/" + id;
                string credentials = username + ":" + password;
                credentials = Convert.ToBase64String(Encoding.Default.GetBytes(credentials));

                WebRequest request = WebRequest.Create(url);
                request.Headers["Authorization"] = "Basic " + credentials;
                request.PreAuthenticate = true;
                request.Method = "POST";
                request.ContentType = "application/json";

                WebResponse response = request.GetResponse();
                response.Close();
            }

            // DELETE method that disables an application specified by application and tenants ids.
            public void DisableApplication(string username, string password, string appId, string id)
            {
                string url = "https://eu2-cloud.acronis.com:443/api/2/applications/" + appId + "/bindings/tenants/" + id;
                string credentials = username + ":" + password;
                credentials = Convert.ToBase64String(Encoding.Default.GetBytes(credentials));

                WebRequest request = WebRequest.Create(url);
                request.Headers["Authorization"] = "Basic " + credentials;
                request.PreAuthenticate = true;
                request.Method = "DELETE";
                request.ContentType = "application/json";

                WebResponse response = request.GetResponse();
                response.Close();
            }
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
            public string phone { get; set; }
            public string lastname { get; set; }
        }
    }
}