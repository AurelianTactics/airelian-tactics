using System;

/// <summary>
/// Game initialization state.
/// Handles the initial setup of the game.
/// </summary>
public class GameInitState : State
{
    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public GameInitState(StateManager stateManager) : base(stateManager)
    {
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entering Game Initialization State");
        Console.WriteLine("Loading game resources...");
    }

    /// <summary>
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        base.Update();
        Console.WriteLine("Game initialization complete!");
        // Mark this state as completed
        CompleteState();
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("Exiting Game Initialization State");
        base.Exit();
    }
} 