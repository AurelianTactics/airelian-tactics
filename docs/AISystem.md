# AI System

This document covers the artificial intelligence system in Airelian Tactics, including AI team configuration, decision-making algorithms, and integration with the game's state management and combat systems.

## Overview

The AI system enables computer-controlled teams to participate in combat alongside or against human players. AI teams make autonomous decisions for their units during combat, including movement, targeting, and action selection.

**Key Components:**
- **AIActionState**: Handles AI decision-making during unit turns
- **AI Team Configuration**: JSON-based setup for AI-controlled teams
- **Decision Algorithms**: Strategic logic for target selection and movement
- **Game Integration**: Seamless integration with turn order and combat systems

## AI Team Configuration

### Team Setup

AI teams are configured through the team configuration JSON files with a special `isAI` flag:

**AI Team Configuration Format:**
```json
{
  "teamId": "1",
  "teamName": "AI Team",
  "isAI": true,
  "units": [
    {
      "unitId": 1,
      "name": "Sam",
      "hp": 100,
      "speed": 10,
      "pa": 5,
      "move": 4,
      "jump": 2,
      "initialCT": 0
    }
  ]
}
```

**Key Configuration Elements:**
- **isAI**: Boolean flag marking the team as AI-controlled
- **Unit Stats**: Same format as human teams, affecting AI decision-making
- **Team Properties**: Standard team configuration applies to AI teams

### Game Configuration Integration

AI teams are loaded through the standard game configuration system:

```json
{
  "teams": [
    "Configs/TeamConfigDirectory/team_sample.json",     // Human team
    "Configs/TeamConfigDirectory/team_sample_2.json"   // AI team
  ]
}
```

The `isAI` flag in each team configuration determines whether the team uses AI or human control.

## AIActionState Architecture

### State Lifecycle

The AIActionState follows the standard state pattern but with automated decision-making:

**State Flow:**
1. **CombatState** detects AI unit's turn based on team's `isAI` flag
2. **Transition** to AIActionState with unit ID set
3. **Decision Making** occurs immediately in Update() method
4. **Action Creation** generates GameTimeObject for the chosen action
5. **Completion** state completes and returns to CombatState

### Key Methods

**Core Decision Method:**
```csharp
private void MakeAIDecision()
{
    // 1. Try to find and attack an enemy
    var attackTarget = FindBestAttackTarget();
    if (attackTarget.HasValue)
    {
        // Create attack action
    }
    else
    {
        // 2. Try to move toward an enemy
        var moveTarget = FindBestMoveTarget();
        if (moveTarget.HasValue)
        {
            // Create move action
        }
        else
        {
            // 3. Wait if no good options
            // Create wait action
        }
    }
}
```

**State Management:**
- `SetActorUnit(int unitId)`: Called by CombatState to specify which unit is acting
- `Enter()`: Retrieves services and validates the acting unit
- `Update()`: Executes decision-making logic and completes state
- `Exit()`: Cleanup and logging

## AI Decision-Making Algorithms

### Strategic Priority System

The AI uses a priority-based decision system:

**Priority Order:**
1. **Attack Available Enemy** - If enemies are in range, attack the weakest
2. **Move Toward Enemy** - If no attacks available, move closer to nearest enemy
3. **Wait** - If no meaningful actions are possible

### Attack Target Selection

**Algorithm:**
```csharp
private Point? FindBestAttackTarget()
{
    var enemyTargets = FindValidAttackTargets();
    
    if (enemyTargets.Count == 0)
        return null;
        
    // Strategy: Attack enemy with lowest HP
    var bestTarget = enemyTargets
        .OrderBy(target => target.unit.StatTotalHP)
        .First();
        
    return bestTarget.position;
}
```

**Target Selection Criteria:**
- **Enemy Identification**: Uses AllianceManager to identify hostile units
- **Range Validation**: Currently assumes all enemies are in range (TODO: implement range checking)
- **Priority Targeting**: Targets the enemy with the lowest current HP
- **Incapacitation Check**: Ignores already incapacitated units

### Movement Decision Logic

**Algorithm:**
```csharp
private Point? FindBestMoveTarget()
{
    // Get current position and valid move tiles
    Point currentPosition = board.GetUnitPosition(currentUnit.UnitId);
    List<Tile> validMoveTiles = board.GetValidMoveTiles(currentPosition, 3);
    
    // Find nearest enemy
    var nearestEnemy = FindNearestEnemy();
    
    // Select move tile that gets closest to nearest enemy
    var bestMoveTile = validMoveTiles
        .OrderBy(tile => CalculateDistance(tile.pos, enemyPos))
        .First();
        
    return bestMoveTile.pos;
}
```

**Movement Strategy:**
- **Nearest Enemy**: Uses distance calculation to find closest hostile unit
- **Optimal Positioning**: Selects move that minimizes distance to nearest enemy
- **Valid Moves Only**: Respects board movement rules and tile accessibility
- **Distance Calculation**: Uses Euclidean distance for pathfinding

### Action Creation

The AI creates standardized GameTimeObjects for different actions:

**Wait Action:**
```csharp
SpellName waitSpell = new SpellName();
waitSpell.AbilityName = "Wait";

GameTimeObject actionGTO = new GameTimeObject(
    phase: Phases.FasterThanFastAction,
    actorUnitId: currentUnit.UnitId,
    spellName: waitSpell
);
```

**Move Action:**
```csharp
SpellName moveSpell = new SpellName();
moveSpell.AbilityName = "Move";
moveSpell.TargetPoint = targetPosition;

GameTimeObject actionGTO = new GameTimeObject(
    phase: Phases.FasterThanFastAction,
    actorUnitId: currentUnit.UnitId,
    spellName: moveSpell
);
```

**Attack Action:**
```csharp
SpellName attackSpell = new SpellName();
attackSpell.AbilityName = "Attack";
attackSpell.BaseQ = 999;  // High damage multiplier
attackSpell.TargetPoint = targetPosition;

GameTimeObject actionGTO = new GameTimeObject(
    phase: Phases.FasterThanFastAction,
    actorUnitId: currentUnit.UnitId,
    targetUnitId: targetUnitId,
    spellName: attackSpell
);
```

## Integration with Game Systems

### State Management Integration

**CombatState Decision Logic:**
```csharp
// In CombatState.Update()
if (gameTimeObject.Phase == Phases.ActiveTurn || gameTimeObject.Phase == Phases.MidTurn)
{
    PlayerUnit currentUnit = unitService.GetUnit(unitId);
    CombatTeam team = combatTeamManager.GetTeamById(currentUnit.TeamId);
    
    if (team != null && team.IsAI)
    {
        // AI team - transition to AIActionState
        var aiActionState = stateManager.GetState<AIActionState>();
        aiActionState.SetActorUnit(unitId);
        stateManager.ChangeState<AIActionState>();
    }
    else
    {
        // Human team - transition to UnitActionState
        var unitActionState = stateManager.GetState<UnitActionState>();
        unitActionState.SetActorUnit(unitId);
        stateManager.ChangeState<UnitActionState>();
    }
}
```

### Service Dependencies

The AI system relies on shared services provided by StateManager:

**Required Services:**
- **UnitService**: Access to all units and their states
- **AllianceManager**: Team relationship queries for enemy identification
- **Board**: Movement validation and position queries
- **GameTimeManager**: Action queuing and execution
- **SpellService**: Spell creation and validation (future use)
- **StatusService**: Status effect queries (future use)

### Turn Order Integration

AI units participate in the same turn order system as human units:

**Turn Resolution:**
1. **GameTimeManager** determines next unit based on CT values
2. **CombatState** checks if unit belongs to AI team
3. **AIActionState** executes immediately without waiting for input
4. **Action Processing** happens through standard GameTimeObjectProcessor
5. **Turn Completion** follows same rules as human players

## Alliance System Integration

### Enemy Identification

The AI uses the AllianceManager to identify valid targets:

```csharp
private List<(PlayerUnit unit, Point position)> FindValidAttackTargets()
{
    var allianceManager = stateManager.AllianceManager;
    
    foreach (var unit in unitService.unitDict.Values)
    {
        if (allianceManager.AreTeamsEnemies(currentUnit.TeamId, unit.TeamId))
        {
            // Valid enemy target
            validTargets.Add((unit, position));
        }
    }
}
```

**Alliance Behaviors:**
- **Enemy Teams**: Primary targets for attacks
- **Allied Teams**: Protected and avoided
- **Neutral Teams**: Currently treated as non-targets
- **Self Team**: Never targeted

### Multi-Team Scenarios

The AI system supports complex alliance configurations:

- **Free-for-All**: Each team fights all others
- **Team Alliances**: Multiple teams cooperate against common enemies
- **Mixed Scenarios**: Human and AI teams in various alliance configurations

## Performance and Efficiency

### Instant Decision Making

AI decisions are made immediately without delay:

- **No Input Waiting**: Unlike human players, AI doesn't wait for user input
- **Single Update Cycle**: All decision-making happens in one Update() call
- **Immediate State Completion**: State completes as soon as action is queued

### Computational Complexity

Current algorithms are optimized for small-scale battles:

- **Target Search**: O(n) where n is number of units
- **Movement Search**: O(m) where m is number of valid move tiles
- **Distance Calculation**: O(1) Euclidean distance calculation

## Future Enhancements and Extensibility

### Planned Improvements

**Range-Based Combat:**
- Implement proper spell range checking
- Add range-based weapon systems
- Improve targeting based on attack ranges

**Advanced AI Strategies:**
- Defensive positioning algorithms
- Terrain advantage evaluation
- Status effect consideration
- Multi-turn planning

**Difficulty Levels:**
- Configurable AI intelligence levels
- Randomization factors for more human-like behavior
- Strategic depth adjustments

### Configuration Extensibility

**Future Configuration Options:**
```json
{
  "isAI": true,
  "aiConfig": {
    "difficulty": "normal",
    "aggressiveness": 0.7,
    "strategy": "aggressive",
    "randomization": 0.2
  }
}
```

### Algorithm Modularity

The current system is designed for easy extension:

- **Strategy Pattern**: Different AI strategies can be plugged in
- **Decision Trees**: More complex decision logic can be added
- **Behavior Trees**: Advanced AI behavior systems can be integrated

## Debugging and Development

### AI Behavior Logging

The system includes comprehensive logging for AI decisions:

```
AI Unit 1 thinking for Unit 1...
AI Unit 1 chooses to attack Unit 0 at (0, 1)
AI created and queued GameTimeObject for 1
```

### Development Mode Integration

AI behavior can be observed through CombatState's dev mode:

- **Unit Information**: Shows AI unit stats and status
- **Board State**: Displays unit positions after AI moves
- **Action Logging**: Tracks AI decisions and their outcomes

### Testing Considerations

**AI Testing Strategies:**
- **Deterministic Scenarios**: Set up specific board states to test AI logic
- **Multi-AI Games**: Test AI vs AI scenarios for balanced gameplay
- **Mixed Games**: Test AI vs human interactions
- **Edge Cases**: Test with single units, no enemies, blocked movement

## Integration Examples

### Simple AI vs Human Setup

**Game Configuration:**
```json
{
  "teams": [
    "Configs/TeamConfigDirectory/human_team.json",
    "Configs/TeamConfigDirectory/ai_team.json"
  ],
  "general": {
    "alliances": [
      {
        "0": {"1":"Enemy"},
        "1": {"0":"Enemy"}
      }
    ]
  }
}
```

### Multi-AI Tournament

**Advanced Configuration:**
```json
{
  "teams": [
    "Configs/TeamConfigDirectory/ai_team_1.json",
    "Configs/TeamConfigDirectory/ai_team_2.json",
    "Configs/TeamConfigDirectory/ai_team_3.json"
  ],
  "general": {
    "alliances": [
      {
        "0": {"1":"Enemy", "2":"Enemy"},
        "1": {"0":"Enemy", "2":"Enemy"},
        "2": {"0":"Enemy", "1":"Enemy"}
      }
    ]
  }
}
```

This AI system provides a solid foundation for computer-controlled opponents while maintaining flexibility for future enhancements and strategic depth improvements. 