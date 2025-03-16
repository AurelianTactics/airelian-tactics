using System;

/// <summary>
/// Game end state.
/// Handles the end of the game.
/// </summary>
public class GameEndState : State
{
    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public GameEndState(StateManager stateManager) : base(stateManager)
    {
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entering Game End State");
        Console.WriteLine("Game has ended!");
    }

    /// <summary>
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        base.Update();
        // This is the final state, so we don't need to complete it
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("Exiting Game End State");
        base.Exit();
    }
} 