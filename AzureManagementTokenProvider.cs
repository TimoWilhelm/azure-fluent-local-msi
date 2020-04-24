using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Rest;

namespace AzureFluentLocalMsi
{
    public class AzureManagementTokenProvider : ITokenProvider
    {
        private static Lazy<AzureServiceTokenProvider> lazyAzureServiceTokenProvider = 
            new Lazy<AzureServiceTokenProvider>(() => new AzureServiceTokenProvider(null, "https://login.microsoftonline.com"));

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            var token = await lazyAzureServiceTokenProvider.Value.GetAccessTokenAsync("https://management.core.windows.net");
            return new AuthenticationHeaderValue("Bearer", token);
        }
    }
}