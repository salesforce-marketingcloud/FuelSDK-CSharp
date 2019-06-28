using NUnit.Framework;
using System;
using FuelSDK;
using Newtonsoft.Json.Linq;

namespace FuelSDK.Test
{
    [TestFixture()]
    public class RefreshTokenTest
    {
        ETClient client;
 
        [OneTimeSetUp]
        public void Setup()
        {
            client = new ETClient();
        }

        [Test()]
        public void Oauth2RefreshToken()
        {
            var token = client.AuthToken;
            var refreshToken = client.RefreshKey;
         

            client.RefreshTokenWithOauth2(true);
            var token1 = client.AuthToken;
            var refreshToken1 = client.RefreshKey;


            Assert.AreNotEqual(token, token1, "Tokens should differ as call is made with Refresh Token");
            Assert.AreNotEqual(refreshToken, refreshToken1, "Refresh Tokens should differ as call is made with Refresh Token");

        }
    }
}
