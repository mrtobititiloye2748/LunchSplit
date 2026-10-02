# LunchSplit
CSCN72010 – Software Quality I – Assignment 1 (TDD + MSTest)

A multi‑project .NET console application for splitting a restaurant bill evenly among multiple people.

---

## Overview

LunchSplit is a simple bill‑splitting application built using a clean multi‑project architecture:

- **LunchSplit.Core** — business logic  
- **LunchSplit.App** — console UI  
- **LunchSplit.Tests** — MSTest unit tests  

The app collects user input (subtotal, tip percentage, number of people), validates it, calculates the total bill including tip, splits it evenly, rounds values to two decimals, and displays formatted results.

---

## Project Structure

LunchSplit/
├── LunchSplit.App/
│    └── Program.cs
├── LunchSplit.Core/
│    ├── Bill.cs
│    ├── Share.cs
│    ├── Splitter.cs
│    └── Formatter.cs
└── LunchSplit.Tests/
├── BillTests.cs
├── SplitterTests.cs
└── FormatterTests.cs

---

## Source Code

### LunchSplit.Core

#### Bill.cs
```csharp
namespace LunchSplit.Core;

public class Bill
{
    public decimal Subtotal { get; }
    public decimal TipPercent { get; }
    public decimal Tip => Math.Round(Subtotal * (TipPercent / 100m), 2);
    public decimal Total => Math.Round(Subtotal + Tip, 2);

    public Bill(decimal subtotal, decimal tipPercent)
    {
        Subtotal = subtotal;
        TipPercent = tipPercent;
    }
}

Share.cs
csharp
namespace LunchSplit.Core;

public class Share
{
    public int PersonNumber { get; }
    public decimal Amount { get; }

    public Share(int personNumber, decimal amount)
    {
        PersonNumber = personNumber;
        Amount = Math.Round(amount, 2);
    }
}
Splitter.cs
csharp
namespace LunchSplit.Core;

public class Splitter
{
    public List<Share> Split(Bill bill, int people)
    {
        var shares = new List<Share>();
        decimal perPerson = bill.Total / people;

        for (int i = 1; i <= people; i++)
            shares.Add(new Share(i, perPerson));

        return shares;
    }
}
Formatter.cs
csharp
namespace LunchSplit.Core;

public static class Formatter
{
    public static string FormatShare(Share share)
    {
        return $"Person {share.PersonNumber} owes {share.Amount:C}";
    }
}
LunchSplit.App
Program.cs
csharp
using LunchSplit.Core;

Console.WriteLine("=== LunchSplit ===");

decimal subtotal;
while (true)
{
    Console.Write("Enter subtotal: ");
    if (decimal.TryParse(Console.ReadLine(), out subtotal) && subtotal >= 0)
        break;

    Console.WriteLine("Invalid subtotal. Please enter a non-negative number.");
}

decimal tipPercent;
while (true)
{
    Console.Write("Enter tip percentage: ");
    if (decimal.TryParse(Console.ReadLine(), out tipPercent) && tipPercent >= 0)
        break;

    Console.WriteLine("Invalid tip percentage. Please enter a non-negative number.");
}

int people;
while (true)
{
    Console.Write("Enter number of people: ");
    if (int.TryParse(Console.ReadLine(), out people) && people > 0)
        break;

    Console.WriteLine("Invalid number. Must be at least 1.");
}

var bill = new Bill(subtotal, tipPercent);
var splitter = new Splitter();
var shares = splitter.Split(bill, people);

Console.WriteLine("\n--- Split Results ---");
foreach (var share in shares)
{
    Console.WriteLine(Formatter.FormatShare(share));
}
LunchSplit.Tests
BillTests.cs
csharp
using LunchSplit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class BillTests
{
    [TestMethod]
    public void Bill_ComputesTipCorrectly()
    {
        var bill = new Bill(100m, 15m);
        Assert.AreEqual(15m, bill.Tip);
    }

    [TestMethod]
    public void Bill_ComputesTotalCorrectly()
    {
        var bill = new Bill(100m, 20m);
        Assert.AreEqual(120m, bill.Total);
    }
}
SplitterTests.cs
csharp
using LunchSplit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class SplitterTests
{
    [TestMethod]
    public void Splitter_SplitsEvenly()
    {
        var bill = new Bill(100m, 0m);
        var splitter = new Splitter();

        var shares = splitter.Split(bill, 4);

        Assert.AreEqual(4, shares.Count);
        Assert.AreEqual(25m, shares[0].Amount);
    }

    [TestMethod]
    public void Splitter_SplitsWithTip()
    {
        var bill = new Bill(100m, 10m);
        var splitter = new Splitter();

        var shares = splitter.Split(bill, 2);

        Assert.AreEqual(55m, shares[0].Amount);
        Assert.AreEqual(55m, shares[1].Amount);
    }
}
FormatterTests.cs
csharp
using LunchSplit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class FormatterTests
{
    [TestMethod]
    public void Formatter_FormatsShareCorrectly()
    {
        var share = new Share(1, 25.5m);
        string result = Formatter.FormatShare(share);

        Assert.AreEqual("Person 1 owes $25.50", result);
    }
}

Running the Application

Run the console app:

cd LunchSplit.App

dotnet run

Run all tests:

dotnet test

Example Output

=== LunchSplit ===
Enter subtotal: 100
Enter tip percentage: 15
Enter number of people: 4

--- Split Results ---
Person 1 owes $28.75
Person 2 owes $28.75
Person 3 owes $28.75
Person 4 owes $28.75