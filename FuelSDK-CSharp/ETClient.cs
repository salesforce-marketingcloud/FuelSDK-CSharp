using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Linq;
using JWT;
using JWT.Serializers;
using Newtonsoft.Json.Linq;

namespace FuelSDK
{

    /// <summary>
    /// Client interface class which manages the authentication process 
    /// </summary>
    public class ETClient
    {
        public const string SDKVersion = "FuelSDK-C#-v1.1.1";

        private FuelSDKConfigurationSection configSection;
        public string AuthToken { get; private set; }
        public SoapClient SoapClient { get; private set; }
        public string InternalAuthToken { get; private set; }
        public string RefreshKey { get; private set; }
        public DateTime AuthTokenExpiration { get; private set; }
        public JObject Jwt { get; private set; }
        public string EnterpriseId { get; private set; }
        public string OrganizationId { get; private set; }
        private string stackKey;
        public bool UseOAuth2Authentication { get; private set; }

        [Obsolete(StackKeyErrorMessage)]
        public string Stack
        {
            get
            {
                if (stackKey != null)
                    return stackKey;

                stackKey = GetStackFromSoapEndPoint(new Uri(configSection.SoapEndPoint));
                return stackKey;
            }
            private set
            {
                stackKey = value;
            }
        }

        private static DateTime soapEndPointExpiration;
        private static DateTime stackKeyExpiration;
        private static string fetchedSoapEndpoint;
        private const long cacheDurationInMinutes = 10;

        private const string StackKeyErrorMessage = "Tenant specific endpoints doesn't support Stack Key property and this will property will be deprecated in next major release";

        public class RefreshState
        {
            public string RefreshKey { get; set; }
            public string EnterpriseId { get; set; }
            public string OrganizationId { get; set; }
            public string Stack { get; set; }
        }

        public ETClient(string jwt)
            : this(new NameValueCollection { { "jwt", jwt } }, null) { }
        public ETClient(NameValueCollection parameters = null, RefreshState refreshState = null)
        {
            // Get configuration file and set variables
            configSection = (FuelSDKConfigurationSection)ConfigurationManager.GetSection("fuelSDK");
            configSection = (configSection != null ? (FuelSDKConfigurationSection)configSection.Clone() : new FuelSDKConfigurationSection());
            configSection = configSection
                .WithDefaultAuthEndpoint(DefaultEndpoints.Auth)
                .WithDefaultRestEndpoint(DefaultEndpoints.Rest);
            if (parameters != null)
            {
                if (parameters.AllKeys.Contains("appSignature"))
                    configSection.AppSignature = parameters["appSignature"];
                if (parameters.AllKeys.Contains("clientId"))
                    configSection.ClientId = parameters["clientId"];
                if (parameters.AllKeys.Contains("clientSecret"))
                    configSection.ClientSecret = parameters["clientSecret"];
                if (parameters.AllKeys.Contains("soapEndPoint"))
                    configSection.SoapEndPoint = parameters["soapEndPoint"];
                if (parameters.AllKeys.Contains("authEndPoint"))
                {
                    configSection.AuthenticationEndPoint = parameters["authEndPoint"];
                }
                if (parameters.AllKeys.Contains("restEndPoint"))
                {
                    configSection.RestEndPoint = parameters["restEndPoint"];
                }
                if (parameters.AllKeys.Contains("oAuth2AuthenticationMode"))
                {
                    configSection.RestEndPoint = parameters["oAuth2AuthenticationMode"];
                }
            }
            UseOAuth2Authentication = String.IsNullOrEmpty(configSection.OAuth2AuthenticationMode)? false : Boolean.Parse(configSection.OAuth2AuthenticationMode);
            if (string.IsNullOrEmpty(configSection.ClientId) || string.IsNullOrEmpty(configSection.ClientSecret))
                throw new Exception("clientId or clientSecret is null: Must be provided in config file or passed when instantiating ETClient");

            // If JWT URL Parameter Used
            var organizationFind = false;
            if (refreshState != null)
            {
                RefreshKey = refreshState.RefreshKey;
                EnterpriseId = refreshState.EnterpriseId;
                OrganizationId = refreshState.OrganizationId;
                Stack = refreshState.Stack;
                RefreshToken();
            }
            else if (parameters != null && parameters.AllKeys.Contains("jwt") && !string.IsNullOrEmpty(parameters["jwt"]))
            {
                if (string.IsNullOrEmpty(configSection.AppSignature))
                    throw new Exception("Unable to utilize JWT for SSO without appSignature: Must be provided in config file or passed when instantiating ETClient");
                var encodedJWT = parameters["jwt"].ToString().Trim();
                var decodedJWT = DecodeJWT(encodedJWT, configSection.AppSignature);
                var parsedJWT = JObject.Parse(decodedJWT);
                AuthToken = parsedJWT["request"]["user"]["oauthToken"].Value<string>().Trim();
                AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedJWT["request"]["user"]["expiresIn"].Value<string>().Trim()));
                InternalAuthToken = parsedJWT["request"]["user"]["internalOauthToken"].Value<string>().Trim();
                RefreshKey = parsedJWT["request"]["user"]["refreshToken"].Value<string>().Trim();
                Jwt = parsedJWT;
                var orgPart = parsedJWT["request"]["organization"];
                if (orgPart != null)
                {
                    EnterpriseId = orgPart["enterpriseId"].Value<string>().Trim();
                    OrganizationId = orgPart["id"].Value<string>().Trim();
                    Stack = orgPart["stackKey"].Value<string>().Trim();
                }
            }
            else
            {
                RefreshToken();
                organizationFind = true;
            }

            FetchSoapEndpoint();

            // Create the SOAP binding for call with Oauth.
            SoapClient = new SoapClient(GetSoapBinding(UseOAuth2Authentication), new EndpointAddress(new Uri(configSection.SoapEndPoint)));
            SoapClient.ClientCredentials.UserName.UserName = "*";
            SoapClient.ClientCredentials.UserName.Password = "*";

            // Find Organization Information
            if (organizationFind)
            {
                if (!UseOAuth2Authentication)
                {
                    using (var scope = new OperationContextScope(SoapClient.InnerChannel))
                    {
                        // Add oAuth token to SOAP header.
                        XNamespace ns = "http://exacttarget.com";
                        var oauthElement = new XElement(ns + "oAuthToken", InternalAuthToken);
                        var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                        OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                        var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                        OperationContext.Current.OutgoingMessageProperties.Add(
                            System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                        httpRequest.Headers.Add(HttpRequestHeader.UserAgent, ETClient.SDKVersion);

                        string requestID;
                        APIObject[] results;
                        var r = SoapClient.Retrieve(
                            new RetrieveRequest
                            {
                                ObjectType = "BusinessUnit",
                                Properties = new[] { "ID", "Client.EnterpriseID" }
                            }, out requestID, out results);
                        if (r == "OK" && results.Length > 0)
                        {
                            EnterpriseId = results[0].Client.EnterpriseID.ToString();
                            OrganizationId = results[0].ID.ToString();
                        }
                    }
                }
                else
                {

                }
            }
        }

        internal string FetchRestAuth()
        {
            var returnedRestAuthEndpoint = new ETEndpoint { AuthStub = this, Type = "restAuth" }.Get();
            if (returnedRestAuthEndpoint.Status && returnedRestAuthEndpoint.Results.Length == 1)
                return ((ETEndpoint)returnedRestAuthEndpoint.Results[0]).URL;
            else
                throw new Exception("REST auth endpoint could not be determined");
        }

        private void FetchSoapEndpoint()
        {
            if (string.IsNullOrEmpty(configSection.SoapEndPoint) || (DateTime.Now > soapEndPointExpiration && fetchedSoapEndpoint != null))
            {
                try
                {
                    var grSingleEndpoint = new ETEndpoint { AuthStub = this, Type = "soap" }.Get();
                    if (grSingleEndpoint.Status && grSingleEndpoint.Results.Length == 1)
                    {
                        // Find the appropriate endpoint for the account
                        configSection.SoapEndPoint = ((ETEndpoint)grSingleEndpoint.Results[0]).URL;
                        fetchedSoapEndpoint = configSection.SoapEndPoint;
                        soapEndPointExpiration = DateTime.Now.AddMinutes(cacheDurationInMinutes);
                    }
                    else
                        configSection.SoapEndPoint = DefaultEndpoints.Soap;
                }
                catch
                {
                    configSection.SoapEndPoint = DefaultEndpoints.Soap;
                }
            }
        }

        private string DecodeJWT(string jwt, string key)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            var json = decoder.Decode(jwt, key, true);
            return json;
        }

        private string GetStackFromSoapEndPoint(Uri uri)
        {
            var parts = uri.Host.Split('.');
            if (parts.Length < 2 || !parts[0].Equals("webservice", StringComparison.OrdinalIgnoreCase))
                throw new Exception(StackKeyErrorMessage);
            return (parts[1] == "exacttarget" ? "s1" : parts[1].ToLower());
        }

        private static Binding GetSoapBinding(bool useOauth2 = false)
        {
            BindingElementCollection bindingCollection = new BindingElementCollection();
            if (!useOauth2)
            {
                bindingCollection.Add(SecurityBindingElement.CreateUserNameOverTransportBindingElement());
            }
            bindingCollection.AddRange(new TextMessageEncodingBindingElement
                {
                    MessageVersion = MessageVersion.Soap12WSAddressingAugust2004,
                    ReaderQuotas =
                    {
                        MaxDepth = 32,
                        MaxStringContentLength = int.MaxValue,
                        MaxArrayLength = int.MaxValue,
                        MaxBytesPerRead = int.MaxValue,
                        MaxNameTableCharCount = int.MaxValue
                    }
                },
                new HttpsTransportBindingElement
                {
                    TransferMode = TransferMode.Buffered,
                    MaxReceivedMessageSize = 655360000,
                    MaxBufferSize = 655360000,
                    KeepAliveEnabled = true
                });
           
            return new CustomBinding(bindingCollection)
            {
                Name = "UserNameSoapBinding",
                Namespace = "Core.Soap",
                CloseTimeout = new TimeSpan(0, 50, 0),
                OpenTimeout = new TimeSpan(0, 50, 0),
                ReceiveTimeout = new TimeSpan(0, 50, 0),
                SendTimeout = new TimeSpan(0, 50, 0)
            };
        }

        public void RefreshToken(bool force = false)
        {
            if (UseOAuth2Authentication)
                RefreshTokenWithOauth2(force);
            // workaround to support TLS 1.2 in .NET 4.0 (source: https://blogs.perficient.com/2016/04/28/tsl-1-2-and-net-support/)
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            // RefreshToken
            if (!string.IsNullOrEmpty(AuthToken) && DateTime.Now.AddSeconds(300) <= AuthTokenExpiration && !force)
                return;

            // Get an internalAuthToken using clientId and clientSecret
            var authEndpoint = new AuthEndpointUriBuilder(configSection).Build();

            // Build the request
            var request = (HttpWebRequest)WebRequest.Create(authEndpoint.Trim());
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = SDKVersion;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json;
                if (!string.IsNullOrEmpty(RefreshKey))
                    json = string.Format("{{\"clientId\": \"{0}\", \"clientSecret\": \"{1}\", \"refreshToken\": \"{2}\", \"scope\": \"cas:{3}\" , \"accessType\": \"offline\"}}", configSection.ClientId, configSection.ClientSecret, RefreshKey, InternalAuthToken);
                else
                    json = string.Format("{{\"clientId\": \"{0}\", \"clientSecret\": \"{1}\", \"accessType\": \"offline\"}}", configSection.ClientId, configSection.ClientSecret);
                streamWriter.Write(json);
            }

            // Get the response
            string responseFromServer = null;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream))
                responseFromServer = reader.ReadToEnd();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            InternalAuthToken = parsedResponse["legacyToken"].Value<string>().Trim();
            AuthToken = parsedResponse["accessToken"].Value<string>().Trim();
            AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expiresIn"].Value<string>().Trim()));
            RefreshKey = parsedResponse["refreshToken"].Value<string>().Trim();
        }

        public void RefreshTokenWithOauth2(bool force = false)
        {
            // workaround to support TLS 1.2 in .NET 4.0 (source: https://blogs.perficient.com/2016/04/28/tsl-1-2-and-net-support/)
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            // RefreshToken
            if (!string.IsNullOrEmpty(AuthToken) && DateTime.Now.AddSeconds(300) <= AuthTokenExpiration && !force)
                return;

            var authEndpoint = configSection.AuthenticationEndPoint + "/v2/token";
            var request = (HttpWebRequest)WebRequest.Create(authEndpoint.Trim());
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = SDKVersion;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = string.Format("{{\"client_id\": \"{0}\", \"client_secret\": \"{1}\", \"grant_type\": \"client_credentials\"}}", configSection.ClientId, configSection.ClientSecret);
                streamWriter.Write(json);
            }

            // Get the response
            string responseFromServer = null;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream))
                responseFromServer = reader.ReadToEnd();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            AuthToken = parsedResponse["access_token"].Value<string>().Trim();
            InternalAuthToken = parsedResponse["access_token"].Value<string>().Trim();
            AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expires_in"].Value<string>().Trim()));
            configSection.SoapEndPoint = parsedResponse["soap_instance_url"].Value<string>().Trim() + "service.asmx";
            configSection.RestEndPoint = parsedResponse["rest_instance_url"].Value<string>().Trim();
        }

        public FuelReturn AddSubscribersToList(string emailAddress, string subscriberKey, IEnumerable<int> listIDs) { return ProcessAddSubscriberToList(emailAddress, subscriberKey, listIDs); }
        public FuelReturn AddSubscribersToList(string emailAddress, IEnumerable<int> listIDs) { return ProcessAddSubscriberToList(emailAddress, null, listIDs); }
        protected FuelReturn ProcessAddSubscriberToList(string emailAddress, string subscriberKey, IEnumerable<int> listIDs)
        {
            var sub = new ETSubscriber
            {
                AuthStub = this,
                EmailAddress = emailAddress,
                Lists = listIDs.Select(x => new ETSubscriberList { ID = x }).ToArray(),
            };
            if (subscriberKey != null)
                sub.SubscriberKey = subscriberKey;
            var prAddSub = sub.Post();
            if (!prAddSub.Status && prAddSub.Results.Length > 0 && prAddSub.Results[0].ErrorCode == 12014)
                return sub.Patch();
            return prAddSub;
        }

        public FuelReturn CreateDataExtensions(ETDataExtension[] dataExtensions)
        {
            var cleanedArray = new List<ETDataExtension>();
            foreach (var de in dataExtensions)
            {
                de.AuthStub = this;
                de.Fields = de.Columns;
                de.Columns = null;
                cleanedArray.Add(de);
            }
            return new PostReturn(cleanedArray.ToArray());
        }


    }

    [Obsolete("ET_Client will be removed in future release. Use ETClient class instead.")]
    public class ET_Client : ETClient
    {
        [Obsolete("This method will be removed in future release.")]
        public FuelReturn CreateDataExtensions(ET_DataExtension[] dataExtensions)
        {
            var cleanedArray = new List<ET_DataExtension>();
            foreach (var de in dataExtensions)
            {
                de.AuthStub = this;
                de.Fields = de.Columns;
                de.Columns = null;
                cleanedArray.Add(de);
            }
            return new PostReturn(cleanedArray.ToArray());
        }
    }

}