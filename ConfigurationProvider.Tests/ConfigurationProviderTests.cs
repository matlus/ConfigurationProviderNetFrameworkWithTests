using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using ConfigurationProviderNetFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConfigurationProvider.Tests
{
    /*
     * In the ConfigurationProviderNetFramework project's assemblyInfo.cs
     * file make InternalsVisibleTo - This assembly like so: [assembly: InternalsVisibleTo("ConfigurationProvider.Tests")]
     * Create an app.config file for this project. Leave it blank 
     */

    [TestClass]
    public class ConfigurationProviderTests
    {
        private readonly ConfigurationProviderBase _configurationProvider = new ConfigurationProviderNetFramework.ConfigurationProvider();
        private readonly Configuration _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private const string emailTemplatesPathKey = "EmailTemplatesPath";
        private const string paymentGatewayServiceUrl = "PaymentGatewayServiceUrl";

        private const string appSettingsSection = "appSettings";
        private const string connectionStringsSection = "connectionStrings";

        private void AddAppSettingInConfigFile(string key, string value)
        {
            _configuration.AppSettings.Settings.Add(key, value);
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(appSettingsSection);
        }

        private void RemoveAppSettingFromFromConfigFile(string key)
        {
            _configuration.AppSettings.Settings.Remove(key);
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(appSettingsSection);
        }

        private void RemoveConnectionStringSettings(string connectionStringSettingName)
        {
            // Found that using configuration.ConnectionStrings.ConnectionStrings[connectionStringSettingName] does not work as expected
            // So using ConfigurationManager to retrieve the ConnectionString setting instead.
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringSettingName];

            if (connectionStringSettings != null)
            {
                _configuration.ConnectionStrings.ConnectionStrings.Remove(connectionStringSettings);
                _configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(connectionStringsSection);
            }
        }

        private void AddConnectionStringSettings(string connectionStringSettingName, string connectionString, string providerInvariantName)
        {
            var connectionStringSettings = new ConnectionStringSettings(connectionStringSettingName, connectionString, providerInvariantName);
            _configuration.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(connectionStringsSection);
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            RemoveAppSettingFromFromConfigFile(emailTemplatesPathKey);
            RemoveAppSettingFromFromConfigFile(paymentGatewayServiceUrl);
        }

        #region EmailTemplatesPath Tests

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void EmailTemplatesPath_WhenConfigSettingExistsAndIsSetupCorretly_ReturnsExpectedValue()
        {
            // Arrange
            var expectedEmailTemplatePath = "\\SomeEmailTemplatesFolder";
            AddAppSettingInConfigFile(emailTemplatesPathKey, expectedEmailTemplatePath);

            // Act
            var actualEmailTemplatePath = _configurationProvider.EmailTemplatesPath;

            // Assert 
            Assert.AreEqual(expectedEmailTemplatePath, actualEmailTemplatePath, $"We were expecting the actualEmailTemplatePath to be: {expectedEmailTemplatePath}, but found it to be: {actualEmailTemplatePath}");
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void EmailTemplatesPath_WhenConfigSettingExistsButDoesNotStartWithBackslash_ReturnsTheValueWithStartingBackslash()
        {
            // Arrange
            var emailTemplatePath = "SomeEmailTemplatesFolder";
            AddAppSettingInConfigFile(emailTemplatesPathKey, emailTemplatePath);
            var expectedEmailTemplatePath = "\\" + emailTemplatePath;

            // Act
            var actualEmailTemplatePath = _configurationProvider.EmailTemplatesPath;

            // Assert 
            Assert.AreEqual(expectedEmailTemplatePath, actualEmailTemplatePath, $"We were expecting the actualEmailTemplatePath to be: {expectedEmailTemplatePath}, but found it to be: {actualEmailTemplatePath}");
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void EmailTemplatesPath_WhenConfigSettingExistsButValueIsEmpty_ThrowsException()
        {
            // Arrange           
            AddAppSettingInConfigFile(emailTemplatesPathKey, string.Empty);

            // Act & Assert
            Assert.That.ThrowsExceptionWithSpecificWords<ConfigurationErrorsException>(() => _configurationProvider.EmailTemplatesPath, new[] { emailTemplatesPathKey, "is Empty", "Required" });
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void EmailTemplatesPath_WhenConfigSettingExistsButValueIsWhiteSpaces_ThrowsException()
        {
            // Arrange           
            AddAppSettingInConfigFile(emailTemplatesPathKey, "           ");

            // Act & Assert
            Assert.That.ThrowsExceptionWithSpecificWords<ConfigurationErrorsException>(() => _configurationProvider.EmailTemplatesPath, new[] { emailTemplatesPathKey, "is White Spaces", "Required" });
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void EmailTemplatesPath_WhenConfigSettingIsMissing_ThrowsException()
        {
            // Arrange           
            // Nothing to arrange

            // Act & Assert
            Assert.That.ThrowsExceptionWithSpecificWords<ConfigurationErrorsException>(() => _configurationProvider.EmailTemplatesPath, new[] { emailTemplatesPathKey, "is Missing", "Required" });
        }

        #endregion EmailTemplatesPath Tests

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void PaymentGatewayServiceUrl_WhenConfigSettingExistsAndIsSetupCorretly_ReturnsExpectedValue()
        {
            // Arrange
            var expectedPaymentGatewayServiceUrl = "http://payments.matlus.com/";
            AddAppSettingInConfigFile(paymentGatewayServiceUrl, expectedPaymentGatewayServiceUrl);

            // Act
            var actualPaymentGatewayServiceUrl = _configurationProvider.PaymentGatewayServiceUrl;

            // Assert 
            Assert.AreEqual(expectedPaymentGatewayServiceUrl, actualPaymentGatewayServiceUrl, $"We were expecting the actualPaymentGatewayServiceUrl to be: {expectedPaymentGatewayServiceUrl}, but found it to be: {actualPaymentGatewayServiceUrl}");
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void PaymentGatewayServiceUrl_WhenConfigSettingExistsButDoesNotEndWithForwardSlash_ReturnsTheValueEndingWithForwardSlash()
        {
            // Arrange
            var gatewayServiceUrl = "http://payments.matlus.com";            
            AddAppSettingInConfigFile(paymentGatewayServiceUrl, gatewayServiceUrl);
            var expectedPaymentGatewayServiceUrl = gatewayServiceUrl + "/";

            // Act
            var actualPaymentGatewayServiceUrl = _configurationProvider.PaymentGatewayServiceUrl;

            // Assert 
            Assert.AreEqual(expectedPaymentGatewayServiceUrl, actualPaymentGatewayServiceUrl, $"We were expecting the actualEmailTemplatePath to be: {expectedPaymentGatewayServiceUrl}, but found it to be: {actualPaymentGatewayServiceUrl}");
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void PaymentGatewayServiceUrl_WhenConfigSettingExistsButValueIsEmpty_ThrowsException()
        {
            // Arrange           
            AddAppSettingInConfigFile(paymentGatewayServiceUrl, string.Empty);

            // Act & Assert
            Assert.That.ThrowsExceptionWithSpecificWords<ConfigurationErrorsException>(() => _configurationProvider.PaymentGatewayServiceUrl, new[] { paymentGatewayServiceUrl, "is Empty", "Required" });
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void PaymentGatewayServiceUrl_WhenConfigSettingExistsButValueIsWhiteSpaces_ThrowsException()
        {
            // Arrange           
            AddAppSettingInConfigFile(paymentGatewayServiceUrl, "           ");

            // Act & Assert
            Assert.That.ThrowsExceptionWithSpecificWords<ConfigurationErrorsException>(() => _configurationProvider.PaymentGatewayServiceUrl, new[] { paymentGatewayServiceUrl, "is White Spaces", "Required" });
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void PaymentGatewayServiceUrl_WhenConfigSettingIsMissing_ThrowsException()
        {
            // Arrange           
            // Nothing to arrange

            // Act & Assert
            Assert.That.ThrowsExceptionWithSpecificWords<ConfigurationErrorsException>(() => _configurationProvider.PaymentGatewayServiceUrl, new[] { paymentGatewayServiceUrl, "is Missing", "Required" });
        }
    }
}
