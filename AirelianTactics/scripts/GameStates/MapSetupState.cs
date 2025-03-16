using System;

/// <summary>
/// Map setup state.
/// Handles the setup of the game map.
/// </summary>
public class MapSetupState : State
{
    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public MapSetupState(StateManager stateManager) : base(stateManager)
    {
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entering Map Setup State");
        Console.WriteLine("Setting up the game map...");
    }

    /// <summary>
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        base.Update();
        Console.WriteLine("Map setup complete!");
        // Mark this state as completed
        CompleteState();
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("Exiting Map Setup State");
        base.Exit();
    }
} 