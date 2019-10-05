using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using static AcronisCyberCloudAPI.Instance;
using static AcronisCyberCloudAPI.InstanceUser;
using static AcronisCyberCloudAPI.Tenants;

namespace AcronisCyberCloudAPI
{
    public class Program
    {
        public static void Main()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            AdminUser adminUser = new AdminUser();
            string adminUserInfo = adminUser.GetAdminUserInfo(username, password);
            adminUser = JsonConvert.DeserializeObject<AdminUser>(adminUserInfo);
            string rootTenantId = adminUser.tenant_id;

            RootTenant rootTenant = new RootTenant();
            string rootTenantInfo = rootTenant.GetRootTenant(username, password, rootTenantId);
            rootTenant = JsonConvert.DeserializeObject<RootTenant>(rootTenantInfo);

            string newTenantJson = File.ReadAllText("templates/tenants.json");

            Tenant partnerTenant = new Tenant();
            partnerTenant = JsonConvert.DeserializeObject<Tenant>(newTenantJson);
            partnerTenant.name = "IVAYLO_TSVETKOV";
            partnerTenant.kind = "partner";
            partnerTenant.parent_id = rootTenant.items[0].id;

            string postData = JsonConvert.SerializeObject(partnerTenant);
            partnerTenant.PostTenant(username, password, postData);
        }
    }
}