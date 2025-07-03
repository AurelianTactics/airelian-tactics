using System;
using AirelianTactics.Services;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Combat state.
/// Handles the combat phase of the game.
/// </summary>
public class CombatState : State
{
    private bool hasCompletedCombat = false;
    private VictoryCondition victoryCondition;
    private CombatTeamManager combatTeamManager;
    private UnitService unitService;
    private AllianceManager allianceManager;
    private Board board;

    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public CombatState(StateManager stateManager) : base(stateManager)
    {
        unitService = new UnitService();
        combatTeamManager = new CombatTeamManager();
        board = new Board();
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

        // Initialize the board from map configuration
        InitializeBoard();

        // Initialize teams and units
        InitializeTeams();

        // Initialize alliances
        InitializeAlliances();

        // Initialize victory condition from game context
        InitializeVictoryCondition();

        // Place units on the board
        PlaceUnitsOnBoard();
    }

    /// <summary>
    /// Initialize the board using the map configuration from game context
    /// </summary>
    private void InitializeBoard()
    {
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
    /// Place units from unitService onto the board tiles.
    /// Temporary Units are placed deterministically starting from lowest UnitId,
    /// into the first available tiles in dictionary iteration order.
    /// </summary>
    private void PlaceUnitsOnBoard()
    {
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
    /// Initialize the victory condition using the game context
    /// </summary>
    private void InitializeVictoryCondition()
    {
        if (GameContext != null && GameContext.GameConfig != null && GameContext.GameConfig.General != null)
        {
            string victoryConditionString = GameContext.GameConfig.General.VictoryCondition;
            victoryCondition = new VictoryCondition(victoryConditionString);
            Console.WriteLine($"Victory condition set to: {victoryConditionString}");
        }
        else
        {
            // Default victory condition
            victoryCondition = new VictoryCondition();
            Console.WriteLine("Using default victory condition: Last Team Standing");
        }
    }

    /// <summary>
    /// Initialize the alliances using the game context
    /// </summary>
    private void InitializeAlliances()
    {
        if (GameContext != null && GameContext.GameConfig != null && 
            GameContext.GameConfig.General != null && GameContext.GameConfig.General.Alliances != null)
        {
            // Create alliance manager with the alliance configurations from the game context
            allianceManager = new AllianceManager(GameContext.GameConfig.General.Alliances);
            
            // Log the alliances that have been set up
            Console.WriteLine($"Initialized alliance manager with alliance configurations from game config");
            
            // For testing purposes, check and log some alliance relationships
            if (combatTeamManager.GetTeamCount() >= 2)
            {
                bool areTeam0And1Allied = allianceManager.AreTeamsAllied(0, 1);
                bool areTeam0And1Enemies = allianceManager.AreTeamsEnemies(0, 1);
                Console.WriteLine($"Teams 0 and 1 are allied: {areTeam0And1Allied}");
                Console.WriteLine($"Teams 0 and 1 are enemies: {areTeam0And1Enemies}");
            }
        }
        else
        {
            // Create an empty alliance manager with default relationships
            allianceManager = new AllianceManager();
            Console.WriteLine("Initialized empty alliance manager with default neutral relationships");
        }
    }

    /// <summary>
    /// Initialize the combat teams using the game context
    /// </summary>
    private void InitializeTeams()
    {
        if (GameContext != null && GameContext.Teams != null)
        {
            Console.WriteLine($"Initializing {GameContext.Teams.Count} teams for combat");
            
            // Create combat teams first
            int teamId = 0;
            foreach (var teamConfig in GameContext.Teams)
            {
                CombatTeam combatTeam = new CombatTeam(teamId);
                combatTeamManager.AddTeam(combatTeam);
                Console.WriteLine($"Created team {teamId}");
                teamId++;
            }

            // Initialize all units using snake draft for fair unit ID assignment
            InitializeAllUnitsWithSnakeDraft();
        }
        else
        {
            Console.WriteLine("No teams found in Game Context");
        }
    }

    /// <summary>
    /// Initialize all units from all teams using snake draft for fair unit ID assignment
    /// Lower unit IDs are better and are distributed fairly across teams
    /// Snake draft order: Team0, Team1, Team2... then Team2, Team1, Team0... and so on
    /// </summary>
    private void InitializeAllUnitsWithSnakeDraft()
    {
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
                        CreateAndAddPlayerUnit(unitConfig, actualTeamId, currentUnitId);
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
                        CreateAndAddPlayerUnit(unitConfig, actualTeamId, currentUnitId);
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
    /// <param name="unitConfig">The unit configuration</param>
    /// <param name="teamId">The team identifier</param>
    /// <param name="assignedUnitId">The unit ID assigned by snake draft</param>
    private void CreateAndAddPlayerUnit(UnitConfig unitConfig, int teamId, int assignedUnitId)
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
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        if (!hasCompletedCombat)
        {
            Console.WriteLine("Combat in progress...");

            // Check if victory condition has been met
            if (victoryCondition.IsVictoryConditionMet(combatTeamManager, unitService))
            {
                Console.WriteLine("Victory condition has been met! Combat is now complete.");
                hasCompletedCombat = true;
                CompleteState();
            }
            else
            {
                // Continue combat
                Console.WriteLine("Combat continues...");
                // Process combat turn logic here
                
                // For testing purposes, we'll complete combat after one update
                Console.WriteLine("Combat complete for testing purposes!");
                hasCompletedCombat = true;
                CompleteState();
            }
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