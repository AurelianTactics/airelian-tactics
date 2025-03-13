using System;
using System.Collections.Generic;

/// <summary>
/// GameCore is the main controller class that manages the game loop and state transitions.
/// It demonstrates how to use the StateManager to centralize state transition logic.
/// </summary>
public class GameCore
{
    /// <summary>
    /// The state manager that handles all state transitions.
    /// </summary>
    private StateManager stateManager;

    /// <summary>
    /// Flag to control the game loop.
    /// </summary>
    private bool isRunning = false;

    /// <summary>
    /// Enum defining the possible game states.
    /// </summary>
    public enum GameStateType
    {
        MainMenu,
        Loading,
        Gameplay,
        Combat,
        Pause,
        GameOver
    }

    /// <summary>
    /// Dictionary mapping GameStateType to the actual Type of the state class.
    /// </summary>
    private Dictionary<GameStateType, Type> stateTypes = new Dictionary<GameStateType, Type>();

    /// <summary>
    /// Constructor that initializes the game core.
    /// </summary>
    public GameCore()
    {
        // Initialize the state manager
        stateManager = new StateManager();

        // Register state types
        stateTypes.Add(GameStateType.MainMenu, typeof(MainMenuState));
        stateTypes.Add(GameStateType.Loading, typeof(LoadingState));
        stateTypes.Add(GameStateType.Gameplay, typeof(GameplayState));
        stateTypes.Add(GameStateType.Combat, typeof(CombatState));
        stateTypes.Add(GameStateType.Pause, typeof(PauseState));
        stateTypes.Add(GameStateType.GameOver, typeof(GameOverState));

        // Subscribe to state change events
        stateManager.OnStateChanged += HandleStateChanged;
    }

    /// <summary>
    /// Initializes the game by creating and registering all states.
    /// </summary>
    public void Initialize()
    {
        // Create and register all states
        stateManager.RegisterState(new MainMenuState(stateManager));
        stateManager.RegisterState(new LoadingState(stateManager));
        stateManager.RegisterState(new GameplayState(stateManager));
        stateManager.RegisterState(new CombatState(stateManager));
        stateManager.RegisterState(new PauseState(stateManager));
        stateManager.RegisterState(new GameOverState(stateManager));

        // Set the initial state
        ChangeState(GameStateType.MainMenu);
    }

    /// <summary>
    /// Starts the game loop.
    /// </summary>
    public void Start()
    {
        if (isRunning)
            return;

        isRunning = true;
        GameLoop();
    }

    /// <summary>
    /// Stops the game loop.
    /// </summary>
    public void Stop()
    {
        isRunning = false;
    }

    /// <summary>
    /// The main game loop that updates the current state.
    /// In a real game, this would be called by your game engine's update loop.
    /// </summary>
    private void GameLoop()
    {
        // This is a simplified game loop for demonstration purposes
        // In a real game, this would be integrated with your game engine's update cycle
        while (isRunning)
        {
            // Update the current state
            stateManager.Update();

            // Process any pending state transitions
            ProcessStateTransitions();

            // Simulate a frame delay
            System.Threading.Thread.Sleep(16); // ~60 FPS
        }
    }

    /// <summary>
    /// Changes the current state to the specified state type.
    /// </summary>
    /// <param name="stateType">The type of state to change to.</param>
    public void ChangeState(GameStateType stateType)
    {
        if (!stateTypes.ContainsKey(stateType))
        {
            throw new ArgumentException($"State type {stateType} is not registered.");
        }

        stateManager.ChangeState(stateTypes[stateType]);
    }

    /// <summary>
    /// Handles state change events from the state manager.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    /// <param name="newState">The new state.</param>
    private void HandleStateChanged(IState previousState, IState newState)
    {
        Console.WriteLine($"State changed from {(previousState?.GetType().Name ?? "null")} to {newState.GetType().Name}");
        
        // You can add additional logic here to handle specific state transitions
    }

    /// <summary>
    /// Processes any pending state transitions based on game conditions.
    /// This is where you centralize your state transition logic.
    /// </summary>
    private void ProcessStateTransitions()
    {
        // Get the current state
        IState currentState = stateManager.GetCurrentState();
        
        // Example of centralized state transition logic
        if (currentState is MainMenuState)
        {
            // Check if the player has selected to start the game
            if (IsStartGameSelected())
            {
                ChangeState(GameStateType.Loading);
                return;
            }
        }
        else if (currentState is LoadingState)
        {
            // Check if loading is complete
            if (IsLoadingComplete())
            {
                ChangeState(GameStateType.Gameplay);
                return;
            }
        }
        else if (currentState is GameplayState)
        {
            // Check if the player has entered combat
            if (HasEnteredCombat())
            {
                ChangeState(GameStateType.Combat);
                return;
            }
            
            // Check if the player has paused the game
            if (IsPauseRequested())
            {
                ChangeState(GameStateType.Pause);
                return;
            }
            
            // Check if the player has died
            if (IsPlayerDefeated())
            {
                ChangeState(GameStateType.GameOver);
                return;
            }
        }
        else if (currentState is CombatState)
        {
            // Check if combat has ended
            if (IsCombatComplete())
            {
                ChangeState(GameStateType.Gameplay);
                return;
            }
            
            // Check if the player has paused the game
            if (IsPauseRequested())
            {
                ChangeState(GameStateType.Pause);
                return;
            }
            
            // Check if the player has died in combat
            if (IsPlayerDefeated())
            {
                ChangeState(GameStateType.GameOver);
                return;
            }
        }
        else if (currentState is PauseState)
        {
            // Check if the player has unpaused the game
            if (IsResumeRequested())
            {
                // Return to the previous gameplay or combat state
                if (WasInCombat())
                {
                    ChangeState(GameStateType.Combat);
                }
                else
                {
                    ChangeState(GameStateType.Gameplay);
                }
                return;
            }
            
            // Check if the player has quit to main menu
            if (IsQuitToMenuRequested())
            {
                ChangeState(GameStateType.MainMenu);
                return;
            }
        }
        else if (currentState is GameOverState)
        {
            // Check if the player wants to restart
            if (IsRestartRequested())
            {
                ChangeState(GameStateType.Loading);
                return;
            }
            
            // Check if the player wants to return to main menu
            if (IsQuitToMenuRequested())
            {
                ChangeState(GameStateType.MainMenu);
                return;
            }
        }
    }

    // Example condition methods - in a real game, these would check actual game state
    private bool IsStartGameSelected() => false; // Placeholder
    private bool IsLoadingComplete() => false; // Placeholder
    private bool HasEnteredCombat() => false; // Placeholder
    private bool IsPauseRequested() => false; // Placeholder
    private bool IsPlayerDefeated() => false; // Placeholder
    private bool IsCombatComplete() => false; // Placeholder
    private bool IsResumeRequested() => false; // Placeholder
    private bool WasInCombat() => false; // Placeholder
    private bool IsQuitToMenuRequested() => false; // Placeholder
    private bool IsRestartRequested() => false; // Placeholder
}

// Example state classes - these would be in separate files in a real project
public class MainMenuState : State
{
    public MainMenuState(StateManager stateManager) : base(stateManager) { }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entered Main Menu State");
    }
    
    public override void Exit()
    {
        Console.WriteLine("Exited Main Menu State");
        base.Exit();
    }
    
    public override void Update()
    {
        // Main menu update logic
    }
}

public class LoadingState : State
{
    public LoadingState(StateManager stateManager) : base(stateManager) { }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entered Loading State");
    }
    
    public override void Exit()
    {
        Console.WriteLine("Exited Loading State");
        base.Exit();
    }
    
    public override void Update()
    {
        // Loading update logic
    }
}

public class GameplayState : State
{
    public GameplayState(StateManager stateManager) : base(stateManager) { }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entered Gameplay State");
    }
    
    public override void Exit()
    {
        Console.WriteLine("Exited Gameplay State");
        base.Exit();
    }
    
    public override void Update()
    {
        // Gameplay update logic
    }
}

public class CombatState : State
{
    public CombatState(StateManager stateManager) : base(stateManager) { }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entered Combat State");
    }
    
    public override void Exit()
    {
        Console.WriteLine("Exited Combat State");
        base.Exit();
    }
    
    public override void Update()
    {
        // Combat update logic
    }
}

public class PauseState : State
{
    public PauseState(StateManager stateManager) : base(stateManager) { }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entered Pause State");
    }
    
    public override void Exit()
    {
        Console.WriteLine("Exited Pause State");
        base.Exit();
    }
    
    public override void Update()
    {
        // Pause update logic
    }
}

public class GameOverState : State
{
    public GameOverState(StateManager stateManager) : base(stateManager) { }
    
    public override void Enter()
    {
        base.Enter();
        Console.WriteLine("Entered Game Over State");
    }
    
    public override void Exit()
    {
        Console.WriteLine("Exited Game Over State");
        base.Exit();
    }
    
    public override void Update()
    {
        // Game over update logic
    }
} 