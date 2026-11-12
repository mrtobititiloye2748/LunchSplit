/*
 * FILE          : SquareSettings.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Plain settings object for the Square Sandbox connection. Values are NOT
 *   hard-coded here: they are loaded at runtime from environment variables or
 *   appsettings.json (see the test project's SquareTestConfiguration helper).
 */

namespace LunchSplit.Infrastructure
{
    // Connection settings needed by SquarePaymentService.
    public class SquareSettings
    {
        // Sandbox access token (env var: SQUARE_SANDBOX_ACCESS_TOKEN)
        public string AccessToken { get; set; }

        // Sandbox location id that payments are taken for (env var: SQUARE_SANDBOX_LOCATION_ID)
        public string LocationId { get; set; }

        // Base URL of the Square API. Defaults to the Sandbox so tests never hit production.
        public string BaseUrl { get; set; }

        // Explicit network timeout in seconds so a dropped socket can never hang 'dotnet test'
        public int TimeoutSeconds { get; set; }

        // Default constructor: empty credentials, Sandbox URL and a 5 second timeout
        public SquareSettings()
        {
            AccessToken = string.Empty;
            LocationId = string.Empty;
            BaseUrl = "https://connect.squareupsandbox.com";
            TimeoutSeconds = 5;
        }

        /*
         * METHOD      : HasCredentials
         * DESCRIPTION : Checks that both the access token and the location id were supplied.
         * PARAMETERS  : none
         * RETURNS     : bool - true when the sandbox credentials are present
         */
        public bool HasCredentials()
        {
            bool hasToken = !string.IsNullOrWhiteSpace(AccessToken);
            bool hasLocation = !string.IsNullOrWhiteSpace(LocationId);
            return hasToken && hasLocation;
        }
    }
}
