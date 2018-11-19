using NUnit.Framework;
using System;
using System.Reflection;

namespace FuelSDK.Test
{
    [TestFixture()]
    class ETClientTest
    {
        ETClient client1;
        ETClient client2;

        [OneTimeSetUp]
        public void Setup()
        {
            client1 = new ETClient();
            client2 = new ETClient();
        }

        [Test()]
        public void GetClientStack()
        {
            Assert.IsNotNull(client1.Stack);
            Assert.IsNotNull(client2.Stack);
            Assert.AreEqual(client1.Stack, client2.Stack);
        }

        [Test()]
        public void TestSoapEndpointCaching()
        {
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
