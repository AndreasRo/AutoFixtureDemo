# AutoFixture Demo Code

This repository contains demo code for the talk "Generating test data with [AutoFixture](https://github.com/AutoFixture/AutoFixture)" by Andreas Rodtwitt.

It is a step-by-step demo where separate branches of the code represent different stages in integrating AutoFixture into your tests. The demo starts in the `master` branch and then continues with branches such as `demo/branch1`, `demo/branch2`, etc.

## The Test Scenario

The system under test is an expanded version of the Visual Studio ASP.NET Core Web API demo project "Weather Forecast". It contains a single endpoint for retrieving weather forecasts for a specific location.

There are three unit tests in the test project:
- One controller-level test covering the happy path
- Two service-layer tests covering the happy path and a null result, respectively

## Branch Walkthrough

### 1: Master

The `master` branch contains a standard test setup without AutoFixture. All test data is set up at the beginning of each test.

### 2: Demo/branch1

In this branch, the test data setup has been replaced with AutoFixture. Instead of using constructors, we use the AutoFixture `Fixture` object to create test data. The setup code is still located at the beginning of each test, so the overall complexity remains more or less the same.

**What have we gained?**  
If the data objects created for tests change, it will in most cases not require updates to the test data setup.

### 3: Demo/branch2

We have added a new class, `DefaultTestCustomization` (poor naming, but still...). The customization class must inherit from AutoFixture’s `ICustomization` interface in order to work as expected.

All AutoFixture setup code has been moved from the individual tests into this centralized class. In our tests, we only need to create a `Fixture` object and apply our customization. We are then ready to generate usable test data.

**What have we gained?**  
Simplified test setup. We now have a centralized class for test data configuration that can be reused across tests.

### 4: Demo/branch3

We have added another class, `EntityTestCustomization`, which contains all AutoFixture setup related to the repository layer.

AutoFixture’s ability to register customizations within other customizations is used here by registering `DefaultTestCustomization` inside this class. `DefaultTestCustomization` now only retains setup related to domain primitives.

This customization hierarchy helps prevent classes from becoming too large and supports better separation of concerns. We chose not to create a `DomainObjectCustomization`, since creating domain objects does not require special setup beyond domain primitives.

We have also created inline data attributes and moved the creation of fixtures and registration of customizations out of the tests. This allows us to create test data inline.

In addition, we introduced an example behaviour class, `BogusBehavior`. Behaviours are similar to customizations, but instead of defining setup per class, they define setup across all classes. Our example behaviour duplicates the setup for `DateOnly` and text primitives.

**What have we gained?**  
Even simpler test setup. Test data can now be created inline and modified within the test when needed.

### 5: Demo/branch4

In this branch, AutoFixture is used as a dependency injection container. The setup of the object under test and its dependencies is added directly to the AutoFixture configuration.

This enables inline creation of fully configured test objects. Assertions on mocked dependencies can also be performed inline.

**What have we gained?**  
An extremely minimal test setup, though potentially at the expense of readability.

