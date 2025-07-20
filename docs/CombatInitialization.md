# Game Initialization

This document covers the complete game initialization process handled by `GameInitState`, which sets up all game components including configurations, teams, units, board, and combat objects before transitioning to combat.

## Overview

When `GameInitState` is entered, it performs a comprehensive initialization sequence to prepare the entire game environment:

1. **Configuration Loading** - Loads game, team, and map configurations from JSON files
2. **Service Initialization** - Sets up shared services through StateManager
3. **Board Setup** - Creates the game map from configuration data
4. **Unit Creation** - Instantiates all player units using snake draft for fair ID assignment
5. **Unit Placement** - Places units on the board in starting positions
6. **Combat Object Setup** - Initializes alliances, victory conditions, and team management

All initialization is completed before any combat begins, ensuring a clean separation between setup and gameplay.

## Configuration Loading System

### Game Configuration Loading

The initialization process begins by loading the master game configuration file:

**File Path:** `AirelianTactics/Configs/GameConfigDirectory/game_config.json`

**Configuration Structure:**
```json
{
  "general": {
    "victoryCondition": "LastTeamStanding",
    "alliances": [
      {
        "0": {"1":"Enemy"},
        "1": {"0":"Enemy"}
      }
    ]
  },
  "teams": [
    "Configs/TeamConfigDirectory/team_sample.json",
    "Configs/TeamConfigDirectory/team_sample_2.json"
  ],
  "map": {
    "mapFile": "Configs/MapConfigDirectory/default_map.json"
  }
}
```

**Key Components:**
- **Victory Condition**: Defines win condition for the game session
- **Alliances**: Specifies relationships between teams (Enemy, Allied, Neutral, etc.)
- **Teams**: Array of file paths to individual team configuration files
- **Map**: Reference to the map configuration file

### Team Configuration Loading

The system loads multiple team configurations specified in the game config:

**Team Configuration Format:**
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

**Key Properties:**
- **teamId**: Unique identifier for the team
- **teamName**: Display name for the team
- **isAI**: Boolean flag indicating if team is AI-controlled
- **units**: Array of unit configurations with stats and properties

### Map Configuration Loading

Map data is loaded from the file specified in the game configuration, setting up the battlefield layout and tile properties.

## Services Architecture Integration

### Shared Services Pattern

GameInitState initializes and configures shared services that persist throughout the game session:

**Services Managed by StateManager:**
- **UnitService**: Manages all player units and their states
- **SpellService**: Handles spell and ability processing
- **StatusService**: Manages status effects on units
- **Board**: Manages map tiles and unit positions
- **GameTimeManager**: Controls turn order and action timing

**Benefits:**
- Services persist across state transitions
- Consistent data access patterns
- Centralized service management
- Clean separation of concerns

## Unit Creation and Snake Draft System

### Snake Draft Algorithm

The system uses a snake draft algorithm to fairly distribute unit IDs across teams, preventing any team from having inherent advantages based on turn order priority.

**Algorithm Process:**

1. **Collection Phase**: Gather all unit configurations from all teams
2. **Grouping Phase**: Organize units by team ID
3. **Assignment Phase**: Use alternating order per round:
   - **Even rounds** (0, 2, 4...): Team 0 → Team 1 → Team 2 → ...
   - **Odd rounds** (1, 3, 5...): Team N → Team N-1 → ... → Team 0

**Example Snake Draft:**
For 2 teams with 2 units each:

| Round | Order | Team 0 Unit | Team 1 Unit | Assigned IDs |
|-------|-------|-------------|-------------|--------------|
| 0 (even) | Forward | Unit A | Unit C | A=0, C=1 |
| 1 (odd) | Reverse | Unit B | Unit D | D=2, B=3 |

**Result:** Team 0 gets IDs 0,3 and Team 1 gets IDs 1,2

### PlayerUnit Creation

Units are created with the following process:

```csharp
PlayerUnit playerUnit = new PlayerUnit(
    unitConfig.InitialCT,     // Starting charge time
    unitConfig.Speed,         // Movement speed stat
    unitConfig.PA,            // Physical attack power
    unitConfig.HP,            // Hit points
    unitConfig.Move,          // Movement range
    unitConfig.Jump,          // Jump height
    assignedUnitId,           // Snake draft assigned ID
    teamId                    // Team identifier
);
```

**Key Properties Set:**
- Combat stats from configuration (HP, Speed, PA, Move, Jump, Initial CT)
- Snake draft assigned ID (used for tiebreakers and turn order)
- Team assignment
- Initial state flags (IsIncapacitated = false, IsMidActiveTurn = false)

## Board Initialization and Unit Placement

### Board Setup

The board is initialized using the map configuration loaded from JSON:

1. **Map Loading**: Board loads tile data from MapConfig
2. **Tile Creation**: Creates Tile objects with properties and positions
3. **Accessibility Setup**: Configures movement and placement permissions

### Unit Placement Process

Units are placed on the board in a systematic order:

1. **Unit Sorting**: Units sorted by UnitId (lowest first)
2. **Tile Selection**: Available tiles selected in iteration order
3. **Sequential Placement**: Each unit placed on next available tile
4. **Validation**: Placement success verified for each unit

**Placement Logic:**
```csharp
// Sort units by ID for consistent placement order
var sortedUnits = unitService.unitDict.Values.OrderBy(unit => unit.UnitId);

// Place each unit on next available tile
foreach (var unit in sortedUnits)
{
    bool placed = board.PlaceUnitOnTile(tile, unit.UnitId);
    // Handle placement success/failure
}
```

## Combat Object Initialization

### Alliance System Setup

The AllianceManager is initialized with configurations from the game config:

**Implementation:**
```csharp
stateManager.InitializeAllianceManager();
```

**Functionality:**
- Parses alliance configurations from JSON
- Sets up team relationship mappings
- Provides alliance queries for combat logic

### Victory Condition Setup

Victory conditions are configured based on game settings:

**Supported Victory Types:**
- **LastTeamStanding**: Game continues until only one team remains

**Implementation:**
```csharp
stateManager.InitializeVictoryCondition();
```

### Combat Team Management

CombatTeam objects are created for team-level management:

**Team Properties:**
- **TeamId**: Unique team identifier
- **IsDefeated**: Current defeat status
- **IsAI**: AI control flag from configuration

**Implementation:**
```csharp
stateManager.InitializeCombatTeams();
```

## Integration with Game Flow

### State Transition

Once initialization is complete, GameInitState transitions to CombatState:

1. **Completion Check**: All initialization steps completed successfully
2. **State Marking**: `CompleteState()` called to mark initialization done
3. **Automatic Transition**: StateManager transitions to CombatState
4. **Service Handoff**: All initialized services available to CombatState

### Data Persistence

**GameContext Storage:**
- Game configuration data
- Team configurations
- Map configuration
- All data accessible across states

**StateManager Services:**
- Unit dictionary with all created units
- Initialized board with placed units
- Combat management objects
- Shared services ready for combat

## Error Handling and Validation

### Configuration Validation

The system validates configurations during loading:

- **File Existence**: Checks if configuration files exist
- **JSON Parsing**: Validates JSON structure and content
- **Data Integrity**: Ensures required fields are present
- **Graceful Degradation**: Continues loading other teams if one fails

### Initialization Verification

Each initialization step includes validation:

- **Service Availability**: Confirms services are properly initialized
- **Data Consistency**: Validates cross-references between configurations
- **Placement Success**: Verifies unit placement on board
- **Combat Readiness**: Ensures all combat objects are properly set up

## Configuration Files Reference

### File Locations

- **Game Config**: `AirelianTactics/Configs/GameConfigDirectory/game_config.json`
- **Team Configs**: `AirelianTactics/Configs/TeamConfigDirectory/`
  - `team_sample.json` - Human team
  - `team_sample_2.json` - AI team
- **Map Config**: `AirelianTactics/Configs/MapConfigDirectory/default_map.json`

### Required Configuration Elements

**Game Configuration Must Include:**
- Victory condition specification
- Team file references
- Map file reference
- Alliance definitions (optional, defaults to neutral)

**Team Configuration Must Include:**
- Team identification and naming
- AI control flag
- Unit definitions with complete stats

**Map Configuration Must Include:**
- Tile definitions with positions
- Movement and placement permissions
- Terrain type specifications

## Best Practices

1. **Configuration Management**: Keep configuration files in designated directories
2. **Error Recovery**: Handle missing files gracefully with meaningful error messages
3. **Data Validation**: Validate all loaded data before proceeding to combat
4. **Service Lifecycle**: Initialize services once in GameInitState, reuse across states
5. **Snake Draft**: Use fair ID assignment to prevent gameplay imbalances
6. **Clean Separation**: Complete all initialization before starting combat logic

This initialization system provides a robust foundation for the game's combat system, ensuring all components are properly configured and ready before gameplay begins. 