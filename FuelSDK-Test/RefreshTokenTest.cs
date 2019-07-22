using NUnit.Framework;
using System;
using FuelSDK;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Collections.Specialized;

namespace FuelSDK.Test
{
    /// <summary>
    /// These tests are specific for OAuth2 Public/Web Apps, So Config should be modified accordingly.
    /// </summary>
    [TestFixture()]
    public class RefreshTokenTest
    {
        ETClient client;
        FuelSDKConfigurationSection config;
 
        [OneTimeSetUp]
        public void Setup()
        {
            client = new ETClient();
            config = new FuelSDKConfigurationSection();
            config.AuthorizationCode = "Auth_Code_For_OAuth2_WebApp";
            config.RedirectURI = "www.google.com";
            config.ClientId = "OAUTH2_CLIENTID";
            config.ClientSecret = "OAUTH2_CLIENT_SECRET";
            
        }

        [Test()]
        public void AuthTokenAndRefreshTokenShouldDifferIfRefreshTokenIsEnforced()
        {
            var token = client.AuthToken;
            var refreshToken = client.RefreshKey;
            client.RefreshTokenWithOauth2(true);
            var token1 = client.AuthToken;
            var refreshToken1 = client.RefreshKey;


            Assert.AreNotEqual(token, token1);
            Assert.AreNotEqual(refreshToken, refreshToken1);
        }

        [Test()]
        public void AuthPayloadShouldHavePublicAppAttributes()
        {
            config.ApplicationType = "public";
            dynamic payload = client.CreatePayload(config);

            Assert.AreEqual(payload.client_id.ToString(), config.ClientId);
            Assert.AreEqual(payload.redirect_uri.ToString(), config.RedirectURI);
            Assert.AreEqual(payload.code.ToString(), config.AuthorizationCode);
            Assert.AreEqual(payload.grant_type.ToString(), "authorization_code");
        }

        [Test()]
        public void AuthPayloadForPublicApp_ShouldNotHaveClientSecret()
        {
            config.ApplicationType = "public";
            dynamic payload = client.CreatePayload(config);

            Assert.AreEqual(payload.client_secret, null);
        }

        [Test()]
        public void AuthPayloadShouldHaveWebAppAttributes()
        {
            config.ApplicationType = "web";
            dynamic payload = client.CreatePayload(config);

            Assert.AreEqual(payload.grant_type.ToString(), "authorization_code");
            Assert.AreEqual(payload.client_id.ToString(), config.ClientId);
            Assert.AreEqual(payload.client_secret.ToString(), config.ClientSecret);
            Assert.AreEqual(payload.redirect_uri.ToString(), config.RedirectURI);
            Assert.AreEqual(payload.code.ToString(), config.AuthorizationCode);
        }

        [Test()]
        public void AuthPayloadShouldHaveServerAppAttributes()
        {
            config.ApplicationType = "server";
            dynamic payload = client.CreatePayload(config);

            Assert.AreEqual(payload.grant_type.ToString(), "client_credentials");
            Assert.AreEqual(payload.client_id.ToString(), config.ClientId);
            Assert.AreEqual(payload.client_secret.ToString(), config.ClientSecret);
        }

        [Test()]
        public void AuthPayloadForServerApp_ShouldNotHaveCodeAndRedirectURI()
        {
            config.ApplicationType = "server";
            dynamic payload = client.CreatePayload(config);

            Assert.AreEqual(payload.code, null);
            Assert.AreEqual(payload.redirect_uri, null);
        }

        [Test()]
        public void AuthPayloadWithRefreshToken_ShouldHaveRefreshTokenAttribute()
        {
            client.RefreshKey = "REFRESH_KEY";

            config.ApplicationType = "public";
            dynamic payload = client.CreatePayload(config);

            Assert.AreEqual(payload.grant_type.ToString(), "refresh_token");
            Assert.AreEqual(payload.refresh_token.ToString(), client.RefreshKey);
        }
    }
}
