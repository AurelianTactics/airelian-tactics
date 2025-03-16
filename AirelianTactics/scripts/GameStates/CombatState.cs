using System;

/// <summary>
/// Combat state.
/// Handles the combat phase of the game.
/// </summary>
public class CombatState : State
{
    private bool hasCompletedCombat = false;

    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public CombatState(StateManager stateManager) : base(stateManager)
    {
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        hasCompletedCombat = false;
        Console.WriteLine("Entering Combat State");
        Console.WriteLine("Combat has begun!");
    }

    /// <summary>
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        if (!hasCompletedCombat)
        {
            Console.WriteLine("Combat in progress...");
            Console.WriteLine("Combat complete!");
            hasCompletedCombat = true;
            // Mark this state as completed
            CompleteState();
        }
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("Exiting Combat State");
        base.Exit();
    }
} 