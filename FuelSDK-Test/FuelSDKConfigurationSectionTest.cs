using NUnit.Framework;
using System.Configuration;

namespace FuelSDK.Test
{
    [TestFixture]
    class FuelSDKConfigurationSectionTest : CustomConfigSectionBasedTest
    {
        [Test()]
        public void NoCustomConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(emptyConfigFileName);
            Assert.IsNull(section);
        }

        [Test()]
        public void MissingRequiredAppSignaturePropertyFromConfigSection()
        {
            Assert.That(() => GetCustomConfigurationSectionFromConfigFile(missingRequiredAppSignaturePropertyConfigFileName), Throws.TypeOf<ConfigurationErrorsException>());
        }

        [Test()]
        public void MissingRequiredClientIdPropertyFromConfigSection()
        {
            Assert.That(() => GetCustomConfigurationSectionFromConfigFile(missingRequiredClientIdConfigFileName), Throws.TypeOf<ConfigurationErrorsException>());
        }

        [Test()]
        public void MissingRequiredClientSecretPropertyFromConfigSection()
        {
            Assert.That(() => GetCustomConfigurationSectionFromConfigFile(missingRequiredClientSecretConfigFileName), Throws.TypeOf<ConfigurationErrorsException>());
        }

        [Test()]
        public void MissingSoapEndPointPropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(requiredPropertiesOnlyConfigFileName);
            Assert.AreEqual(string.Empty, section.SoapEndPoint);
        }

        [Test()]
        public void MissingAuthEndPointPropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(requiredPropertiesOnlyConfigFileName);
            Assert.AreEqual(string.Empty, section.AuthenticationEndPoint);
        }

        [Test()]
        public void MissingRestEndPointPropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(requiredPropertiesOnlyConfigFileName);
            Assert.AreEqual(string.Empty, section.RestEndPoint);
        }

        [Test()]
        public void AllPropertiesSetInConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(allPropertiesSetConfigFileName);
            Assert.AreEqual(section.AppSignature, "none");
            Assert.AreEqual(section.ClientId, "abc");
            Assert.AreEqual(section.ClientSecret, "cde");
            Assert.AreEqual(section.SoapEndPoint, "https://soapendpoint.com");
            Assert.AreEqual(section.AuthenticationEndPoint, "https://authendpoint.com");
            Assert.AreEqual(section.RestEndPoint, "https://restendpoint.com");
        }

        [Test]
        public void AllPropertiesSetButAuthEndpointIsEmpty()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(allPropertiesSetButAuthEndpointIsEmptyConfigFileName);
            var sectionWithDefaultAuthEndpoint = section.WithDefaultAuthEndpoint(DefaultEndpoints.Auth);

            Assert.AreEqual(DefaultEndpoints.Auth, sectionWithDefaultAuthEndpoint.AuthenticationEndPoint);
        }

        [Test]
        public void AllPropertiesSetButRestEndpointIsEmpty()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(allPropertiesSetButRestEndpointIsEmptyConfigFileName);
            var sectionWithDefaultRestEndpoint = section.WithDefaultRestEndpoint(DefaultEndpoints.Rest);

            Assert.AreEqual(DefaultEndpoints.Rest, sectionWithDefaultRestEndpoint.RestEndPoint);
        }

        [Test]
        public void WithDefaultsDoesNotOverwriteValuesSetInConfig()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(allPropertiesSetConfigFileName);
            section = section
                .WithDefaultRestEndpoint(DefaultEndpoints.Rest)
                .WithDefaultAuthEndpoint(DefaultEndpoints.Auth);

            Assert.AreEqual(section.AuthenticationEndPoint, "https://authendpoint.com");
            Assert.AreEqual(section.RestEndPoint, "https://restendpoint.com");
        }

        [Test]
        public void ModifyingAClonedConfigSectionAffectsTheOriginalSectionAndAnyNewInstance()
        {
            var section = (FuelSDKConfigurationSection)ConfigurationManager.GetSection("fuelSDK");
            var clonedSection = (FuelSDKConfigurationSection)section.Clone();

            clonedSection.SoapEndPoint = "https://soapendpoint.com";
            var newSection = (FuelSDKConfigurationSection)ConfigurationManager.GetSection("fuelSDK");

            Assert.AreEqual(object.ReferenceEquals(section, clonedSection), false);
            Assert.AreNotSame(section, clonedSection);
            Assert.AreEqual(section.SoapEndPoint, clonedSection.SoapEndPoint);
            Assert.AreEqual(section.SoapEndPoint, newSection.SoapEndPoint);
        }
    }
}
