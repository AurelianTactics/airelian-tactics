# State Management System

## Overview

The state management system in Airelian Tactics provides a structured approach to manage different game states and transitions between them. The system consists of three key components:

1. **StateManager**: The central component that manages state transitions and maintains the current state.
2. **IState**: The interface that all game states must implement.
3. **State**: A base abstract class that implements the `IState` interface with common functionality.

## If Creating a New State -- Checklist

1. Create a new class that extends `State`
2. Implement `Enter()`, `Update()`, and `Exit()` methods
3. Register the state with `stateManager.RegisterState(yourState)`
4. Define transitions with `stateManager.DefineStateFlow<FromState, YourState>()` and `stateManager.DefineStateFlow<YourState, ToState>()` in the file that handles the state flow
5. Call `CompleteState()` when your state's work is done

## Components

### StateManager

The `StateManager` controls the flow between different game states and manages state transitions. It:

- Maintains a registry of available states
- Defines and enforces the flow between states
- Handles state transitions
- Updates the current active state

Key features:
- Register states with `RegisterState(IState state)`
- Define state transitions with `DefineStateFlow<TFrom, TTo>()`
- Change states with `ChangeState<T>()` 
- Update the current state with `Update()`
- Mark the current state as completed with `CompleteCurrentState()`

### IState Interface

The `IState` interface defines the contract that all game states must fulfill:

- `Enter()`: Called when entering the state
- `Exit()`: Called when exiting the state
- `Update()`: Called to update the state logic

### State Base Class

The `State` abstract class provides common functionality for game states:

- Tracks completion status with `IsCompleted`
- Manages event listeners with `AddListeners()` and `RemoveListeners()`
- Provides state completion with `CompleteState()`
- Simplifies state transitions with `ChangeState<T>()`

## Usage

### Creating a New State

To create a new game state:

```csharp
public class GameInitState : State
{
    public GameInitState(StateManager stateManager) : base(stateManager) { }

    public override void Enter()
    {
        base.Enter();
        // Initialization logic
    }

    public override void Update()
    {
        // State-specific logic
        
        // When the state is done
        CompleteState();
    }

    public override void Exit()
    {
        // Cleanup logic
        base.Exit();
    }
}
```

### Registering States

Register states with the `StateManager`:

```csharp
StateManager stateManager = new StateManager();

// Create and register states
GameInitState initState = new GameInitState(stateManager);
BattleState battleState = new BattleState(stateManager);
ResultsState resultsState = new ResultsState(stateManager);

stateManager.RegisterState(initState);
stateManager.RegisterState(battleState);
stateManager.RegisterState(resultsState);
```

### Defining State Flow

Define the flow between states:

```csharp
// Define the flow: Init -> Battle -> Results
stateManager.DefineStateFlow<GameInitState, BattleState>();
stateManager.DefineStateFlow<BattleState, ResultsState>();
```

### Starting the State Machine

Start the state machine by changing to the initial state:

```csharp
stateManager.ChangeState<GameInitState>();
```

### Updating the State Machine

Update the state machine regularly:

```csharp
// In your game loop
stateManager.Update();
```

## Best Practices

1. **Single Responsibility**: Each state should handle one specific phase of the game
2. **Clean Transitions**: Use `Enter()` and `Exit()` to handle setup and cleanup
3. **Event-Based Communication**: Use events to communicate between states
4. **State Completion**: Mark states as completed using `CompleteState()` to trigger automatic transitions
5. **State Data**: Pass data between states through a shared context object if needed 