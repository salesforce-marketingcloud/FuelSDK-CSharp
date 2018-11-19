using System;

namespace FuelSDK
{
    public class AuthEndpointUriBuilder
    {
        private const string legacyQuery = "legacy=1";
        private readonly FuelSDKConfigurationSection configSection;

        public AuthEndpointUriBuilder(FuelSDKConfigurationSection configSection)
        {
            this.configSection = configSection;
        }

        public string Build()
        {
            UriBuilder uriBuilder = new UriBuilder(configSection.AuthenticationEndPoint);

            if (uriBuilder.Query.ToLower().Contains(legacyQuery))
            {
                return uriBuilder.Uri.AbsoluteUri;
            }

            if (uriBuilder.Query.Length > 1)
                uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + legacyQuery;
            else
                uriBuilder.Query = legacyQuery;

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}
