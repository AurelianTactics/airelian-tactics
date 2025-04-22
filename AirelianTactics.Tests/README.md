# AirelianTactics.Tests

This directory contains unit tests for the AirelianTactics game state management system.

## Project Structure

```
AirelianTactics.Tests/
├── AirelianTactics.Tests.csproj    # Test project file
├── GameStates/                     # Tests for game state classes
│   ├── StateManagerTests.cs        # Tests for StateManager
│   ├── StateTests.cs               # Tests for State base class
│   └── IStateTests.cs              # Tests for IState interface
└── RunTests.cs                     # Example of programmatically running tests
```

## Running Tests

### Using Visual Studio

1. Open the solution in Visual Studio
2. Right-click on the test project in Solution Explorer
3. Select "Run Tests"

### Using .NET CLI

1. Navigate to the test project directory in your terminal:
   ```
   cd AirelianTactics.Tests
   ```

2. Run all tests:
   ```
   dotnet test
   ```

3. Run specific tests:
   ```
   dotnet test --filter "FullyQualifiedName~StateManagerTests"
   ```

4. Run with detailed output:
   ```
   dotnet test --logger "console;verbosity=detailed"
   ```

### Manual Run (Using the RunTests class)

1. Update the project file to set RunTests as the entry point
2. Execute:
   ```
   dotnet run
   ```

## Test Naming Conventions

Tests follow the naming convention: `MethodName_Scenario_ExpectedBehavior`

For example:
- `RegisterState_StateIsRegistered`
- `ChangeState_StateChanges_EnterAndExitMethodsCalled`

## Code Coverage

To run tests with code coverage:

```
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```

This generates a coverage report in lcov format that can be viewed using various tools.

## Continuous Integration

These tests can be integrated into a CI pipeline by adding the `dotnet test` command to your build script or GitHub Actions workflow. 