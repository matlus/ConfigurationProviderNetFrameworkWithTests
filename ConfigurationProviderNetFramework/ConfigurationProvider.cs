using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationProviderNetFramework
{
    internal sealed class ConfigurationProvider : ConfigurationProviderBase
    {
        protected override string GetConfigurationSettingValue(string configurationSettingKey)
        {
            return ConfigurationManager.AppSettings[configurationSettingKey];
        }

        protected override DbConnectionInformation GetDbConnectionInformationCore(string connectionStringName)
        {
            var connectionStringSection = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSection == null)
            {
                throw new ConfigurationErrorsException($"The ConnectionString setting with the Name: {connectionStringName} is Missing in the configuration file. This setting is a Required setting");
            }

            var connectionString = connectionStringSection.ConnectionString;
            EnsureConnectionStringIsPresent(connectionStringName, connectionString);

            var providerName = connectionStringSection.ProviderName;
            EnsureProviderNameIsPresent(connectionStringName, providerName);

            return new DbConnectionInformation(connectionStringName, connectionString, connectionStringSection.ProviderName);
        }
    }    
}
