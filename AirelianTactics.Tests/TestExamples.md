# Running Tests for AirelianTactics State Management

This document provides examples for running the unit tests created for the State Management system.

## Test Project Structure

The test project follows standard C# best practices, with:

- Test classes organized in folders mirroring the main project structure
- Tests following the naming convention `MethodName_Scenario_ExpectedBehavior`
- Using the MSTest framework for test organization and assertions
- Using Moq for mocking dependencies where appropriate

## Running Tests Using Visual Studio

1. Open the solution in Visual Studio
2. Open the Test Explorer (Test > Test Explorer)
3. Click "Run All Tests" or select specific tests to run

![Visual Studio Test Explorer](https://docs.microsoft.com/en-us/visualstudio/test/media/vs-2022/test-explorer-window.png?view=vs-2022)

## Running Tests Using .NET CLI

### Run All Tests

```powershell
# From the solution root
dotnet test AirelianTactics.Tests

# From within the test project directory
cd AirelianTactics.Tests
dotnet test
```

### Run Specific Tests

```powershell
# Run tests that match a filter
dotnet test --filter "FullyQualifiedName~StateManagerTests"

# Run a specific test
dotnet test --filter "FullyQualifiedName=AirelianTactics.Tests.GameStates.StateManagerTests.RegisterState_StateIsRegistered"
```

### Run with Additional Options

```powershell
# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```

## Test Examples

### StateManager Tests

```csharp
// Test that registering a state works correctly
[TestMethod]
public void RegisterState_StateIsRegistered()
{
    // Create a concrete test state
    var concreteState = new TestState1();
    
    // Act
    stateManager.RegisterState(concreteState);
    
    // Assert
    Assert.AreEqual(concreteState, stateManager.GetState<TestState1>());
}

// Test that state flow transitions work correctly
[TestMethod]
public void DefineStateFlow_StateFlowDefined_TransitionsAutomatically()
{
    // Arrange
    var completableState = new CompletableState(stateManager);
    var nextState = new TestState2();
    stateManager.RegisterState(completableState);
    stateManager.RegisterState(nextState);
    stateManager.DefineStateFlow<CompletableState, TestState2>();
    
    // Act
    stateManager.ChangeState<CompletableState>();
    completableState.Complete(); // Mark the state as completed
    stateManager.Update(); // Trigger state transition
    
    // Assert
    Assert.AreEqual(typeof(TestState2), stateManager.GetCurrentState().GetType());
}
```

### State Tests

```csharp
// Test that the enter method initializes the state correctly
[TestMethod]
public void Enter_SetsIsCompletedToFalse_CallsAddListeners()
{
    // Arrange
    testState.IsListenerAdded = false;
    testState.SetIsCompleted(true);
    
    // Act
    testState.Enter();
    
    // Assert
    Assert.IsFalse(testState.IsCompleted);
    Assert.IsTrue(testState.IsListenerAdded);
}
```

## Continuous Integration

These tests can be easily integrated into a CI/CD pipeline:

```yaml
# Example GitHub Actions workflow
name: Run Tests

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 9.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
``` 