# LunchSplit: Assignment 1

LunchSplit is a C# console application that calculates how much each attendee owes for a shared meal. Users enter the subtotal, tax, tip, attendees, and shared items. The program then calculates each person's final amount based on their portion weight and inclusion status.

This project was developed for CS Assignment 1, focusing on C# fundamentals, software architecture, unit testing, and Git workflow.

## How to Run the Project

### Prerequisites

- .NET 6 or later
- Visual Studio or Visual Studio Code

### Running the Application

From the solution root, run:

```bash
dotnet run --project LunchSplit.App
```

Alternatively, if you are already inside the application folder, run:

```bash
cd LunchSplit.App
dotnet run
```

The console will prompt you to enter the subtotal, tax, tip, attendees, and shared items.

Once the information has been entered, the application will display a formatted bill summary and the final amount each attendee owes.

## Project Architecture

### LunchSplit.Core

This project contains the main business logic used to calculate and format the bill.

- **Bill.cs:** Represents the full bill and its details.
- **Attendee.cs:** Represents a person participating in the bill split.
- **Share.cs:** Represents shared items and their costs.
- **Splitter.cs:** Calculates how the bill is divided among attendees.
- **Rounder.cs:** Handles rounding calculations.
- **Formatter.cs:** Formats the bill summary for display.

### LunchSplit.App

This project contains the console application that users interact with.

- **Program.cs:** Handles user input, runs the application logic, and displays the results.

### LunchSplit.Tests

This project contains the MSTest unit tests used to verify the application's behaviour.

The tests cover the main classes and follow the Arrange, Act, Assert (AAA) pattern. They also follow the professor's requirement to avoid using `var` in test code, keeping the syntax consistent and beginner-friendly.

## Testing

### How to Run the Tests

From the solution root, run:

```bash
dotnet test
```

This command runs the test suite and reports the results in the console.

### Testing Approach

The tests follow the AAA pattern:

- **Arrange:** Set up the objects and values needed for the test.
- **Act:** Execute the method being tested.
- **Assert:** Verify that the result matches the expected outcome.

The test suite covers:

- Portion weight calculations
- Attendee inclusion and exclusion
- Shared item distribution
- Rounding logic
- Output formatting

The tests use explicit variable types instead of `var`, as required by the professor.

Test results can be checked directly in the terminal using the output from `dotnet test`. This makes it possible to verify the application's behaviour without needing screenshots or additional visual evidence in this document.

## Git Workflow

Git was used to organize development, track changes, and manage different parts of the project.

### Branches Used

- `main`: Contains the stable version of the project.
- `feature/core-cleanup`: Used for cleaning up the core classes.
- `feature/tests`: Used for changes to the unit test suite.
- `feature-docs`: Used for documentation and README updates.

### Workflow Summary

Each major change was handled on a dedicated feature branch to keep development organized. Related changes were grouped into meaningful commits, and completed branches were merged back into `main`.

This approach helped separate different areas of development, made changes easier to track, and provided a clearer project history.

### Example Commit Messages

The following examples show how commits can describe specific changes:

```text
Refactored Program.cs prompts for clarity and user-friendliness
Cleaned Bill.cs and added XML documentation
Rewrote SplitterTests using AAA pattern and removed var
Added README and project documentation
```

The repository's commit history provides a record of the changes made throughout development.

## Reflection

Working on LunchSplit helped me understand how the different parts of a C# project fit together. At first, I struggled with connecting the classes and getting the console application to run correctly. As I worked through these issues, I developed a better understanding of how to separate responsibilities between classes and organize the application's logic.

Improving the prompts in `Program.cs` made the application easier to use. Reworking the tests also helped me understand the Arrange, Act, Assert pattern and the importance of testing individual parts of a program.

I also gained a better understanding of Git workflow. Creating feature branches, making meaningful commits, and merging changes into `main` helped me see how version control can keep a project organized as it develops.

Overall, this assignment strengthened my understanding of C# fundamentals, software architecture, unit testing, Git workflow, and technical documentation. It also taught me the importance of understanding the code I write and making sure that my tests reflect the actual structure and behaviour of the application.

## GitHub Repository

The source code and project files are available on GitHub:

https://github.com/mrtobititiloye2748/LunchSplit
---

# Assignment 3: Payment Gateway Integration & Isolated Mocking

Assignment 3 connects LunchSplit to the **Square Payments API (Sandbox)** through a decoupled, interface-driven design. A two-tier test suite checks it. See `CODEMAP.md` for the file map used by the grading harness.

## Architecture

- **LunchSplit.Core/Payments**: the domain side. It has no vendor code.
  - `IPaymentProcessor`: gateway contract (`ProcessPaymentAsync`, `GetTransactionStatusAsync`).
  - `PaymentRequest`, `PaymentResult`, `PaymentStatus`: domain DTOs (dollars, ISO currency, idempotency key, card nonce).
  - `PaymentCoordinator`: the payment manager. It receives `IPaymentProcessor` through its constructor (Dependency Injection). It rejects $0.00 bills before any call is made, splits a bill with the A1 `Splitter`, charges each attendee, and wraps network errors in `PaymentGatewayTimeoutException`.
  - `BatchPaymentSummary`: per-attendee successes and failures for a whole-bill run.
- **LunchSplit.Infrastructure**: the only project that references the Square SDK.
  - `SquarePaymentService`: implements `IPaymentProcessor` with `client.Payments.CreateAsync` / `GetAsync`. It uses Sandbox mode with an explicit 5-second HttpClient timeout.
  - `SquareSettings`: token, location ID and timeout. These are loaded at runtime and never hard-coded.

## Test Suite

| Tier | Class | Tests |
| --- | --- | --- |
| Tier 1: live Square Sandbox | `LunchSplit.Tests/Integration/SquareSandboxTests.cs` | M3-01, M3-02, M3-08, M3-09, M3-10 |
| Tier 2: Moq in-memory | `LunchSplit.Tests/Unit/PaymentProcessorMoqTests.cs` | M3-03, M3-04, M3-05, M3-06, M3-07 |

## Configuring Square Sandbox Credentials

Credentials are **not** stored in the repository. Set them as environment variables before running the tests:

```bash
export SQUARE_SANDBOX_ACCESS_TOKEN="<sandbox access token>"
export SQUARE_SANDBOX_LOCATION_ID="<sandbox location id>"
export SQUARE_CURRENCY="CAD"   # must match the sandbox location's currency
dotnet test LunchSplit.sln
```

On Windows PowerShell, use `$env:SQUARE_SANDBOX_ACCESS_TOKEN="..."`. Alternatively, put the same keys in `LunchSplit.Tests/appsettings.Development.json`. That file is git-ignored.

If the credentials are missing, the Tier 1 tests are reported as **Inconclusive (skipped)** rather than failing. The Tier 2 Moq tests always run offline.
