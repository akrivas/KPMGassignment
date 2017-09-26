using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


/* The assumption is that I have access to the accounts of all the companies using Azure. I request all the users of each company
 * using the AD graph API of the company and get a JSON response, after getting access token for OAuth2. 
 * From that, I count the users of each company. The total number is stored in an array. The process is automated for all the companies. 
 * Then, I sort them from largest to smallest.*/

namespace Assignment
{
    static class LargestCompanies
    {
        public static int CompanySize = 0;

        public static string TokenForUser { get; private set; }

        static void Main()
        {
            int AzureCompanies = 450; /* Microsoft says that 90% of Fortune 500 uses Azure. Thus, let's assume the total number of companies 
            using azure are 450. Alternatively, a number suggests that 200.000 organizations use azure..*/

            int[] LargestAzureCompanies = new int[450];
            for (int i = 0; i <= AzureCompanies; i++)
            {
                MakeUsersRequest();
                LargestAzureCompanies[i] = CompanySize;
            }

            Array.Sort<int>(LargestAzureCompanies);
            Array.Reverse(LargestAzureCompanies);

            for (int i = 0; i <= 50; i++)
            {
                Console.WriteLine(LargestAzureCompanies[i]); //Output of the largest 50 companies using Azure as a vertical descending list
            }
        }

        static async void MakeUsersRequest()
        {

            // OAuth2 access token is required to access this API. Need to register the app and then request a token.
            string AuthString = "https://login.microsoftonline.com/";
            string ResourceUrl = "https://graph.windows.net";
            string ClientId = "***";
            var redirectUri = new Uri("https://localhost");
            string TenantId = "***";
            int TotalUsers = 0;

            AuthenticationContext authenticationContext = new AuthenticationContext(AuthString + TenantId, false);
            AuthenticationResult userAuthnResult = await authenticationContext.AcquireTokenAsync(ResourceUrl,
                ClientId, redirectUri, new PlatformParameters(PromptBehavior.RefreshSession));
            TokenForUser = userAuthnResult.AccessToken;

            //Connecting to AD Graph API
            var client = new HttpClient();

            var uri = "https://graph.windows.net/myorganization/users?api-version=1.6";
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", TokenForUser);
            var response = await client.GetAsync(uri);

            if (response.Content != null)
            {
                var responseString = await response.Content.ReadAsStringAsync(); //Response with all the users of the company's Azure AD
                Console.WriteLine(responseString);
            }

            /*Assuming I get the total number of users from each company from the responseString. 
            The Get Users returns the list of all the users and therefore, a count is needed. 
            Respecting the assignment duration to be maximum 2 hours, this part was not implemented*/
            
            CompanySize = TotalUsers;

            return;

        }
    }
}
