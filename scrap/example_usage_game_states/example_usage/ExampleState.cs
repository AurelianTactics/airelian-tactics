using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Example state that demonstrates how to implement a concrete state.
/// This is a template that can be used as a reference for creating other states.
/// </summary>
public class ExampleState : State
{
    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public ExampleState(StateManager stateManager) : base(stateManager)
    {
        // Initialization that doesn't depend on Enter being called
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public override void Enter()
    {
        base.Enter(); // Important to call base.Enter() to ensure AddListeners() is called
        
        Console.WriteLine("Entered ExampleState");
        
        // Initialize state-specific variables
        // Set up UI elements
        // Start any coroutines or timers
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("Exited ExampleState");
        
        // Clean up state-specific variables
        // Hide UI elements
        // Stop any coroutines or timers
        
        base.Exit(); // Important to call base.Exit() to ensure RemoveListeners() is called
    }

    /// <summary>
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        // Main logic of the state
        // Check for conditions to transition to other states
        
        // Example of a state transition:
        // if (someCondition)
        // {
        //     ChangeState<AnotherState>();
        // }
    }

    /// <summary>
    /// Adds event listeners for this state.
    /// </summary>
    protected override void AddListeners()
    {
        base.AddListeners();
        
        // Add event listeners specific to this state
        // Example:
        // InputController.moveEvent += OnMove;
        // InputController.fireEvent += OnFire;
    }

    /// <summary>
    /// Removes event listeners for this state.
    /// </summary>
    protected override void RemoveListeners()
    {
        // Remove event listeners specific to this state
        // Example:
        // InputController.moveEvent -= OnMove;
        // InputController.fireEvent -= OnFire;
        
        base.RemoveListeners();
    }

    // Example event handlers:
    // private void OnMove(object sender, EventArgs e)
    // {
    //     // Handle move event
    // }
    //
    // private void OnFire(object sender, EventArgs e)
    // {
    //     // Handle fire event
    // }
} 