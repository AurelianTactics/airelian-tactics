# Game Time and Turn Order System

This document covers the game time management and turn order systems in Airelian Tactics, including the GameTimeManager, phase-based action processing, charge time (CT) mechanics, and turn resolution algorithms.

## Overview

The game time system manages when and how actions are executed during combat. It uses a phase-based approach where different types of actions are processed in specific orders, combined with a charge time (CT) system that determines when units are eligible for turns.

**Key Components:**
- **GameTimeManager**: Central coordinator for action timing and phase processing
- **GameTimeObject**: Data structure representing scheduled actions
- **GameTimeObjectProcessor**: Executes actions and applies their effects
- **Phase System**: Priority-based action execution order
- **CT System**: Determines unit turn eligibility based on charge time accumulation

## GameTimeManager Architecture

### Core Responsibilities

The GameTimeManager serves as the central hub for all timing-related operations:

**Primary Functions:**
- **Phase Queue Management**: Maintains separate queues for each action phase
- **Turn Order Resolution**: Determines which unit acts next based on CT values
- **Action Scheduling**: Queues actions for future execution
- **Action Processing**: Coordinates action execution through the processor

### Phase-Based Queue System

The system uses multiple queues organized by action phases:

**Phase Priority Order:**
```csharp
private List<Phases> phaseOrder = new List<Phases>
{
    Phases.FasterThanFastAction,  // Highest priority
    Phases.Reaction,
    Phases.Mime,
    Phases.MidTurn,
    Phases.QuickTurn,
    Phases.ActiveTurn,
    Phases.EndOfActiveTurn,
    Phases.SlowAction               // Lowest priority
};
```

**Queue Management:**
- Each phase has its own `List<GameTimeObject>` queue
- Actions are processed in phase priority order
- Within each phase, actions are processed first-in-first-out (FIFO)

### Turn Order Logic

**Active Turn Detection:**
```csharp
private GameTimeObject HandleActiveTurnPhase()
{
    var activeTurnUnit = unitService.GetNextActiveTurnPlayerUnit();
    if (activeTurnUnit != null && !activeTurnUnit.IsMidActiveTurn)
    {
        return new GameTimeObject(phase: Phases.ActiveTurn, actorUnitId: activeTurnUnit.UnitId);
    }
    return null;
}
```

**Key Behaviors:**
- **CT-Based Eligibility**: Units must have CT ≥ 100 to be eligible for turns
- **Mid-Turn Priority**: Units already in their turn (MidTurn phase) take precedence
- **Tiebreaker Resolution**: Lower unit IDs act first when CT values are equal

## GameTimeObject Structure

### Core Properties

GameTimeObjects represent scheduled actions with comprehensive metadata:

```csharp
public class GameTimeObject
{
    public int GameTimeObjectId { get; private set; }     // Unique identifier
    public int ResolveTick { get; set; }                  // When to resolve (-1 = immediate)
    public Phases? Phase { get; set; }                    // Execution phase
    public int? ActorUnitId { get; set; }                 // Unit performing action
    public int? TargetUnitId { get; set; }                // Target unit (if applicable)
    public SpellName SpellName { get; set; }              // Action details
}
```

### Action Types

**Wait Action:**
```csharp
SpellName waitSpell = new SpellName();
waitSpell.AbilityName = "Wait";

GameTimeObject gto = new GameTimeObject(
    phase: Phases.FasterThanFastAction,
    actorUnitId: unitId,
    spellName: waitSpell
);
```

**Move Action:**
```csharp
SpellName moveSpell = new SpellName();
moveSpell.AbilityName = "Move";
moveSpell.TargetPoint = destinationPosition;

GameTimeObject gto = new GameTimeObject(
    phase: Phases.FasterThanFastAction,
    actorUnitId: unitId,
    spellName: moveSpell
);
```

**Attack Action:**
```csharp
SpellName attackSpell = new SpellName();
attackSpell.AbilityName = "Attack";
attackSpell.BaseQ = 999;
attackSpell.TargetPoint = targetPosition;

GameTimeObject gto = new GameTimeObject(
    phase: Phases.FasterThanFastAction,
    actorUnitId: unitId,
    targetUnitId: targetId,
    spellName: attackSpell
);
```

### Lifecycle Management

**Creation and Queuing:**
1. **Action Decision**: Player or AI chooses an action
2. **GTO Creation**: GameTimeObject created with appropriate properties
3. **Queue Addition**: Added to appropriate phase queue via `AddGameTimeObject()`
4. **Processing**: Retrieved by `GetNextGameTimeObject()` when ready

## Charge Time (CT) System

### CT Mechanics

The CT system determines when units can take their turns:

**Core Principles:**
- **CT Accumulation**: Units gain CT each world tick based on their Speed stat
- **Turn Eligibility**: Units with CT ≥ 100 can take active turns
- **CT Consumption**: Taking actions reduces CT
- **Speed Impact**: Higher Speed units accumulate CT faster

### CT Calculation

**Per-Tick CT Gain:**
```csharp
public void AddCT()
{
    if (this.StatTotalCT < 100)
    {
        this.StatTotalCT += this.StatTotalSpeed;
        // TODO: Add status effect modifiers
    }
}
```

**Turn Completion:**
```csharp
public void EndTurn()
{
    this.IsMidActiveTurn = false;
    this.StatTotalCT -= 100;
    if (this.StatTotalCT < 0)
    {
        this.StatTotalCT = 0;
    }
}
```

### Turn Order Priority

When multiple units reach CT ≥ 100 simultaneously:

**Priority Resolution:**
1. **CT Value**: Higher CT goes first
2. **Unit ID Tiebreaker**: Lower Unit ID goes first if CT is equal
3. **Mid-Turn Status**: Units in MidTurn phase take precedence

## Phase System Details

### Phase Types and Purposes

**FasterThanFastAction:**
- Highest priority actions
- Most player actions (Move, Attack, Wait) use this phase
- Processed before any other actions

**Reaction:**
- Reactive abilities triggered by other actions
- Currently placeholder for future implementation
- Would interrupt normal action flow

**Mime:**
- Copy actions from other units
- Placeholder for advanced ability system
- Queue-based processing when implemented

**MidTurn:**
- Units that have started but not completed their turn
- Takes precedence over new ActiveTurn units
- Special handling in GameTimeManager

**QuickTurn:**
- Fast actions that can interrupt normal turns
- Future implementation for speed-based abilities
- Would process before ActiveTurn

**ActiveTurn:**
- Standard unit turns when CT ≥ 100
- Handled specially by checking UnitService directly
- Triggers state transitions to UnitActionState/AIActionState

**EndOfActiveTurn:**
- Cleanup actions after a unit's turn
- Status effect processing
- Turn-end triggers

**SlowAction:**
- Lowest priority actions
- Delayed effects and background processing
- Processed last in each cycle

### Phase Processing Logic

**Standard Phase Processing:**
```csharp
private GameTimeObject HandleRegularPhase(Phases phase)
{
    var queue = phaseQueues[phase];
    
    if (queue.Count > 0)
    {
        var firstObject = queue[0];
        
        if (MeetsCriteria(firstObject))
        {
            queue.RemoveAt(0);  // Remove from queue
            return firstObject;
        }
    }
    
    return null;
}
```

**Special Turn Phase Processing:**
- **MidTurn**: Checks UnitService for units with `IsMidActiveTurn = true`
- **ActiveTurn**: Checks UnitService for units with CT ≥ 100 and not mid-turn
- **Direct Processing**: Doesn't use queues, queries unit state directly

## GameTimeObjectProcessor

### Action Resolution

The processor executes GameTimeObjects and applies their effects:

**Processing Pipeline:**
1. **Validation**: Verify actor unit exists and is valid
2. **Action Routing**: Route to appropriate handler based on ability name
3. **Effect Application**: Apply the action's effects to game state
4. **Turn Management**: Handle turn completion and CT reduction

### Action Handlers

**Wait Action Processing:**
```csharp
private bool ProcessWaitAction(GameTimeObject gameTimeObject, PlayerUnit actorUnit)
{
    Console.WriteLine($"Processing Wait action for Unit {actorUnit.UnitId}");
    
    // End the unit's turn
    unitService.EndUnitTurn(actorUnit);
    
    return true;
}
```

**Move Action Processing:**
```csharp
private bool ProcessMoveAction(GameTimeObject gameTimeObject, PlayerUnit actorUnit)
{
    Point? targetPosition = gameTimeObject.SpellName?.TargetPoint;
    Point? currentPosition = board.GetUnitPosition(actorUnit.UnitId);
    
    // Validate and execute move
    bool moveSuccessful = board.MoveUnit(actorUnit.UnitId, currentPosition.Value, targetPosition.Value);
    
    if (moveSuccessful)
    {
        unitService.EndUnitTurn(actorUnit);
        return true;
    }
    
    return false;
}
```

**Attack Action Processing:**
```csharp
private bool ProcessAttackAction(GameTimeObject gameTimeObject, PlayerUnit actorUnit)
{
    PlayerUnit targetUnit = GetTargetUnit(gameTimeObject);
    
    // Calculate damage
    int damage = CalculateBasicDamage(actorUnit, targetUnit, gameTimeObject.SpellName);
    
    // Apply damage
    unitService.AlterHP(targetUnit, -damage);
    
    // End attacker's turn
    unitService.EndUnitTurn(actorUnit);
    
    return true;
}
```

### Damage Calculation

**Basic Damage Formula:**
```csharp
private int CalculateBasicDamage(PlayerUnit attacker, PlayerUnit target, SpellName spell)
{
    // Simple damage: Attacker PA * Spell multiplier
    int baseDamage = attacker.StatTotalPA * spell.BaseQ;
    
    return Math.Max(0, baseDamage);
}
```

## World Tick System

### Tick Processing

The world tick system advances game time and updates unit states:

**Tick Operations:**
1. **Increment World Tick**: Advance global time counter
2. **Status Processing**: Update status effects (future implementation)
3. **CT Increment**: Add CT to all eligible units
4. **State Updates**: Refresh unit and game states

**Implementation:**
```csharp
private void RunWorldTick(VictoryCondition victoryCondition, CombatTeamManager combatTeamManager, UnitService unitService)
{
    // Increment world tick counter
    stateManager.IncrementWorldTick();
    
    // TODO: Process status effects
    
    // Update unit CT for all units
    unitService.IncrementCTAll();
}
```

### CT Increment Process

**All-Units CT Update:**
```csharp
public void IncrementCTAll()
{
    foreach (var unit in unitDict.Values)
    {
        if (!unit.IsIncapacitated)
        {
            unit.AddCT();
        }
    }
}
```

**Benefits:**
- **Simultaneous Updates**: All units advance CT together
- **Incapacitation Handling**: Dead units don't gain CT
- **Speed Advantage**: Faster units gain CT more quickly

## Integration with Combat System

### Combat Loop Integration

The game time system integrates seamlessly with the combat loop:

**CombatState Processing:**
```csharp
while (true)
{
    // Check victory condition
    if (IsVictoryConditionMet()) break;
    
    // Get next scheduled action
    GameTimeObject gameTimeObject = gameTimeManager.GetNextGameTimeObject();
    
    if (gameTimeObject != null)
    {
        if (gameTimeObject.Phase == Phases.ActiveTurn || gameTimeObject.Phase == Phases.MidTurn)
        {
            // Handle turn - transition to UnitActionState or AIActionState
            HandleUnitTurn(gameTimeObject);
            return; // Exit to let action state take over
        }
        else
        {
            // Process action immediately
            gameTimeManager.ProcessGameTimeObject(gameTimeObject);
        }
    }
    else
    {
        // No actions queued - run world tick
        RunWorldTick();
    }
}
```

### State Transition Coordination

**Turn Handling:**
1. **Turn Detection**: GameTimeManager identifies unit turn
2. **State Transition**: CombatState transitions to appropriate action state
3. **Action Creation**: Action state creates and queues GameTimeObject
4. **Return to Combat**: Action state completes, returns to CombatState
5. **Action Processing**: CombatState processes the queued action

## Performance Considerations

### Queue Management Efficiency

**Optimization Strategies:**
- **Phase Separation**: Separate queues prevent unnecessary searches
- **FIFO Processing**: Simple list operations for queue management
- **Minimal Sorting**: Unit order determined by UnitService, not GameTimeManager

### Memory Management

**Object Lifecycle:**
- **Creation**: GameTimeObjects created only when needed
- **Processing**: Objects removed from queues after processing
- **Cleanup**: No persistent storage of processed objects

### Computational Complexity

**Time Complexities:**
- **GetNextGameTimeObject()**: O(p) where p is number of phases (constant)
- **CT Updates**: O(n) where n is number of units
- **Queue Operations**: O(1) for enqueue, O(1) for dequeue

## Future Enhancements

### Planned Improvements

**Advanced Timing:**
- **Delayed Actions**: Support for actions that resolve on specific ticks
- **Conditional Processing**: Actions that wait for specific conditions
- **Priority Modifiers**: Dynamic phase priority based on game state

**Status Effect Integration:**
- **CT Modification**: Status effects that alter CT gain/loss
- **Action Interruption**: Status effects that prevent or modify actions
- **Turn-Based Effects**: Effects that trigger on specific phases

**Performance Optimization:**
- **Priority Queues**: More efficient queue management for large action counts
- **Batch Processing**: Group similar actions for efficiency
- **Predictive Scheduling**: Pre-calculate turn order for multiple ticks

### Configuration Extensions

**Future Configuration Options:**
```json
{
  "timing": {
    "tickRate": 100,
    "ctRequired": 100,
    "defaultActionPhase": "FasterThanFastAction"
  },
  "phases": {
    "enableReactions": true,
    "enableMime": true,
    "enableQuick": true
  }
}
```

## Debugging and Development

### Time System Logging

The system provides comprehensive logging for timing operations:

```
Processing Move action for Unit 1
Unit 1 moved from (0,0) to (1,0)
Unit 1 has completed their turn
World Tick: 5
Unit CT updates: Unit 0 CT: 85, Unit 1 CT: 0
```

### Development Tools

**Debugging Features:**
- **World Tick Counter**: Track global time progression
- **CT Monitoring**: Observe CT accumulation for all units
- **Phase Queue Inspection**: Examine queued actions by phase
- **Action History**: Log of all processed actions

### Testing Scenarios

**Common Test Cases:**
- **Single Unit**: Verify CT accumulation and turn taking
- **Multiple Units**: Test turn order with different speeds
- **Simultaneous Turns**: Handle units reaching CT 100 together
- **Action Queuing**: Verify proper phase-based processing
- **State Transitions**: Test integration with action states

This game time system provides precise control over action timing and turn order, ensuring fair and predictable gameplay while maintaining flexibility for complex combat scenarios. 