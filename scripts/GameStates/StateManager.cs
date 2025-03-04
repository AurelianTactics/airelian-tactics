using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// StateManager handles the transitions between different game states.
/// It maintains a reference to the current state and manages the state transitions.
/// </summary>
public class StateManager
{
    /// <summary>
    /// The currently active state.
    /// </summary>
    private IState currentState;

    /// <summary>
    /// Dictionary of all registered states, mapped by their type.
    /// </summary>
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    /// <summary>
    /// Event triggered when a state transition occurs.
    /// Provides the previous state and the new state.
    /// </summary>
    public event Action<IState, IState> OnStateChanged;

    /// <summary>
    /// Registers a state with the state manager.
    /// </summary>
    /// <param name="state">The state to register.</param>
    public void RegisterState(IState state)
    {
        Type stateType = state.GetType();
        
        if (states.ContainsKey(stateType))
        {
            // Replace existing state if it's already registered
            states[stateType] = state;
        }
        else
        {
            // Add new state
            states.Add(stateType, state);
        }
    }

    /// <summary>
    /// Changes to a new state of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of state to change to.</typeparam>
    public void ChangeState<T>() where T : IState
    {
        ChangeState(typeof(T));
    }

    /// <summary>
    /// Changes to a new state of the specified type.
    /// </summary>
    /// <param name="stateType">The type of state to change to.</param>
    public void ChangeState(Type stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            throw new ArgumentException($"State of type {stateType.Name} is not registered with the StateManager.");
        }

        IState newState = states[stateType];
        
        // Don't change if it's the same state
        if (currentState == newState)
        {
            return;
        }

        // Exit the current state if it exists
        IState previousState = currentState;
        if (currentState != null)
        {
            currentState.Exit();
        }

        // Set and enter the new state
        currentState = newState;
        currentState.Enter();

        // Trigger the state changed event
        OnStateChanged?.Invoke(previousState, currentState);
    }

    /// <summary>
    /// Updates the current state.
    /// </summary>
    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    /// <summary>
    /// Gets the current state.
    /// </summary>
    /// <returns>The current state.</returns>
    public IState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Gets a registered state of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of state to get.</typeparam>
    /// <returns>The state of the specified type.</returns>
    public T GetState<T>() where T : IState
    {
        Type stateType = typeof(T);
        if (!states.ContainsKey(stateType))
        {
            throw new ArgumentException($"State of type {stateType.Name} is not registered with the StateManager.");
        }

        return (T)states[stateType];
    }
} 