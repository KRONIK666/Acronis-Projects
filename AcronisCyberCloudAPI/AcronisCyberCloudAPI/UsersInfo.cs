using System;

namespace AcronisCyberCloudAPI
{
    public class UsersInfo
    {
        public class UserInfo
        {
            public bool activated { get; set; }
            public object personal_tenant_id { get; set; }
            public string id { get; set; }
            public Contact contact { get; set; }
            public int version { get; set; }
            public string tenant_id { get; set; }
            public string[] notifications { get; set; }
            public bool terms_accepted { get; set; }
            public string login { get; set; }
            public bool enabled { get; set; }
            public string language { get; set; }
            public string[] business_types { get; set; }
            public string idp_id { get; set; }
            public DateTime created_at { get; set; }
        }

        public class Contact
        {
            public string city { get; set; }
            public string address1 { get; set; }
            public string state { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public string lastname { get; set; }
            public string firstname { get; set; }
            public string address2 { get; set; }
            public string zipcode { get; set; }
            public string country { get; set; }
        }
    }
}