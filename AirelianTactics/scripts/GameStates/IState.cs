using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface for all game states in the state machine.
/// States represent different modes or phases of gameplay.
/// Each state handles its own logic and transitions.
/// </summary>
public interface IState
{
    /// <summary>
    /// Gets or sets the shared game context containing data across all states.
    /// </summary>
    GameContext GameContext { get; set; }

    /// <summary>
    /// Called when the state is entered.
    /// Use this for initialization logic.
    /// </summary>
    void Enter();

    /// <summary>
    /// Called when the state is exited.
    /// Use this for cleanup logic.
    /// </summary>
    void Exit();

    /// <summary>
    /// Called to update the state.
    /// This is where the main logic of the state is executed.
    /// </summary>
    void Update();
} 