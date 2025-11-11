# Copilot Instructions for OpenMeteo .NET Client SDK

## Project Overview
This repository contains a .NET8 SDK for interacting with the OpenMeteo API. It includes core library code and unit tests.

## Language & Framework
- Language: C#12
- Target Framework: .NET8

## Test Framework
- **MSTest** is used for unit testing.
 - Test classes are annotated with `[TestClass]`
 - Test methods use `[TestMethod]`
 - Assertions use `Assert.*` methods

## Directory Structure
- `OpenMeteo/` — Main SDK source code
- `OpenMeteoTests/` — Unit tests for SDK features

## Coding Conventions
- Use file-scoped namespaces
- Prefer concise, modern C# syntax (e.g., collection expressions, null checks)
- Organize helpers and extensions in the `Helpers` directory
- Place tests in corresponding subfolders under `OpenMeteoTests`

## Test Discovery
- All test files are located in `OpenMeteoTests/`
- Each test class should target a specific feature or helper
- Use MSTest attributes for test organization

## How to Run Tests
- Use Visual Studio Test Explorer or run `dotnet test` from the solution directory

## Adding New Tests
- Create new test classes in `OpenMeteoTests/` with `[TestClass]`
- Name test methods descriptively and annotate with `[TestMethod]`
- Use MSTest `Assert` methods for validation

## Example MSTest Usage
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ExampleTests
{
 [TestMethod]
 public void TestSomething()
 {
 Assert.AreEqual(1,1);
 }
}
```

## Additional Guidance
- Keep code and tests up to date with .NET8 and C#12 features
- Follow best practices for null handling and type conversions
- Ensure all public helpers have corresponding unit tests
