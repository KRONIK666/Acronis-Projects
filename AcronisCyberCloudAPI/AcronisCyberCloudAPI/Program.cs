﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using static AcronisCyberCloudAPI.Applications;
using static AcronisCyberCloudAPI.Instance;
using static AcronisCyberCloudAPI.InstanceUser;
using static AcronisCyberCloudAPI.Tenants;
using static AcronisCyberCloudAPI.TenantsInfo;
using static AcronisCyberCloudAPI.Users;
using static AcronisCyberCloudAPI.UsersInfo;

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
            string partnerTenantInfo = partnerTenant.PostTenant(username, password, postData);

            TenantInfo createdTenant = new TenantInfo();
            createdTenant = JsonConvert.DeserializeObject<TenantInfo>(partnerTenantInfo);

            Application applications = new Application();
            string applicationsInfo = applications.GetApplicationsInfo(username, password);
            applications = JsonConvert.DeserializeObject<Application>(applicationsInfo);

            for (int i = 0; i < applications.items.Length; i++)
            {
                if (applications.items[i].name == "Backup")
                {
                    createdTenant.EnableApplication(username, password, applications.items[i].id, createdTenant.id);
                }
                else if (applications.items[i].name == "File Sync & Share")
                {
                    createdTenant.DisableApplication(username, password, applications.items[i].id, createdTenant.id);
                }
            }

            string offeringJson = File.ReadAllText("templates/offering_items.json");

            OfferingItems offeringItems = new OfferingItems();
            offeringItems = JsonConvert.DeserializeObject<OfferingItems>(offeringJson);

            for (int i = 0; i < offeringItems.offering_items.Length; i++)
            {
                if (offeringItems.offering_items[i].name.Contains("o365"))
                {
                    offeringItems.offering_items[i].status = 0;
                }
            }

            string putData = JsonConvert.SerializeObject(offeringItems);
            offeringItems.EnableOfferingItems(username, password, createdTenant.id, putData);

            string newPartnerUser = File.ReadAllText("templates/user.json");

            User partnerUser = new User();
            partnerUser = JsonConvert.DeserializeObject<User>(newPartnerUser);
            partnerUser.tenant_id = createdTenant.id;

            string postUserData = JsonConvert.SerializeObject(partnerUser);
            string partnerUserInfo = partnerUser.PostUser(username, password, postUserData);

            UserInfo createdPartnerUser = new UserInfo();
            createdPartnerUser = JsonConvert.DeserializeObject<UserInfo>(partnerUserInfo);

            string newPartnerRole = File.ReadAllText("templates/user_roles.json");

            Roles partnerUserRole = new Roles();
            partnerUserRole = JsonConvert.DeserializeObject<Roles>(newPartnerRole);
            partnerUserRole.items[0].trustee_id = createdPartnerUser.id;
            partnerUserRole.items[0].tenant_id = createdPartnerUser.tenant_id;
            partnerUserRole.items[0].role_id = "partner_admin";

            string putUserData = JsonConvert.SerializeObject(partnerUserRole);
            partnerUserRole.PutAccessPolicies(username, password, createdPartnerUser.id, putUserData);
        }
    }
}