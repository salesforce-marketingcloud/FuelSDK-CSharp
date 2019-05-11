using NUnit.Framework;
using System;
using System.Configuration;
using System.Reflection;

namespace FuelSDK.Test
{
    [TestFixture()]
    class ETClientTest
    {
        [Test()]
        public void TestSoapEndpointCaching()
        {
            var configSection = (FuelSDKConfigurationSection)ConfigurationManager.GetSection("fuelSDK");
            if (!string.IsNullOrEmpty(configSection.UseOAuth2Authentication) && Convert.ToBoolean(configSection.UseOAuth2Authentication) == true)
            {
                // Test does not apply for legacy authentication
                Assert.Pass();
                return;
            }

            var client1 = new ETClient();
            var client2 = new ETClient();

            var client1SoapEndpointExpirationField = client1.GetType().GetField("soapEndPointExpiration", BindingFlags.NonPublic | BindingFlags.Static);
            var client2SoapEndpointExpirationField = client2.GetType().GetField("soapEndPointExpiration", BindingFlags.NonPublic | BindingFlags.Static);

            var client1SoapEndpointExpiration = (DateTime)client1SoapEndpointExpirationField.GetValue(null);
            var client2SoapEndpointExpiration = (DateTime)client2SoapEndpointExpirationField.GetValue(null);

            Assert.IsTrue(client1SoapEndpointExpiration > DateTime.MinValue);
            Assert.IsTrue(client2SoapEndpointExpiration > DateTime.MinValue);
            Assert.AreEqual(client1SoapEndpointExpiration, client2SoapEndpointExpiration);
        }
    }
}
