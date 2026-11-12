/*
 * FILE          : SquareTestConfiguration.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Helper that loads the Square Sandbox credentials for the Tier 1 tests.
 *   Credentials are externalised (never hard-coded in C#). Lookup order, where
 *   later sources override earlier ones:
 *     1. appsettings.json              (committed template with empty values)
 *     2. appsettings.Development.json  (optional, git-ignored, local keys)
 *     3. Environment variables         (SQUARE_SANDBOX_ACCESS_TOKEN, ...)
 *   Paths are relative to the test output folder, so no local machine paths are used.
 */

using System;
using LunchSplit.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace LunchSplit.Tests.Integration
{
    // Builds SquareSettings from configuration files and environment variables.
    public static class SquareTestConfiguration
    {
        // Configuration key / environment variable names
        public const string AccessTokenKey = "SQUARE_SANDBOX_ACCESS_TOKEN";
        public const string LocationIdKey = "SQUARE_SANDBOX_LOCATION_ID";
        public const string CurrencyKey = "SQUARE_CURRENCY";
        public const string TimeoutKey = "SQUARE_TIMEOUT_SECONDS";

        /*
         * METHOD      : BuildConfiguration
         * DESCRIPTION : Creates the layered configuration (json files + environment variables).
         * PARAMETERS  : none
         * RETURNS     : IConfiguration - merged configuration values
         */
        private static IConfiguration BuildConfiguration()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();

            // Look for the json files next to the compiled test assembly (works on any machine)
            builder.SetBasePath(AppContext.BaseDirectory);
            builder.AddJsonFile("appsettings.json", true);
            builder.AddJsonFile("appsettings.Development.json", true);

            // Environment variables are added last so they win over the json files
            builder.AddEnvironmentVariables();

            return builder.Build();
        }

        /*
         * METHOD      : LoadSettings
         * DESCRIPTION : Reads the Square sandbox token, location id and timeout.
         * PARAMETERS  : none
         * RETURNS     : SquareSettings - settings ready to pass to SquarePaymentService
         */
        public static SquareSettings LoadSettings()
        {
            IConfiguration configuration = BuildConfiguration();
            SquareSettings settings = new SquareSettings();

            settings.AccessToken = ReadValue(configuration, AccessTokenKey, string.Empty);
            settings.LocationId = ReadValue(configuration, LocationIdKey, string.Empty);

            // Timeout falls back to 5 seconds if the value is missing or not a number
            string timeoutText = ReadValue(configuration, TimeoutKey, "5");
            int timeoutSeconds;
            if (int.TryParse(timeoutText, out timeoutSeconds) && timeoutSeconds > 0)
            {
                settings.TimeoutSeconds = timeoutSeconds;
            }

            return settings;
        }

        /*
         * METHOD      : LoadCurrency
         * DESCRIPTION : Reads the currency of the sandbox location (defaults to CAD).
         * PARAMETERS  : none
         * RETURNS     : string - ISO currency code
         */
        public static string LoadCurrency()
        {
            IConfiguration configuration = BuildConfiguration();
            return ReadValue(configuration, CurrencyKey, "CAD").ToUpperInvariant();
        }

        /*
         * METHOD      : ReadValue
         * DESCRIPTION : Returns a trimmed configuration value or a default when it is empty.
         * PARAMETERS  : IConfiguration configuration - merged configuration
         *               string key                  - setting name
         *               string defaultValue         - value used when the setting is blank
         * RETURNS     : string - the setting value
         */
        private static string ReadValue(IConfiguration configuration, string key, string defaultValue)
        {
            string? value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }
            return value.Trim();
        }
    }
}
