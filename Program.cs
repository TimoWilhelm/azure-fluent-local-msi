using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Rest;

namespace AzureFluentLocalMsi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var azureManagementOptions = new AzureManagementOptions // use DI
            {
                TenantId = "<TENANT_ID>",
                SubscriptionId = "<SUBSCRIPTION_ID>"
            };

            var azureManagementTokenProvider = new AzureManagementTokenProvider(); // use DI


            var tokenCredentials = new TokenCredentials(azureManagementTokenProvider);

            var azureCredentials = new AzureCredentials(
                tokenCredentials,
                tokenCredentials,
                azureManagementOptions.TenantId,
                AzureEnvironment.AzureGlobalCloud);

            // If you only need the authenticated rest client, 'Microsoft.Azure.Management.ResourceManager.Fluent' is enough.
            var client = RestClient
                .Configure()
                .WithEnvironment(AzureEnvironment.AzureGlobalCloud)
                .WithDelegatingHandler(new HttpLoggingDelegatingHandler())
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .WithCredentials(azureCredentials)
                .Build();


            // If you need the full fluent Azure API, 'Microsoft.Azure.Management.Fluent' is needed.
            var azure = Microsoft.Azure.Management.Fluent.Azure
                .Authenticate(client, azureManagementOptions.TenantId)
                .WithSubscription(azureManagementOptions.SubscriptionId);


            Console.WriteLine("Resource Groups:");
            foreach (var rg in await azure.ResourceGroups.ListAsync())
            {
                Console.WriteLine(rg.Name);
            }


            // Plain HTTP call
            var url = $"{client.BaseUri}subscriptions?api-version=2014-04-01";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            await client.Credentials.ProcessHttpRequestAsync(request, CancellationToken.None);
            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);

            Console.WriteLine("Subscriptions:");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
