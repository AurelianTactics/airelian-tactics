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
        Console.WriteLine("--- Entering Map Setup State");
        Console.WriteLine("Setting up the game map...");
        
        // Verify that we have a game configuration
        if (GameContext.GameConfig != null)
        {
            // Display game configuration info
            GameConfig config = GameContext.GameConfig;
            Console.WriteLine($"Game victory condition: {config.General.VictoryCondition}");
            Console.WriteLine($"Map to load: {config.Map.MapFile}");
            
            // Display information about alliances
            if (config.General.Alliances != null && config.General.Alliances.Count > 0)
            {
                Console.WriteLine($"Number of alliances: {config.General.Alliances.Count}");
                for (int i = 0; i < config.General.Alliances.Count; i++)
                {
                    var alliance = config.General.Alliances[i];
                    Console.WriteLine($"Alliance {i+1}: {alliance.Count} entries");
                    // TODO: Fix alliance structure access - currently Dictionary<string, Dictionary<string, string>>
                }
            }
        }
        
        // Verify that we have teams
        if (GameContext.Teams != null && GameContext.Teams.Count > 0)
        {
            Console.WriteLine($"Number of teams loaded: {GameContext.Teams.Count}");
            
            // Display information about each team
            for (int i = 0; i < GameContext.Teams.Count; i++)
            {
                var team = GameContext.Teams[i];
                Console.WriteLine($"Team {i+1}: {team.TeamName} (ID: {team.TeamId}) with {team.Units.Count} units");
                
                // Display information about each unit
                foreach (var unit in team.Units)
                {
                    Console.WriteLine($"  - Unit {unit.UnitId}: {unit.Name}, HP: {unit.HP}, Speed: {unit.Speed}");
                }
            }
        }
        else
        {
            Console.WriteLine("Warning: No teams found in GameContext");
        }
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
        Console.WriteLine("--- Exiting Map Setup State");
        base.Exit();
    }
} 