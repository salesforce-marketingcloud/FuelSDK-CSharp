using System.Configuration;

namespace FuelSDK
{
    class ConfigUtil
    {
        public static FuelSDKConfigurationSection GetFuelSDKConfigSection()
        {
            return (FuelSDKConfigurationSection)ConfigurationManager.GetSection("fuelSDK");
        }
    }
}
