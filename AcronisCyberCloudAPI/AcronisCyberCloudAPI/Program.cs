using Newtonsoft.Json;
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
            // Enter credentials for Basic Authentication.
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            // Setting security protocol and timeout for all server requests.
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Get information about the admin user of the Cloud instace.
            AdminUser adminUser = new AdminUser();
            string adminUserInfo = adminUser.GetAdminUserInfo(username, password);
            adminUser = JsonConvert.DeserializeObject<AdminUser>(adminUserInfo);

            // Get the root tenant uuid from the admin user info.
            string rootTenantId = adminUser.tenant_id;

            // Get the information for the root tenant of the Cloud instance.
            RootTenant rootTenant = new RootTenant();
            string rootTenantInfo = rootTenant.GetRootTenant(username, password, rootTenantId);
            rootTenant = JsonConvert.DeserializeObject<RootTenant>(rootTenantInfo);

            // Load the json template for creating a new tenant.
            string tenantJson = File.ReadAllText("../../../templates/tenants.json");

            // Create new partner tenant under the root tenant of the Cloud instance.
            Tenant partnerTenant = new Tenant();
            partnerTenant = JsonConvert.DeserializeObject<Tenant>(tenantJson);
            partnerTenant.name = "IVAYLO_TSVETKOV";
            partnerTenant.kind = "partner";
            partnerTenant.parent_id = rootTenant.items[0].id;

            string postData = JsonConvert.SerializeObject(partnerTenant);
            string partnerTenantInfo = partnerTenant.PostTenant(username, password, postData);

            // Store the info of the created partner into an object.
            TenantInfo createdPartner = new TenantInfo();
            createdPartner = JsonConvert.DeserializeObject<TenantInfo>(partnerTenantInfo);

            // Get the information of all applications and store it into an object.
            Application applications = new Application();
            string applicationsInfo = applications.GetApplicationsInfo(username, password);
            applications = JsonConvert.DeserializeObject<Application>(applicationsInfo);

            // Declare variables for storing the activated applications.
            string activatedApplications = "{\"items\": [";
            string tempApp;

            // Enable "Backup" application and disable "File Sync & Share" application.
            // Looping all applications to find the required ones for purpose of the given task.
            // Storing all activated applications into the activatedApplications variable for later use.
            for (int i = 0; i < applications.items.Length; i++)
            {
                if (applications.items[i].name == "Backup")
                {
                    createdPartner.EnableApplication(username, password, applications.items[i].id, createdPartner.id);
                    tempApp = JsonConvert.SerializeObject(applications.items[i]);
                    activatedApplications = activatedApplications + tempApp + ",";
                }
                else if (applications.items[i].name == "File Sync & Share")
                {
                    createdPartner.DisableApplication(username, password, applications.items[i].id, createdPartner.id);
                }
            }

            // Trimming the trailing character and concatenate additional characters to build the json.
            activatedApplications = activatedApplications.Remove(activatedApplications.Length - 1);
            activatedApplications = activatedApplications + "]}";

            // Load the json template for enabling of the offering items.
            string offeringItemsJson = File.ReadAllText("../../../templates/offering_items.json");

            // Instantiate offering items object and deserialize the json template.
            OfferingItems offeringItems = new OfferingItems();
            offeringItems = JsonConvert.DeserializeObject<OfferingItems>(offeringItemsJson);

            // Declare variable for storing the enabled offering items.
            string activatedOfferingItems = "{\"offering_items\": [";

            // Looping all offering items and set enabled/disabled status according to task requirements.
            // Storing all offering items into the activatedOfferingItems variable for later use.
            for (int i = 0; i < offeringItems.offering_items.Length; i++)
            {
                if (offeringItems.offering_items[i].name.Contains("o365"))
                {
                    offeringItems.offering_items[i].status = 0;
                }
                else
                {
                    tempApp = JsonConvert.SerializeObject(offeringItems.offering_items[i]);
                    activatedOfferingItems = activatedOfferingItems + tempApp + ",";
                }
            }

            // Trimming the trailing character and concatenate additional characters to build the json.
            activatedOfferingItems = activatedOfferingItems.Remove(activatedOfferingItems.Length - 1);
            activatedOfferingItems = activatedOfferingItems + "]}";

            // Enable the offering items according to the configured status.
            string putData = JsonConvert.SerializeObject(offeringItems);
            offeringItems.EnableOfferingItems(username, password, createdPartner.id, putData);

            // Load the json template for creating of a new user.
            string userJson = File.ReadAllText("../../../templates/user.json");

            // Creating a new user under the created partner tenant.
            User partnerUser = new User();
            partnerUser = JsonConvert.DeserializeObject<User>(userJson);
            partnerUser.tenant_id = createdPartner.id;

            postData = JsonConvert.SerializeObject(partnerUser);
            string partnerUserInfo = partnerUser.PostUser(username, password, postData);

            // Store the info of the created partner user into an object.
            UserInfo createdPartnerUser = new UserInfo();
            createdPartnerUser = JsonConvert.DeserializeObject<UserInfo>(partnerUserInfo);

            // Load the json template for assigning role on the user.
            string newUserRole = File.ReadAllText("../../../templates/user_roles.json");

            // Setting partner_admin role on the created partner user.
            Roles partnerUserRole = new Roles();
            partnerUserRole = JsonConvert.DeserializeObject<Roles>(newUserRole);
            partnerUserRole.items[0].trustee_id = createdPartnerUser.id;
            partnerUserRole.items[0].tenant_id = createdPartnerUser.tenant_id;
            partnerUserRole.items[0].role_id = "partner_admin";

            putData = JsonConvert.SerializeObject(partnerUserRole);
            partnerUserRole.PutAccessPolicies(username, password, createdPartnerUser.id, putData);

            // Create new end-user/customer tenant under the partner tenant.
            Tenant customerTenant = new Tenant();
            customerTenant = JsonConvert.DeserializeObject<Tenant>(tenantJson);
            customerTenant.name = "End-User";
            customerTenant.kind = "customer";
            customerTenant.parent_id = createdPartner.id;

            // Post the tenant and store the info of the created customer into an object.
            postData = JsonConvert.SerializeObject(customerTenant);
            string customerTenantInfo = customerTenant.PostTenant(username, password, postData);

            TenantInfo createdCustomer = new TenantInfo();
            createdCustomer = JsonConvert.DeserializeObject<TenantInfo>(customerTenantInfo);

            // Inherit the activated applications from the partner tenant to the customer tenant.
            applications = JsonConvert.DeserializeObject<Application>(activatedApplications);
            for (int i = 0; i < applications.items.Length; i++)
            {
                createdCustomer.EnableApplication(username, password, applications.items[i].id, createdCustomer.id);
            }

            // Inherit the activated offering items from the partner tenant to the customer tenant.
            offeringItems.EnableOfferingItems(username, password, createdCustomer.id, activatedOfferingItems);

            // Load the json template for creating of a backup user.
            string backupUserJson = File.ReadAllText("../../../templates/backup_user.json");

            // Creating a backup user under the created customer tenant.
            User backupUser = new User();
            backupUser = JsonConvert.DeserializeObject<User>(backupUserJson);
            backupUser.tenant_id = createdCustomer.id;
            backupUser.contact.email = "backup.user@users.com";
            backupUser.login = backupUser.contact.email;
            backupUser.contact.firstname = "Backup";
            backupUser.contact.lastname = "User";

            postData = JsonConvert.SerializeObject(backupUser);
            string customerUserInfo = partnerUser.PostUser(username, password, postData);

            // Store the info of the created backup user into an object.
            UserInfo createdBackupUser = new UserInfo();
            createdBackupUser = JsonConvert.DeserializeObject<UserInfo>(customerUserInfo);

            // Setting backup_user role on the created backup user.
            Roles backupUserRole = new Roles();
            backupUserRole = JsonConvert.DeserializeObject<Roles>(newUserRole);
            backupUserRole.items[0].trustee_id = createdBackupUser.id;
            backupUserRole.items[0].tenant_id = createdBackupUser.tenant_id;
            backupUserRole.items[0].role_id = "backup_user";
            backupUserRole.items[0].trustee_type = "user";
            backupUserRole.items[0].version = 0;

            putData = JsonConvert.SerializeObject(backupUserRole);
            backupUserRole.PutAccessPolicies(username, password, createdBackupUser.id, putData);
        }
    }
}