﻿namespace IntelligentPlant.IndustrialAppStore.Authentication {
    internal static class IndustrialAppStoreAuthenticationOptionsExtensions {

        private static string GetEndpoint(this IndustrialAppStoreAuthenticationOptions options, string relativePath) {
            var baseUrl = options.IndustrialAppStoreUrl;
            baseUrl = baseUrl?.TrimEnd('/');
            return string.Concat(baseUrl, relativePath);
        }


        internal static string GetAuthorizationEndpoint(this IndustrialAppStoreAuthenticationOptions options) {
            return options.GetEndpoint("/AuthorizationServer/OAuth/Authorize");
        }


        internal static string GetTokenEndpoint(this IndustrialAppStoreAuthenticationOptions options) {
            return options.GetEndpoint("/AuthorizationServer/OAuth/Token");
        }


        internal static string GetUserInformationEndpoint(this IndustrialAppStoreAuthenticationOptions options) {
            return options.GetEndpoint("/api/user-search/me");
        }

    }
}
