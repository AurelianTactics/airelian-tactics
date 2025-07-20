using System;
using System.Collections.Generic;
using System.Linq;
using AirelianTactics.Services;

/// <summary>
/// Handles AI decision-making during unit turns for AI-controlled teams.
/// Triggered when CombatState returns a GameTimeObject for an AI unit's turn.
/// </summary>
public class AIActionState : State
{
    private int actorUnitId;
    private PlayerUnit currentUnit;
    private UnitService unitService;
    private SpellService spellService;
    private StatusService statusService;
    private Board board;
    private GameTimeManager gameTimeManager;

    /// <summary>
    /// Constructor that takes a state manager.
    /// </summary>
    /// <param name="stateManager">The state manager that will manage this state.</param>
    public AIActionState(StateManager stateManager) : base(stateManager)
    {
        // Services will be initialized from GameContext in Enter()
    }

    /// <summary>
    /// Set the actor unit ID for this turn. Called by CombatState before transitioning.
    /// </summary>
    /// <param name="unitId">The ID of the unit taking their turn</param>
    public void SetActorUnit(int unitId)
    {
        actorUnitId = unitId;
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        
        // Get shared services from StateManager
        unitService = stateManager.UnitService;
        spellService = stateManager.SpellService;
        statusService = stateManager.StatusService;
        board = stateManager.Board;
        gameTimeManager = stateManager.GameTimeManager;
        
        Console.WriteLine("Entering AI Action State");
        
        // Get the current unit using the actorUnitId
        if (unitService.unitDict.TryGetValue(actorUnitId, out currentUnit) == false)
        {
            currentUnit = null;
        }
        
        if (currentUnit == null)
        {
            Console.WriteLine($"AI Unit {actorUnitId} not found");
            CompleteState();
            return;
        }

        Console.WriteLine($"AI Unit {currentUnit.UnitId} (Team {currentUnit.TeamId}) is taking their turn");
    }

    /// <summary>
    /// Called to update the state.
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // AI makes decision immediately - no waiting for input
        MakeAIDecision();
        CompleteState();
    }

    /// <summary>
    /// Core AI decision-making logic
    /// </summary>
    private void MakeAIDecision()
    {
        Console.WriteLine($"AI thinking for Unit {currentUnit.UnitId}...");
        
        // Simple AI strategy: 
        // 1. Try to attack an enemy if one is in range
        // 2. Otherwise, move toward the nearest enemy
        // 3. If no enemies or can't move, just wait
        
        GameTimeObject actionGTO = null;
        
        // First, try to find and attack an enemy
        var attackTarget = FindBestAttackTarget();
        if (attackTarget.HasValue)
        {
            Console.WriteLine($"AI Unit {currentUnit.UnitId} chooses to attack Unit {board.GetUnitAtPosition(attackTarget.Value)} at ({attackTarget.Value.x}, {attackTarget.Value.y})");
            actionGTO = new GameTimeObject(
                phase: Phases.FasterThanFastAction,
                actorUnitId: currentUnit.UnitId,
                targetUnitId: board.GetUnitAtPosition(attackTarget.Value),
                spellName: CreateAttackSpell(attackTarget.Value)
            );
        }
        else
        {
            // No attack available, try to move toward an enemy
            var moveTarget = FindBestMoveTarget();
            if (moveTarget.HasValue)
            {
                Console.WriteLine($"AI Unit {currentUnit.UnitId} chooses to move to ({moveTarget.Value.x}, {moveTarget.Value.y})");
                actionGTO = new GameTimeObject(
                    phase: Phases.FasterThanFastAction,
                    actorUnitId: currentUnit.UnitId,
                    targetUnitId: null,
                    spellName: CreateMoveSpell(moveTarget.Value)
                );
            }
            else
            {
                // Can't attack or move meaningfully, just wait
                Console.WriteLine($"AI Unit {currentUnit.UnitId} chooses to wait");
                actionGTO = new GameTimeObject(
                    phase: Phases.FasterThanFastAction,
                    actorUnitId: currentUnit.UnitId,
                    spellName: CreateWaitSpell()
                );
            }
        }
        
        if (actionGTO != null)
        {
            gameTimeManager.AddGameTimeObject(actionGTO);
            Console.WriteLine($"AI created and queued GameTimeObject for {currentUnit.UnitId}");
        }
    }

    /// <summary>
    /// Find the best enemy to attack if any are in range
    /// </summary>
    /// <returns>Position of best attack target, or null if none available</returns>
    private Point? FindBestAttackTarget()
    {
        var enemyTargets = FindValidAttackTargets();
        
        if (enemyTargets.Count == 0)
            return null;
            
        // Simple strategy: attack the enemy with lowest HP
        var bestTarget = enemyTargets
            .OrderBy(target => target.unit.StatTotalHP)
            .First();
            
        return bestTarget.position;
    }

    /// <summary>
    /// Find the best position to move toward (usually toward nearest enemy)
    /// </summary>
    /// <returns>Best move position, or null if no good moves available</returns>
    private Point? FindBestMoveTarget()
    {
        // Get current position
        Point? currentPositionNullable = board.GetUnitPosition(currentUnit.UnitId);
        if (!currentPositionNullable.HasValue)
            return null;
            
        Point currentPosition = currentPositionNullable.Value;
        
        // Get valid move tiles
        List<Tile> validMoveTiles = board.GetValidMoveTiles(currentPosition, 3); // Using default move range
        
        if (validMoveTiles.Count == 0)
            return null;
            
        // Find nearest enemy
        var nearestEnemy = FindNearestEnemy();
        if (!nearestEnemy.HasValue)
            return null; // No enemies to move toward
            
        // Find the move tile that gets us closest to the nearest enemy
        Point enemyPos = nearestEnemy.Value;
        var bestMoveTile = validMoveTiles
            .OrderBy(tile => CalculateDistance(tile.pos, enemyPos))
            .First();
            
        return bestMoveTile.pos;
    }

    /// <summary>
    /// Find the position of the nearest enemy unit
    /// </summary>
    /// <returns>Position of nearest enemy, or null if none found</returns>
    private Point? FindNearestEnemy()
    {
        Point? currentPosition = board.GetUnitPosition(currentUnit.UnitId);
        if (!currentPosition.HasValue)
            return null;
            
        var allianceManager = stateManager.AllianceManager;
        if (allianceManager == null)
            return null;
            
        Point? nearestEnemyPos = null;
        double nearestDistance = double.MaxValue;
        
        foreach (var kvp in unitService.unitDict)
        {
            PlayerUnit unit = kvp.Value;
            
            // Skip self and incapacitated units
            if (unit.UnitId == currentUnit.UnitId || unit.IsIncapacitated)
                continue;
                
            // Check if this unit is an enemy
            if (allianceManager.AreTeamsEnemies(currentUnit.TeamId, unit.TeamId))
            {
                Point? unitPos = board.GetUnitPosition(unit.UnitId);
                if (unitPos.HasValue)
                {
                    double distance = CalculateDistance(currentPosition.Value, unitPos.Value);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemyPos = unitPos.Value;
                    }
                }
            }
        }
        
        return nearestEnemyPos;
    }

    /// <summary>
    /// Find valid enemy units that can be attacked
    /// </summary>
    /// <returns>List of enemy units with their positions</returns>
    private List<(PlayerUnit unit, Point position)> FindValidAttackTargets()
    {
        List<(PlayerUnit unit, Point position)> validTargets = new List<(PlayerUnit unit, Point position)>();
        
        var allianceManager = stateManager.AllianceManager;
        if (allianceManager == null)
            return validTargets;
        
        foreach (var kvp in unitService.unitDict)
        {
            PlayerUnit unit = kvp.Value;
            
            // Skip self and incapacitated units
            if (unit.UnitId == currentUnit.UnitId || unit.IsIncapacitated)
                continue;
                
            // Check if this unit is an enemy
            if (allianceManager.AreTeamsEnemies(currentUnit.TeamId, unit.TeamId))
            {
                Point? position = board.GetUnitPosition(unit.UnitId);
                if (position.HasValue)
                {
                    // TODO: Add proper range checking when spell range is implemented
                    // For now, assume all enemies are in range
                    validTargets.Add((unit, position.Value));
                }
            }
        }
        
        return validTargets;
    }

    /// <summary>
    /// Calculate distance between two points
    /// </summary>
    private double CalculateDistance(Point a, Point b)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Create a spell for the Wait command
    /// </summary>
    private SpellName CreateWaitSpell()
    {
        SpellName waitSpell = new SpellName();
        waitSpell.AbilityName = "Wait";
        return waitSpell;
    }

    /// <summary>
    /// Create a spell for the Move command
    /// </summary>
    private SpellName CreateMoveSpell(Point targetPosition)
    {
        SpellName moveSpell = new SpellName();
        moveSpell.AbilityName = "Move";
        moveSpell.TargetPoint = targetPosition;
        return moveSpell;
    }

    /// <summary>
    /// Create a spell for the Attack command
    /// </summary>
    private SpellName CreateAttackSpell(Point targetPosition)
    {
        SpellName attackSpell = new SpellName();
        attackSpell.AbilityName = "Attack";
        attackSpell.BaseQ = 999;
        attackSpell.TargetPoint = targetPosition;
        return attackSpell;
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public override void Exit()
    {
        Console.WriteLine("Exiting AI Action State");
        base.Exit();
    }
} 