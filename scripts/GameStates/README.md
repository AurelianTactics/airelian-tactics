# State Machine System

This directory contains a simple state machine implementation for managing game states.

## Overview

The state machine consists of the following components:

1. **IState Interface**: Defines the contract for all states.
2. **State Base Class**: Provides common functionality for all states.
3. **StateManager**: Handles state transitions and maintains the current state.
4. **Example State**: Demonstrates how to implement a concrete state.

## How to Use

### Creating a New State

1. Create a new class that inherits from `State`.
2. Implement the required methods: `Enter()`, `Exit()`, and `Update()`.
3. Add any state-specific logic and event handlers.

Example:

```csharp
public class MyGameState : State
{
    public MyGameState(StateManager stateManager) : base(stateManager)
    {
        // Constructor logic
    }

    public override void Enter()
    {
        base.Enter();
        // State initialization logic
    }

    public override void Exit()
    {
        // State cleanup logic
        base.Exit();
    }

    public override void Update()
    {
        // State update logic
        
        // Example of transitioning to another state
        if (someCondition)
        {
            ChangeState<AnotherState>();
        }
    }
}
```

### Setting Up the State Manager

1. Create a StateManager instance.
2. Register all your states with the manager.
3. Set the initial state.
4. Call the Update method in your game loop.

Example:

```csharp
// Create the state manager
StateManager stateManager = new StateManager();

// Create and register states
stateManager.RegisterState(new MainMenuState(stateManager));
stateManager.RegisterState(new GameplayState(stateManager));
stateManager.RegisterState(new PauseState(stateManager));

// Set the initial state
stateManager.ChangeState<MainMenuState>();

// In your game loop
void Update()
{
    stateManager.Update();
}
```

### State Transitions

To transition from one state to another, call the `ChangeState<T>()` method from within a state:

```csharp
// Inside a state method
ChangeState<GameplayState>();
```

Or from outside a state using the StateManager:

```csharp
stateManager.ChangeState<GameplayState>();
```

## Best Practices

1. Always call `base.Enter()` at the beginning of your `Enter()` method.
2. Always call `base.Exit()` at the end of your `Exit()` method.
3. Keep states focused on a single responsibility.
4. Use the state's constructor for one-time setup and the `Enter()` method for initialization that needs to happen each time the state is entered.
5. Clean up resources in the `Exit()` method to prevent memory leaks.
6. Use event listeners to respond to game events rather than polling in the `Update()` method when possible. 