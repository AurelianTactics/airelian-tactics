using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all game states.
/// Provides common functionality and implements the IState interface.
/// </summary>
public abstract class State : IState
{
    /// <summary>
    /// Reference to the state manager that manages this state.
    /// </summary>
    protected StateManager stateManager;

    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public State(StateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    /// <summary>
    /// Called when the state is entered.
    /// Override this method to provide initialization logic.
    /// </summary>
    public virtual void Enter()
    {
        AddListeners();
    }

    /// <summary>
    /// Called when the state is exited.
    /// Override this method to provide cleanup logic.
    /// </summary>
    public virtual void Exit()
    {
        RemoveListeners();
    }

    /// <summary>
    /// Called to update the state.
    /// Override this method to provide the main logic of the state.
    /// </summary>
    public virtual void Update()
    {
        // Default implementation does nothing
    }

    /// <summary>
    /// Adds event listeners for this state.
    /// Override this method to add specific listeners.
    /// </summary>
    protected virtual void AddListeners()
    {
        // Default implementation does nothing
    }

    /// <summary>
    /// Removes event listeners for this state.
    /// Override this method to remove specific listeners.
    /// </summary>
    protected virtual void RemoveListeners()
    {
        // Default implementation does nothing
    }

    /// <summary>
    /// Changes to a new state of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of state to change to.</typeparam>
    protected void ChangeState<T>() where T : IState
    {
        stateManager.ChangeState<T>();
    }
} 