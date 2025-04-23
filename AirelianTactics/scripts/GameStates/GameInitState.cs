using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Game initialization state.
/// Handles the initial setup of the game.
/// </summary>
public class GameInitState : State
{
    // Path to the game configuration file
    private const string GAME_CONFIG_PATH = "AirelianTactics/Configs/GameConfigDirectory/game_config.json";

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
        Console.WriteLine("--- Entering Game Initialization State");
        Console.WriteLine("Loading game resources...");
    }

    /// <summary>
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        try
        {
            // Load the game configuration from JSON
            LoadGameConfiguration();
            Console.WriteLine("Game configuration loaded successfully.");
            
            // Load all team configurations from the game config
            LoadAllTeamConfigurations();
            Console.WriteLine("Team configurations loaded successfully.");
            
            Console.WriteLine("Game initialization complete!");
            // Mark this state as completed
            CompleteState();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during game initialization: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("--- Exiting Game Initialization State");
        base.Exit();
    }
    
    /// <summary>
    /// Loads the game configuration from JSON and stores it in the GameContext.
    /// </summary>
    protected virtual void LoadGameConfiguration()
    {
        // Check if the file exists
        if (!File.Exists(GAME_CONFIG_PATH))
        {
            string workingDir = Environment.CurrentDirectory;
            throw new FileNotFoundException(
                $"Game configuration file not found at path: {GAME_CONFIG_PATH}. " +
                $"Current working directory: {workingDir}");
        }
        
        Console.WriteLine($"Loading game configuration from: {GAME_CONFIG_PATH}");
        
        // Use the GameConfigLoader to load the game configuration
        GameConfig gameConfig = GameConfigLoader.LoadGameConfig(GAME_CONFIG_PATH);
        
        // Store the game configuration in the GameContext
        GameContext.GameConfig = gameConfig;
        
        Console.WriteLine($"Loaded game configuration with victory condition: {gameConfig.General.VictoryCondition}");
        Console.WriteLine($"Teams to load: {gameConfig.Teams.Count}");
    }
    
    /// <summary>
    /// Loads all team configurations specified in the game config.
    /// </summary>
    protected virtual void LoadAllTeamConfigurations()
    {
        // Clear any previously loaded teams
        GameContext.Teams.Clear();
        
        // Check if we have a game config loaded
        if (GameContext.GameConfig == null || GameContext.GameConfig.Teams == null)
        {
            throw new InvalidOperationException("Game configuration must be loaded before loading teams.");
        }
        
        foreach (string teamFilePath in GameContext.GameConfig.Teams)
        {
            // Check if the team file exists
            if (!File.Exists(teamFilePath))
            {
                string workingDir = Environment.CurrentDirectory;
                Console.WriteLine($"Warning: Team configuration file not found at path: {teamFilePath}. " +
                                 $"Current working directory: {workingDir}");
                continue;
            }
            
            Console.WriteLine($"Loading team configuration from: {teamFilePath}");
            
            try
            {
                // Use the TeamConfigLoader to load the team configuration
                TeamConfig teamConfig = TeamConfigLoader.LoadTeamConfig(teamFilePath);
                
                // Add the team configuration to the GameContext
                GameContext.Teams.Add(teamConfig);
                
                Console.WriteLine($"Loaded team: {teamConfig.TeamName} with {teamConfig.Units.Count} units");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading team configuration from {teamFilePath}: {ex.Message}");
                // Continue loading other teams even if one fails
            }
        }
        
        Console.WriteLine($"Loaded {GameContext.Teams.Count} teams in total.");
    }
    
    /// <summary>
    /// Legacy method that loads a single team configuration.
    /// For backward compatibility.
    /// </summary>
    protected virtual void LoadTeamConfiguration()
    {
        // This now uses the first team from the game config
        if (GameContext.Teams.Count > 0)
        {
            // The first team is already loaded and accessible via TeamConfig property
            Console.WriteLine($"Using first team from config: {GameContext.TeamConfig.TeamName}");
        }
        else
        {
            Console.WriteLine("No teams were loaded from the game configuration.");
        }
    }
} 