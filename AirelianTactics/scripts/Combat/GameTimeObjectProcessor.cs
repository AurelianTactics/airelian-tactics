using System;
using AirelianTactics.Services;

/// <summary>
/// Processes GameTimeObjects and applies their effects by calling appropriate services.
/// This class takes GameTimeObjects from the GameTimeManager and executes their actions.
/// </summary>
public class GameTimeObjectProcessor
{
    private readonly UnitService unitService;
    private readonly SpellService spellService;
    private readonly StatusService statusService;
    private readonly Board board;

    /// <summary>
    /// Constructor that takes the required services for processing game time objects
    /// </summary>
    /// <param name="unitService">Service for managing units</param>
    /// <param name="spellService">Service for managing spells</param>
    /// <param name="statusService">Service for managing status effects</param>
    /// <param name="board">Board for managing unit positions</param>
    public GameTimeObjectProcessor(UnitService unitService, SpellService spellService, 
        StatusService statusService, Board board)
    {
        this.unitService = unitService;
        this.spellService = spellService;
        this.statusService = statusService;
        this.board = board;
    }

    /// <summary>
    /// Process a GameTimeObject by examining its properties and applying the appropriate effects
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject to process</param>
    /// <returns>True if the object was processed successfully, false otherwise</returns>
    public bool ProcessGameTimeObject(GameTimeObject gameTimeObject)
    {
        if (gameTimeObject == null)
        {
            Console.WriteLine("Warning: Attempted to process null GameTimeObject");
            return false;
        }

        try
        {
            // Get the actor unit
            PlayerUnit actorUnit = GetActorUnit(gameTimeObject);
            if (actorUnit == null)
            {
                Console.WriteLine($"Warning: Could not find actor unit with ID {gameTimeObject.ActorUnitId}");
                return false;
            }

            // Process based on the spell's ability name
            string abilityName = gameTimeObject.SpellName?.AbilityName?.ToLower() ?? "";
            
            switch (abilityName)
            {
                case "wait":
                    return ProcessWaitAction(gameTimeObject, actorUnit);
                case "move":
                    return ProcessMoveAction(gameTimeObject, actorUnit);
                case "attack":
                    return ProcessAttackAction(gameTimeObject, actorUnit);
                default:
                    Console.WriteLine($"Warning: Unknown ability '{abilityName}' for GameTimeObject {gameTimeObject.GameTimeObjectId}");
                    return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing GameTimeObject {gameTimeObject.GameTimeObjectId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Process a Wait action - ends the unit's turn
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject containing wait action details</param>
    /// <param name="actorUnit">The unit performing the wait action</param>
    /// <returns>True if processed successfully</returns>
    private bool ProcessWaitAction(GameTimeObject gameTimeObject, PlayerUnit actorUnit)
    {
        Console.WriteLine($"Processing Wait action for Unit {actorUnit.UnitId}");
        
        try
        {
            // End the unit's turn, which will:
            // - Set IsMidActiveTurn to false
            // - Subtract CT
            unitService.EndUnitTurn(actorUnit);
            
            Console.WriteLine($"Unit {actorUnit.UnitId} has completed their wait action");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing wait action: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Process a Move action - updates the unit's position on the board
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject containing move action details</param>
    /// <param name="actorUnit">The unit performing the move action</param>
    /// <returns>True if processed successfully</returns>
    private bool ProcessMoveAction(GameTimeObject gameTimeObject, PlayerUnit actorUnit)
    {
        Console.WriteLine($"Processing Move action for Unit {actorUnit.UnitId}");
        
        try
        {
            // Get the target position from the spell
            Point? targetPosition = gameTimeObject.SpellName?.TargetPoint;
            if (!targetPosition.HasValue)
            {
                Console.WriteLine("Error: Move action has no target position specified");
                return false;
            }

            // Get current position
            Point? currentPosition = board.GetUnitPosition(actorUnit.UnitId);
            if (!currentPosition.HasValue)
            {
                Console.WriteLine($"Error: Could not find current position for Unit {actorUnit.UnitId}");
                return false;
            }

            // Move the unit on the board
            bool moveSuccessful = board.MoveUnit(actorUnit.UnitId, currentPosition.Value, targetPosition.Value);
            
            if (moveSuccessful)
            {
                Console.WriteLine($"Unit {actorUnit.UnitId} moved from ({currentPosition.Value.x},{currentPosition.Value.y}) to ({targetPosition.Value.x},{targetPosition.Value.y})");
                
                // End the unit's turn after successful move
                unitService.EndUnitTurn(actorUnit);
                return true;
            }
            else
            {
                Console.WriteLine($"Error: Failed to move Unit {actorUnit.UnitId} to target position");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing move action: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Process an Attack action - performs combat calculation and applies damage
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject containing attack action details</param>
    /// <param name="actorUnit">The unit performing the attack action</param>
    /// <returns>True if processed successfully</returns>
    private bool ProcessAttackAction(GameTimeObject gameTimeObject, PlayerUnit actorUnit)
    {
        Console.WriteLine($"Processing Attack action for Unit {actorUnit.UnitId}");
        
        try
        {
            // Get target unit
            PlayerUnit targetUnit = GetTargetUnit(gameTimeObject);
            if (targetUnit == null)
            {
                Console.WriteLine($"Error: Could not find target unit for attack");
                return false;
            }

            // Perform basic damage calculation
            // For now, using a simple calculation based on attacker's PA
            // TODO: Implement more complex damage formula based on spell properties
            int damage = CalculateBasicDamage(actorUnit, targetUnit, gameTimeObject.SpellName);
            
            Console.WriteLine($"Unit {actorUnit.UnitId} attacks Unit {targetUnit.UnitId} for {damage} damage");
            
            // Apply damage to target
            unitService.AlterHP(targetUnit, -damage);
            
            Console.WriteLine($"Unit {targetUnit.UnitId} HP reduced by {damage} (HP: {targetUnit.StatTotalHP})");
            
            // Check if target is incapacitated
            if (targetUnit.IsIncapacitated)
            {
                Console.WriteLine($"Unit {targetUnit.UnitId} has been incapacitated!");
            }
            
            // End the attacker's turn
            unitService.EndUnitTurn(actorUnit);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing attack action: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Calculate basic damage for an attack
    /// TODO: Implement more sophisticated damage calculation based on spell properties
    /// </summary>
    /// <param name="attacker">The attacking unit</param>
    /// <param name="target">The target unit</param>
    /// <param name="spell">The spell being used</param>
    /// <returns>Damage amount</returns>
    private int CalculateBasicDamage(PlayerUnit attacker, PlayerUnit target, SpellName spell)
    {
        // Simple damage calculation for now
        // Base damage from attacker's PA, with some randomization
        Random random = new Random();
        int baseDamage = attacker.StatTotalPA;
        
        // Add some variance (±25%)
        int variance = (int)(baseDamage * 0.25f);
        int finalDamage = baseDamage + random.Next(-variance, variance + 1);
        
        // Ensure minimum damage of 1
        return Math.Max(1, finalDamage);
    }

    /// <summary>
    /// Get the actor unit from the GameTimeObject
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject</param>
    /// <returns>The actor PlayerUnit, or null if not found</returns>
    private PlayerUnit GetActorUnit(GameTimeObject gameTimeObject)
    {
        if (!gameTimeObject.ActorUnitId.HasValue)
        {
            return null;
        }

        return unitService.unitDict.TryGetValue(gameTimeObject.ActorUnitId.Value, out PlayerUnit unit) ? unit : null;
    }

    /// <summary>
    /// Get the target unit from the GameTimeObject
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject</param>
    /// <returns>The target PlayerUnit, or null if not found</returns>
    private PlayerUnit GetTargetUnit(GameTimeObject gameTimeObject)
    {
        if (!gameTimeObject.TargetUnitId.HasValue)
        {
            return null;
        }

        return unitService.unitDict.TryGetValue(gameTimeObject.TargetUnitId.Value, out PlayerUnit unit) ? unit : null;
    }

    /// <summary>
    /// Get a summary of what action will be performed by a GameTimeObject
    /// Useful for logging or UI display
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject to summarize</param>
    /// <returns>A string description of the action</returns>
    public string GetActionSummary(GameTimeObject gameTimeObject)
    {
        if (gameTimeObject == null)
        {
            return "Invalid GameTimeObject";
        }

        string abilityName = gameTimeObject.SpellName?.AbilityName ?? "Unknown";
        string actorId = gameTimeObject.ActorUnitId?.ToString() ?? "Unknown";
        
        switch (abilityName.ToLower())
        {
            case "wait":
                return $"Unit {actorId} will wait and end their turn";
            case "move":
                Point? targetPos = gameTimeObject.SpellName?.TargetPoint;
                if (targetPos.HasValue)
                {
                    return $"Unit {actorId} will move to ({targetPos.Value.x},{targetPos.Value.y})";
                }
                return $"Unit {actorId} will move (position not specified)";
            case "attack":
                string targetId = gameTimeObject.TargetUnitId?.ToString() ?? "Unknown";
                return $"Unit {actorId} will attack Unit {targetId}";
            default:
                return $"Unit {actorId} will perform {abilityName}";
        }
    }
} 