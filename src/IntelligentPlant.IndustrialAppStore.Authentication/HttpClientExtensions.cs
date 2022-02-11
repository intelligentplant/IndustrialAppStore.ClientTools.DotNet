using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.Authentication {
    public static class HttpClientExtensions {

        public static async Task<OAuthTokenResponse> UseRefreshTokenAsync(this HttpClient backchannel, string refreshToken, IndustrialAppStoreAuthenticationOptions options, CancellationToken cancellationToken) {
            var tokenRequestParameters = new Dictionary<string, string>() {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

#if NETCOREAPP
            tokenRequestParameters["client_id"] = options.ClientId;
            if (!string.IsNullOrWhiteSpace(options.ClientSecret) && !string.Equals(options.ClientSecret, IndustrialAppStoreAuthenticationExtensions.DefaultClientSecret, StringComparison.OrdinalIgnoreCase)) {
                tokenRequestParameters["client_secret"] = options.ClientSecret;
            }
#else
            tokenRequestParameters["client_id"] = options.ClientId;
            tokenRequestParameters["client_secret"] = options.ClientSecret;
#endif

            var refreshRequestContent = new FormUrlEncodedContent(tokenRequestParameters!);
            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, options.GetTokenEndpoint()) {
                Content = refreshRequestContent
            };
            refreshRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var refreshResponse = await backchannel.SendAsync(refreshRequest, cancellationToken);

#if NETCOREAPP
            var tokenResponseJson = System.Text.Json.JsonDocument.Parse(await refreshRequest.Content.ReadAsStreamAsync());
#else
            var tokenResponseJson = Newtonsoft.Json.Linq.JObject.Parse(await refreshResponse.Content.ReadAsStringAsync());
#endif

            return OAuthTokenResponse.Success(tokenResponseJson);
        }

    }
}
