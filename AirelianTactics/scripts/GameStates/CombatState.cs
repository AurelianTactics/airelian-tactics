using System;
using AirelianTactics.Services;

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

    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public CombatState(StateManager stateManager) : base(stateManager)
    {
        unitService = new UnitService();
        combatTeamManager = new CombatTeamManager();
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

        // Initialize teams and units
        InitializeTeams();

        // Initialize alliances
        InitializeAlliances();

        // Initialize victory condition from game context
        InitializeVictoryCondition();

        
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
            
            // Create combat teams
            int teamId = 0;
            foreach (var teamConfig in GameContext.Teams)
            {
                CombatTeam combatTeam = new CombatTeam(teamId);
                combatTeamManager.AddTeam(combatTeam);
                
                // Create units for this team
                InitializeUnitsForTeam(teamConfig, teamId);
                
                Console.WriteLine($"Created team {teamId}");
                teamId++;
            }
        }
        else
        {
            Console.WriteLine("No teams found in Game Context");
        }
    }

    /// <summary>
    /// Initialize units for a specific team
    /// </summary>
    /// <param name="teamConfig">The team configuration</param>
    /// <param name="teamId">The team identifier</param>
    private void InitializeUnitsForTeam(TeamConfig teamConfig, int teamId)
    {
        if (teamConfig != null && teamConfig.Units != null)
        {
            foreach (var unitConfig in teamConfig.Units)
            {
                // Create player unit from unit config
                PlayerUnit playerUnit = new PlayerUnit(
                    unitConfig.InitialCT,
                    unitConfig.Speed,
                    unitConfig.PA,
                    unitConfig.HP,
                    unitConfig.Move,
                    unitConfig.Jump,
                    unitConfig.UnitId,
                    teamId
                );
                
                // Add the unit to the unit service
                unitService.AddUnit(playerUnit);
                
                Console.WriteLine($"Created unit {unitConfig.UnitId} for team {teamId}");
            }
        }
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