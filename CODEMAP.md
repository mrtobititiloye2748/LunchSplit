# LunchSplit Project CodeMap
- **SolutionFile**: `LunchSplit.sln`
- **CoreSourceRoot**: `LunchSplit.Core`
- **InfraSourceRoot**: `LunchSplit.Infrastructure`
- **TestProjectRoot**: `LunchSplit.Tests`
- **Tier1TestClass**: `LunchSplit.Tests/Integration/SquareSandboxTests.cs`
- **Tier2TestClass**: `LunchSplit.Tests/Unit/PaymentProcessorMoqTests.cs`

## Assignment 3 file map
| Role | Path |
| --- | --- |
| Payment contract (`IPaymentProcessor`) | `LunchSplit.Core/Payments/IPaymentProcessor.cs` |
| Domain DTOs | `LunchSplit.Core/Payments/PaymentRequest.cs`, `PaymentResult.cs`, `PaymentStatus.cs` |
| Payment manager (uses the interface) | `LunchSplit.Core/Payments/PaymentCoordinator.cs` |
| Batch summary | `LunchSplit.Core/Payments/BatchPaymentSummary.cs` |
| Timeout exception | `LunchSplit.Core/Payments/PaymentGatewayTimeoutException.cs` |
| Square SDK wrapper | `LunchSplit.Infrastructure/SquarePaymentService.cs` |
| Square settings | `LunchSplit.Infrastructure/SquareSettings.cs` |
| Credential loader (tests) | `LunchSplit.Tests/Integration/SquareTestConfiguration.cs` |
| Credential template | `LunchSplit.Tests/appsettings.json` (empty values; real keys come from environment variables) |

## Running the tests
```bash
# Square Sandbox credentials (never committed)
export SQUARE_SANDBOX_ACCESS_TOKEN="<sandbox access token>"
export SQUARE_SANDBOX_LOCATION_ID="<sandbox location id>"
# optional, defaults to CAD - must match the sandbox location's currency
export SQUARE_CURRENCY="CAD"

dotnet test LunchSplit.sln
```
Windows PowerShell: `$env:SQUARE_SANDBOX_ACCESS_TOKEN="..."` (same names).
