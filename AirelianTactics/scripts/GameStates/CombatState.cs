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
    private SpellService spellService;
    private StatusService statusService;
    private AllianceManager allianceManager;
    private Board board;
    private GameTimeManager gameTimeManager;

    
    /// <summary>
    /// Dev mode flag - when true, prints unit info and map layout on each update
    /// </summary>
    public bool IsDevMode { get; set; } = true; // Default to true for development

    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public CombatState(StateManager stateManager) : base(stateManager)
    {
        // Combat objects will be retrieved from StateManager in Enter()
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        
        // Get shared services from StateManager (already initialized by GameInitState)
        unitService = stateManager.UnitService;
        spellService = stateManager.SpellService;
        statusService = stateManager.StatusService;
        board = stateManager.Board;
        gameTimeManager = stateManager.GameTimeManager;
        
        // Get shared combat objects from StateManager (already initialized by GameInitState)
        combatTeamManager = stateManager.CombatTeamManager;
        allianceManager = stateManager.AllianceManager;
        victoryCondition = stateManager.VictoryCondition;
        
        hasCompletedCombat = false;
        Console.WriteLine("Entering Combat State");
    }

    

    /// <summary>
    /// Called to update the state. In prototype called by program.cs
    /// If GameTimeObject, then process it. if not then proceed to world tick
    /// </summary>
    public override void Update()
    {
        base.Update();
        /*
           backlog: add statuses tied to unit turn
        */
            
        while( true){
            
            hasCompletedCombat = IsVictoryConditionMet(victoryCondition, combatTeamManager, unitService);

            if (hasCompletedCombat){
                break;
            }
            else{
                GameTimeObject gameTimeObject = gameTimeManager.GetNextGameTimeObject();

                if (gameTimeObject != null)
                {
                    PrintDevView();

                    // Handle different types of GameTimeObjects
                    if (gameTimeObject.Phase == Phases.ActiveTurn || gameTimeObject.Phase == Phases.MidTurn)
                    {
                        // Turn trigger - check if this is an AI team or human team
                        int unitId = gameTimeObject.ActorUnitId.Value;
                        
                        // Get the unit to determine its team
                        PlayerUnit currentUnit = null;
                        if (unitService.unitDict.TryGetValue(unitId, out currentUnit))
                        {
                            // Get the team to check if it's AI-controlled
                            CombatTeam team = combatTeamManager.GetTeamById(currentUnit.TeamId);
                            
                            if (team != null && team.IsAI)
                            {
                                // AI team - transition to AIActionState
                                Console.WriteLine($"AI Unit {unitId} (Team {currentUnit.TeamId}) turn - transitioning to AIActionState");
                                
                                var aiActionState = stateManager.GetState<AIActionState>();
                                aiActionState.SetActorUnit(unitId);
                                
                                stateManager.ChangeState<AIActionState>();
                                return; // Exit the game loop to let AIActionState take over
                            }
                            else
                            {
                                // Human team - transition to UnitActionState for player input
                                Console.WriteLine($"Human Unit {unitId} (Team {currentUnit.TeamId}) turn - transitioning to UnitActionState");
                                
                                var unitActionState = stateManager.GetState<UnitActionState>();
                                unitActionState.SetActorUnit(unitId);
                                
                                stateManager.ChangeState<UnitActionState>();
                                return; // Exit the game loop to let UnitActionState take over
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Unit {unitId} not found in unitService");
                        }
                    }
                    else
                    {
                        // Action effect - process through the processor
                        bool processed = gameTimeManager.ProcessGameTimeObject(gameTimeObject);
                        if (!processed)
                        {
                            Console.WriteLine("Failed to process action GameTimeObject");
                        }

                        // Print debug information if dev mode is enabled
                        PrintDevView();
                    }
                }
                else
                {
                    // //Console.WriteLine("No game time object found");
                    // break;
                    RunWorldTick(victoryCondition, combatTeamManager, unitService);
                }
            }
        }
        
        // If we exit the loop, combat has completed - transition to GameEndState
        if (hasCompletedCombat)
        {
            Console.WriteLine("Victory condition met! Transitioning to Game End State...");
            PrintDevView();
            stateManager.ChangeState<GameEndState>();
        }
    }

    /// <summary>
    /// Run world time related logic
    /// increment world tick, potentially remove statuses, increment unit CT
    /// </summary>
    /// <param name="victoryCondition">The victory condition</param>
    /// <param name="combatTeamManager">The combat team manager</param>
    /// <param name="unitService">The unit service</param>
    private void RunWorldTick(VictoryCondition victoryCondition, CombatTeamManager combatTeamManager, UnitService unitService)
    {
        // increment world tick
        stateManager.IncrementWorldTick();

        // to do: potentially remove statuses

        // update unit CT
        unitService.IncrementCTAll();

        PrintDevView();

    }


    private bool IsVictoryConditionMet(VictoryCondition victoryCondition, CombatTeamManager combatTeamManager, UnitService unitService)
    {
        return victoryCondition.IsVictoryConditionMet(combatTeamManager, unitService);
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("Exiting Combat State");
        base.Exit();
    }

    public void PrintDevView()
    {
        if (IsDevMode)
        {
            Console.WriteLine("Combat in progress. Tick: " + stateManager.WorldTick);
            //Console.WriteLine(); // Add spacing
            unitService.PrintUnitInfo();
            Console.WriteLine(); // Add spacing
                board.PrintMap();
                Console.WriteLine(); // Add spacing
        }
    }
} 