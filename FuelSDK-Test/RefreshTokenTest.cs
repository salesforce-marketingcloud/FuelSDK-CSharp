using NUnit.Framework;
using System;
using FuelSDK;
using Newtonsoft.Json.Linq;
using System.Reflection;

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
        public void AuthTokenShouldDifferIfRefreshTokenIsEnforced()
        {
            var token = client.AuthToken;
            var refreshToken = client.RefreshKey;
            client.RefreshTokenWithOauth2(true);
            var token1 = client.AuthToken;
            var refreshToken1 = client.RefreshKey;


            Assert.AreNotEqual(token, token1);
            Assert.AreNotEqual(refreshToken, refreshToken1);

        }
    }
}
