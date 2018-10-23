using System.Configuration;
using System.IO;
using System.Reflection;

namespace FuelSDK.Test
{
    public class CustomConfigSectionBasedTest
    {
        protected readonly string emptyConfigFileName = "empty.config";
        protected readonly string missingRequiredAppSignaturePropertyConfigFileName = "missingRequiredAppSignatureProperty.config";
        protected readonly string missingRequiredClientIdConfigFileName = "missingRequiredClientIdProperty.config";
        protected readonly string missingRequiredClientSecretConfigFileName = "missingRequiredClientSecretProperty.config";
        protected readonly string requiredPropertiesOnlyConfigFileName = "requiredPropertiesOnly.config";
        protected readonly string allPropertiesSetConfigFileName = "allPropertiesSet.config";
        protected readonly string authEndpointMissingLegacyQueryParamFileName = "authEndpointMissingLegacyQueryParam.config";
        protected readonly string authEndpointWithLegacyQueryParamFileName = "authEndpointWithLegacyQueryParam.config";
        protected readonly string authEndpointWithMultipleQueryParamsButMissingLegacyParamFileName = "authEndpointWithMultipleQueryParamsButMissingLegacyParam.config";
        protected readonly string authEndpointWithMultipleQueryParamsIncludingLegacyParamFileName = "authEndpointWithMultipleQueryParamsButMissingLegacyParam.config";

        protected FuelSDKConfigurationSection GetCustomConfigurationSectionFromConfigFile(string configFileName)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ConfigFiles", configFileName);

            Configuration config
              = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                ConfigurationUserLevel.None);

            return config.GetSection("fuelSDK") as FuelSDKConfigurationSection;
        }
    }
}
