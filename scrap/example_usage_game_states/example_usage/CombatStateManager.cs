using System;
using System.Collections.Generic;

/// <summary>
/// CombatStateManager is a specialized controller for managing combat states.
/// It demonstrates how to use the StateManager for a turn-based combat system.
/// </summary>
public class CombatStateManager
{
    /// <summary>
    /// The state manager that handles all state transitions.
    /// </summary>
    private StateManager stateManager;

    /// <summary>
    /// Enum defining the possible combat states.
    /// </summary>
    public enum CombatStateType
    {
        InitCombat,
        StartTurn,
        PlayerInput,
        EnemyTurn,
        ExecuteAction,
        ApplyEffects,
        CheckVictory,
        EndTurn,
        Victory,
        Defeat
    }

    /// <summary>
    /// Dictionary mapping CombatStateType to the actual Type of the state class.
    /// </summary>
    private Dictionary<CombatStateType, Type> stateTypes = new Dictionary<CombatStateType, Type>();

    /// <summary>
    /// Reference to the combat data that states will need to access.
    /// </summary>
    private CombatData combatData;

    /// <summary>
    /// Constructor that initializes the combat state manager.
    /// </summary>
    /// <param name="combatData">The combat data that will be shared between states.</param>
    public CombatStateManager(CombatData combatData)
    {
        this.combatData = combatData;
        
        // Initialize the state manager
        stateManager = new StateManager();

        // Register state types
        stateTypes.Add(CombatStateType.InitCombat, typeof(InitCombatState));
        stateTypes.Add(CombatStateType.StartTurn, typeof(StartTurnState));
        stateTypes.Add(CombatStateType.PlayerInput, typeof(PlayerInputState));
        stateTypes.Add(CombatStateType.EnemyTurn, typeof(EnemyTurnState));
        stateTypes.Add(CombatStateType.ExecuteAction, typeof(ExecuteActionState));
        stateTypes.Add(CombatStateType.ApplyEffects, typeof(ApplyEffectsState));
        stateTypes.Add(CombatStateType.CheckVictory, typeof(CheckVictoryState));
        stateTypes.Add(CombatStateType.EndTurn, typeof(EndTurnState));
        stateTypes.Add(CombatStateType.Victory, typeof(VictoryState));
        stateTypes.Add(CombatStateType.Defeat, typeof(DefeatState));

        // Subscribe to state change events
        stateManager.OnStateChanged += HandleStateChanged;
    }

    /// <summary>
    /// Initializes the combat by creating and registering all states.
    /// </summary>
    public void Initialize()
    {
        // Create and register all states with shared combat data
        stateManager.RegisterState(new InitCombatState(stateManager, combatData));
        stateManager.RegisterState(new StartTurnState(stateManager, combatData));
        stateManager.RegisterState(new PlayerInputState(stateManager, combatData));
        stateManager.RegisterState(new EnemyTurnState(stateManager, combatData));
        stateManager.RegisterState(new ExecuteActionState(stateManager, combatData));
        stateManager.RegisterState(new ApplyEffectsState(stateManager, combatData));
        stateManager.RegisterState(new CheckVictoryState(stateManager, combatData));
        stateManager.RegisterState(new EndTurnState(stateManager, combatData));
        stateManager.RegisterState(new VictoryState(stateManager, combatData));
        stateManager.RegisterState(new DefeatState(stateManager, combatData));

        // Set the initial state
        ChangeState(CombatStateType.InitCombat);
    }

    /// <summary>
    /// Updates the current state.
    /// </summary>
    public void Update()
    {
        // Update the current state
        stateManager.Update();
        
        // Process any automatic state transitions
        ProcessStateTransitions();
    }

    /// <summary>
    /// Changes the current state to the specified state type.
    /// </summary>
    /// <param name="stateType">The type of state to change to.</param>
    public void ChangeState(CombatStateType stateType)
    {
        if (!stateTypes.ContainsKey(stateType))
        {
            throw new ArgumentException($"State type {stateType} is not registered.");
        }

        stateManager.ChangeState(stateTypes[stateType]);
    }

    /// <summary>
    /// Gets the current state.
    /// </summary>
    /// <returns>The current state.</returns>
    public IState GetCurrentState()
    {
        return stateManager.GetCurrentState();
    }

    /// <summary>
    /// Handles state change events from the state manager.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    /// <param name="newState">The new state.</param>
    private void HandleStateChanged(IState previousState, IState newState)
    {
        Console.WriteLine($"Combat state changed from {(previousState?.GetType().Name ?? "null")} to {newState.GetType().Name}");
        
        // You can add additional logic here to handle specific state transitions
    }

    /// <summary>
    /// Processes any automatic state transitions based on the current state and combat data.
    /// This is where you centralize your state transition logic.
    /// </summary>
    private void ProcessStateTransitions()
    {
        // Get the current state
        IState currentState = stateManager.GetCurrentState();
        
        // Handle automatic transitions based on the current state
        if (currentState is InitCombatState)
        {
            // When initialization is complete, move to start turn
            if (combatData.IsInitialized)
            {
                ChangeState(CombatStateType.StartTurn);
            }
        }
        else if (currentState is StartTurnState)
        {
            // After starting the turn, go to player input or enemy turn based on whose turn it is
            if (combatData.IsTurnStarted)
            {
                if (combatData.IsPlayerTurn)
                {
                    ChangeState(CombatStateType.PlayerInput);
                }
                else
                {
                    ChangeState(CombatStateType.EnemyTurn);
                }
            }
        }
        else if (currentState is PlayerInputState)
        {
            // When player has selected an action, execute it
            if (combatData.IsActionSelected)
            {
                ChangeState(CombatStateType.ExecuteAction);
            }
        }
        else if (currentState is EnemyTurnState)
        {
            // When enemy has decided on an action, execute it
            if (combatData.IsActionSelected)
            {
                ChangeState(CombatStateType.ExecuteAction);
            }
        }
        else if (currentState is ExecuteActionState)
        {
            // After executing the action, apply any effects
            if (combatData.IsActionExecuted)
            {
                ChangeState(CombatStateType.ApplyEffects);
            }
        }
        else if (currentState is ApplyEffectsState)
        {
            // After applying effects, check for victory conditions
            if (combatData.AreEffectsApplied)
            {
                ChangeState(CombatStateType.CheckVictory);
            }
        }
        else if (currentState is CheckVictoryState)
        {
            // After checking victory conditions, either end the turn or go to victory/defeat
            if (combatData.IsVictoryCheckComplete)
            {
                if (combatData.IsPlayerVictorious)
                {
                    ChangeState(CombatStateType.Victory);
                }
                else if (combatData.IsPlayerDefeated)
                {
                    ChangeState(CombatStateType.Defeat);
                }
                else
                {
                    ChangeState(CombatStateType.EndTurn);
                }
            }
        }
        else if (currentState is EndTurnState)
        {
            // After ending the turn, start the next turn
            if (combatData.IsTurnEnded)
            {
                ChangeState(CombatStateType.StartTurn);
            }
        }
        // Victory and Defeat states don't automatically transition to other combat states
    }
}

/// <summary>
/// Class that holds all the data related to the current combat.
/// This is shared between all combat states.
/// </summary>
public class CombatData
{
    // Flags for state transitions
    public bool IsInitialized { get; set; } = false;
    public bool IsTurnStarted { get; set; } = false;
    public bool IsPlayerTurn { get; set; } = false;
    public bool IsActionSelected { get; set; } = false;
    public bool IsActionExecuted { get; set; } = false;
    public bool AreEffectsApplied { get; set; } = false;
    public bool IsVictoryCheckComplete { get; set; } = false;
    public bool IsPlayerVictorious { get; set; } = false;
    public bool IsPlayerDefeated { get; set; } = false;
    public bool IsTurnEnded { get; set; } = false;

    // Combat-related data
    public List<Unit> PlayerUnits { get; set; } = new List<Unit>();
    public List<Unit> EnemyUnits { get; set; } = new List<Unit>();
    public Unit CurrentUnit { get; set; }
    public Action SelectedAction { get; set; }
    public Unit TargetUnit { get; set; }
    
    // Reset all flags
    public void ResetFlags()
    {
        IsInitialized = false;
        IsTurnStarted = false;
        IsActionSelected = false;
        IsActionExecuted = false;
        AreEffectsApplied = false;
        IsVictoryCheckComplete = false;
        IsTurnEnded = false;
    }
}

// Example combat state classes - these would be in separate files in a real project
public class InitCombatState : State
{
    protected CombatData combatData;
    
    public InitCombatState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Initializing combat...");
        
        // Reset all flags
        combatData.ResetFlags();
        
        // Initialize combat data
        // In a real game, this would set up the battlefield, units, etc.
        
        // Mark initialization as complete
        combatData.IsInitialized = true;
    }
}

public class StartTurnState : State
{
    protected CombatData combatData;
    
    public StartTurnState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Starting new turn...");
        
        // Reset turn-related flags
        combatData.IsTurnStarted = false;
        combatData.IsActionSelected = false;
        combatData.IsActionExecuted = false;
        combatData.AreEffectsApplied = false;
        combatData.IsVictoryCheckComplete = false;
        combatData.IsTurnEnded = false;
        
        // Determine whose turn it is
        // In a real game, this would be based on initiative, turn order, etc.
        combatData.IsPlayerTurn = !combatData.IsPlayerTurn; // Alternate between player and enemy
        
        // Mark turn as started
        combatData.IsTurnStarted = true;
    }
}

public class PlayerInputState : State
{
    protected CombatData combatData;
    
    public PlayerInputState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Waiting for player input...");
        
        // In a real game, this would enable UI for player input
        // For this example, we'll simulate player input after a delay
        
        // Simulated player input (would be triggered by actual player input in a real game)
        // combatData.SelectedAction = new Attack();
        // combatData.TargetUnit = combatData.EnemyUnits[0];
        // combatData.IsActionSelected = true;
    }
    
    // In a real game, you would have methods here to handle player input events
}

public class EnemyTurnState : State
{
    protected CombatData combatData;
    
    public EnemyTurnState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Enemy is deciding action...");
        
        // In a real game, this would run AI to determine the enemy's action
        // For this example, we'll simulate enemy decision after a delay
        
        // Simulated enemy decision
        // combatData.SelectedAction = new Attack();
        // combatData.TargetUnit = combatData.PlayerUnits[0];
        // combatData.IsActionSelected = true;
    }
}

public class ExecuteActionState : State
{
    protected CombatData combatData;
    
    public ExecuteActionState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Executing action...");
        
        // In a real game, this would animate the action and calculate results
        // For this example, we'll simulate action execution
        
        // Simulated action execution
        // combatData.SelectedAction.Execute(combatData.TargetUnit);
        combatData.IsActionExecuted = true;
    }
}

public class ApplyEffectsState : State
{
    protected CombatData combatData;
    
    public ApplyEffectsState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Applying effects...");
        
        // In a real game, this would apply status effects, damage over time, etc.
        // For this example, we'll simulate applying effects
        
        // Simulated effect application
        combatData.AreEffectsApplied = true;
    }
}

public class CheckVictoryState : State
{
    protected CombatData combatData;
    
    public CheckVictoryState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Checking victory conditions...");
        
        // In a real game, this would check if all enemies are defeated or all players are defeated
        // For this example, we'll simulate victory check
        
        // Simulated victory check
        // combatData.IsPlayerVictorious = combatData.EnemyUnits.All(u => u.IsDead);
        // combatData.IsPlayerDefeated = combatData.PlayerUnits.All(u => u.IsDead);
        combatData.IsVictoryCheckComplete = true;
    }
}

public class EndTurnState : State
{
    protected CombatData combatData;
    
    public EndTurnState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Ending turn...");
        
        // In a real game, this would clean up turn-related data and prepare for the next turn
        // For this example, we'll simulate turn end
        
        // Simulated turn end
        combatData.IsTurnEnded = true;
    }
}

public class VictoryState : State
{
    protected CombatData combatData;
    
    public VictoryState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Victory! Player has won the combat.");
        
        // In a real game, this would show victory UI, give rewards, etc.
    }
}

public class DefeatState : State
{
    protected CombatData combatData;
    
    public DefeatState(StateManager stateManager, CombatData combatData) : base(stateManager)
    {
        this.combatData = combatData;
    }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Defeat! Player has lost the combat.");
        
        // In a real game, this would show defeat UI, game over screen, etc.
    }
}

// Placeholder classes for the example
public class Unit { }
public class Action { public void Execute(Unit target) { } } 