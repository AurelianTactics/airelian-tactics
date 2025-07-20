using System;
using System.IO;
using System.Collections.Generic;
using System.Linq; // Added for Max() and OrderBy()
using AirelianTactics.Services; // Added for UnitService

/// <summary>
/// Game initialization state.
/// Handles the initial setup of the game.
/// </summary>
public class GameInitState : State
{
    // Path to the game configuration file
    private const string GAME_CONFIG_PATH = "Configs/GameConfigDirectory/game_config.json";

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
            
            // Load the map configuration
            LoadMapConfiguration();
            Console.WriteLine("Map configuration loaded successfully.");
            
            // Initialize game services with loaded data
            InitializeBoard();
            InitializeUnits();
            PlaceUnitsOnBoard();
            
            // Initialize combat objects
            InitializeCombatObjects();
            
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
    /// Loads the map configuration from the file specified in the game config.
    /// </summary>
    protected virtual void LoadMapConfiguration()
    {
        // Check if we have a game config loaded
        if (GameContext.GameConfig == null || GameContext.GameConfig.Map == null)
        {
            throw new InvalidOperationException("Game configuration must be loaded before loading map.");
        }

        string mapFilePath = GameContext.GameConfig.Map.MapFile;
        
        // Check if the map file exists
        if (string.IsNullOrEmpty(mapFilePath))
        {
            Console.WriteLine("Warning: No map file specified in game config.");
            return;
        }
        
        if (!File.Exists(mapFilePath))
        {
            string workingDir = Environment.CurrentDirectory;
            Console.WriteLine($"Warning: Map configuration file not found at path: {mapFilePath}. " +
                             $"Current working directory: {workingDir}");
            return;
        }
        
        Console.WriteLine($"Loading map configuration from: {mapFilePath}");
        
        try
        {
            // Use the MapConfigLoader to load the map configuration
            MapConfig mapConfig = MapConfigLoader.LoadMapConfig(mapFilePath);
            
            // Update the map configuration in the GameContext
            GameContext.GameConfig.Map = mapConfig;
            
            Console.WriteLine($"Loaded map: {mapConfig.MapFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading map configuration from {mapFilePath}: {ex.Message}");
        }
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

    /// <summary>
    /// Initialize the board using the map configuration from game context
    /// </summary>
    private void InitializeBoard()
    {
        var board = stateManager.Board;
        
        if (GameContext != null && GameContext.GameConfig != null && GameContext.GameConfig.Map != null)
        {
            board.Load(GameContext.GameConfig.Map);
            Console.WriteLine("Board initialized from map configuration");
        }
        else
        {
            Console.WriteLine("No map configuration found in Game Context - board will be empty");
        }
    }

    /// <summary>
    /// Initialize all units from all teams using snake draft for fair unit ID assignment
    /// </summary>
    private void InitializeUnits()
    {
        var unitService = stateManager.UnitService;
        
        if (GameContext == null || GameContext.Teams == null)
        {
            Console.WriteLine("No teams found in Game Context");
            return;
        }

        Console.WriteLine($"Initializing units for {GameContext.Teams.Count} teams");

        // First, collect all unit configs with their team information
        var allUnitData = new List<(UnitConfig unitConfig, int teamId)>();
        
        for (int teamId = 0; teamId < GameContext.Teams.Count; teamId++)
        {
            var teamConfig = GameContext.Teams[teamId];
            if (teamConfig != null && teamConfig.Units != null)
            {
                foreach (var unitConfig in teamConfig.Units)
                {
                    allUnitData.Add((unitConfig, teamId));
                }
            }
        }

        if (allUnitData.Count == 0)
        {
            Console.WriteLine("No units found to initialize");
            return;
        }

        // Group units by team to handle snake draft
        var unitsByTeam = new List<List<(UnitConfig unitConfig, int teamId)>>();
        for (int teamId = 0; teamId < GameContext.Teams.Count; teamId++)
        {
            unitsByTeam.Add(new List<(UnitConfig unitConfig, int teamId)>());
        }

        foreach (var (unitConfig, teamId) in allUnitData)
        {
            unitsByTeam[teamId].Add((unitConfig, teamId));
        }

        // Find the maximum number of units any team has
        int maxUnitsPerTeam = unitsByTeam.Max(teamUnits => teamUnits.Count);

        // Assign unit IDs using snake draft
        int currentUnitId = 0;
        
        for (int round = 0; round < maxUnitsPerTeam; round++)
        {
            bool isEvenRound = (round % 2) == 0;
            
            if (isEvenRound)
            {
                // Forward order: Team 0, 1, 2, ...
                for (int teamId = 0; teamId < GameContext.Teams.Count; teamId++)
                {
                    if (round < unitsByTeam[teamId].Count)
                    {
                        var (unitConfig, actualTeamId) = unitsByTeam[teamId][round];
                        CreateAndAddPlayerUnit(unitConfig, actualTeamId, currentUnitId, unitService);
                        currentUnitId++;
                    }
                }
            }
            else
            {
                // Reverse order: Team ..., 2, 1, 0
                for (int teamId = GameContext.Teams.Count - 1; teamId >= 0; teamId--)
                {
                    if (round < unitsByTeam[teamId].Count)
                    {
                        var (unitConfig, actualTeamId) = unitsByTeam[teamId][round];
                        CreateAndAddPlayerUnit(unitConfig, actualTeamId, currentUnitId, unitService);
                        currentUnitId++;
                    }
                }
            }
        }

        Console.WriteLine($"Snake draft completed. Assigned {currentUnitId} unit IDs across {GameContext.Teams.Count} teams");
    }

    /// <summary>
    /// Create a PlayerUnit and add it to the unit service with the specified unit ID
    /// </summary>
    private void CreateAndAddPlayerUnit(UnitConfig unitConfig, int teamId, int assignedUnitId, UnitService unitService)
    {
        // Create player unit from unit config with the assigned unit ID
        PlayerUnit playerUnit = new PlayerUnit(
            unitConfig.InitialCT,
            unitConfig.Speed,
            unitConfig.PA,
            unitConfig.HP,
            unitConfig.Move,
            unitConfig.Jump,
            assignedUnitId,  // Use the snake draft assigned ID
            teamId
        );
        
        // Add the unit to the unit service with the specific ID
        unitService.AddUnitWithId(playerUnit, assignedUnitId);
        
        Console.WriteLine($"Created unit with snake draft ID {assignedUnitId} (original ID: {unitConfig.UnitId}) for team {teamId}");
    }

    /// <summary>
    /// Place units from unitService onto the board tiles.
    /// </summary>
    private void PlaceUnitsOnBoard()
    {
        var unitService = stateManager.UnitService;
        var board = stateManager.Board;
        
        if (unitService == null || unitService.unitDict == null || unitService.unitDict.Count == 0)
        {
            Console.WriteLine("No units found to place on board");
            return;
        }

        if (board == null || board.tiles == null || board.tiles.Count == 0)
        {
            Console.WriteLine("No board tiles available for unit placement");
            return;
        }

        // Get units sorted by UnitId (lowest first)
        var sortedUnits = unitService.unitDict.Values
            .OrderBy(unit => unit.UnitId)
            .ToList();

        // Get tiles in dictionary iteration order
        var availableTiles = board.tiles.Values.ToList();

        int tileIndex = 0;
        int unitsPlaced = 0;

        foreach (var unit in sortedUnits)
        {
            if (tileIndex >= availableTiles.Count)
            {
                Console.WriteLine($"Warning: Not enough tiles to place all units. Placed {unitsPlaced}/{sortedUnits.Count} units");
                break;
            }

            var tile = availableTiles[tileIndex];
            bool placed = board.PlaceUnitOnTile(tile, unit.UnitId);
            
            if (placed)
            {
                Console.WriteLine($"Placed unit {unit.UnitId} (Team {unit.TeamId}) at position {tile.pos}");
                unitsPlaced++;
            }
            else
            {
                Console.WriteLine($"Failed to place unit {unit.UnitId} at position {tile.pos} - tile may be occupied");
            }
            
            tileIndex++;
        }

        Console.WriteLine($"Unit placement complete: {unitsPlaced} units placed on board");
    }

    /// <summary>
    /// Initialize combat-specific objects that depend on game configuration
    /// </summary>
    private void InitializeCombatObjects()
    {
        // Initialize combat objects using StateManager methods
        stateManager.InitializeAllianceManager();
        stateManager.InitializeVictoryCondition();
        stateManager.InitializeCombatTeams();
        
        Console.WriteLine("Combat objects initialization complete!");
    }
} 