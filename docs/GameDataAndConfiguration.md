
## Game Data and Configuration

This document covers the comprehensive configuration system in Airelian Tactics, including JSON file structures, data loading mechanisms, and configuration management through the GameContext system.

## Overview

The game uses a hierarchical JSON configuration system that separates concerns across multiple files:

- **Game Configuration**: Master configuration defining teams, maps, victory conditions, and alliances
- **Team Configurations**: Individual team setups with units, AI flags, and team properties
- **Map Configuration**: Battlefield layout, tile properties, and terrain definitions

All configurations are loaded during GameInitState and stored in the GameContext for access throughout the game session.

## Game Configuration Structure

### Master Configuration File

**Location:** `AirelianTactics/Configs/GameConfigDirectory/game_config.json`

**Complete Structure:**
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

### General Section

**Victory Condition Configuration:**
- **Type**: String value defining win condition
- **Current Options**: `"LastTeamStanding"` (game continues until one team remains)
- **Future Extensions**: Additional victory types can be added as needed

**Alliance Configuration:**
```json
"alliances": [
  {
    "0": {"1":"Enemy", "2":"Allied"},
    "1": {"0":"Enemy", "2":"Neutral"},
    "2": {"0":"Allied", "1":"Neutral"}
  }
]
```

**Alliance Types:**
- **Enemy**: Teams fight each other
- **Allied**: Teams cooperate and protect each other
- **Neutral**: Teams ignore each other (no targeting)
- **Hero**: Special alliance for player-controlled units
- **Self**: Automatic alliance with same team (implicit)

### Team References

**Team File Array:**
```json
"teams": [
  "path/to/team1.json",
  "path/to/team2.json",
  "path/to/teamN.json"
]
```

**Properties:**
- **Relative Paths**: All paths relative to project root
- **Load Order**: Teams loaded sequentially, assigned team IDs 0, 1, 2, etc.
- **Error Handling**: Missing files logged as warnings but don't stop loading

### Map Reference

**Map Configuration:**
```json
"map": {
  "mapFile": "Configs/MapConfigDirectory/default_map.json"
}
```

**Properties:**
- **Single Map**: One map per game session
- **Required Field**: Map file must be specified and exist
- **Path Format**: Relative path from project root

## Team Configuration Structure

### Complete Team Format

**Location:** `AirelianTactics/Configs/TeamConfigDirectory/`

**Example Files:**
- `team_sample.json` - Human-controlled team
- `team_sample_2.json` - AI-controlled team

**Complete Structure:**
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
    },
    {
      "unitId": 2,
      "name": "Stan",
      "hp": 80,
      "speed": 12,
      "pa": 4,
      "move": 3,
      "jump": 1,
      "initialCT": 0
    }
  ]
}
```

### Team Properties

**Team Identification:**
- **teamId**: String identifier (converted to int during loading)
- **teamName**: Display name for the team
- **isAI**: Boolean flag determining AI vs human control

**Unit Array:**
- **units**: Array of unit configurations
- **Required**: At least one unit per team
- **Snake Draft**: Unit IDs reassigned during initialization for fairness

### Unit Properties

**Core Statistics:**
- **hp**: Hit points (health/damage capacity)
- **speed**: CT accumulation rate (higher = more frequent turns)
- **pa**: Physical attack power (damage multiplier)
- **move**: Movement range in tiles
- **jump**: Vertical movement capability
- **initialCT**: Starting charge time (usually 0)

**Identification:**
- **unitId**: Original unit identifier (overridden by snake draft)
- **name**: Display name for the unit

**Data Types:**
- All numeric values are integers
- Boolean values for flags
- String values for names and identifiers

## Map Configuration Structure

### Map File Format

**Location:** `AirelianTactics/Configs/MapConfigDirectory/`

**Structure Overview:**
```json
{
  "general": {
    "mapName": "Default Map"
  },
  "tiles": [
    {
      "tileId": 0,
      "x": 0,
      "y": 0,
      "z": 0,
      "standable": true,
      "traversable": true,
      "terrain": "grass",
      "canPlayerStart": true
    }
  ]
}
```

**Tile Properties:**
- **Position**: x, y, z coordinates defining tile location
- **Movement**: standable and traversable flags for unit interaction
- **Terrain**: String identifier for terrain type effects
- **Placement**: canPlayerStart flag for initial unit positioning

## GameContext System

### Architecture Overview

The GameContext system provides centralized data management across all game states:

**Core Components:**
- **GameContext Class**: Central data container
- **StateManager Integration**: Automatic context sharing
- **Service Coordination**: Links configuration data with game services

### GameContext Properties

**Configuration Storage:**
```csharp
public class GameContext
{
    public GameConfig GameConfig { get; set; }        // Master game configuration
    public List<TeamConfig> Teams { get; set; }       // All loaded team configurations
    public TeamConfig TeamConfig { get; }             // Backward compatibility property
    public MapConfig Map => GameConfig?.Map;          // Map configuration accessor
}
```

**Access Patterns:**
- **Direct Access**: `GameContext.GameConfig.General.VictoryCondition`
- **Team Access**: `GameContext.Teams[0]` or `GameContext.TeamConfig`
- **Map Access**: `GameContext.Map` or `GameContext.GameConfig.Map`

### State Integration

**Automatic Context Sharing:**
```csharp
// In StateManager.RegisterState()
state.GameContext = gameContext;

// In State classes
protected GameContext GameContext { get; set; }
```

**Benefits:**
- **Consistent Access**: All states have identical context access
- **Automatic Updates**: Context changes propagated to all states
- **Type Safety**: Strongly-typed configuration objects

## Configuration Loading Process

### GameInitState Loading Sequence

**Loading Order:**
1. **Game Configuration**: Load master config file first
2. **Team Configurations**: Load all team files specified in game config
3. **Map Configuration**: Load map file specified in game config
4. **Validation**: Verify all required data loaded successfully

### Error Handling

**File Validation:**
```csharp
if (!File.Exists(configPath))
{
    string workingDir = Environment.CurrentDirectory;
    throw new FileNotFoundException(
        $"Configuration file not found at path: {configPath}. " +
        $"Current working directory: {workingDir}");
}
```

**Graceful Degradation:**
- **Missing Teams**: Continue loading other teams, log warnings
- **Invalid JSON**: Stop loading with detailed error message
- **Missing Map**: Continue with empty board, log warning

### Validation Rules

**Required Elements:**
- Game configuration file must exist
- At least one team file must load successfully
- Team files must contain valid unit arrays
- Map file should exist (warning if missing)

**Optional Elements:**
- Alliance configurations (defaults to neutral)
- Map configurations (defaults to empty board)
- Extended team properties (AI flags, etc.)

## Data Flow and Usage

### Initialization to Combat Flow

**Data Loading (GameInitState):**
1. Load and parse JSON configurations
2. Store in GameContext for state sharing
3. Initialize services with configuration data
4. Create game objects from configuration

**Service Integration:**
- **UnitService**: Creates PlayerUnits from team configurations
- **Board**: Loads map structure from map configuration
- **AllianceManager**: Parses alliance rules from game configuration
- **CombatTeamManager**: Creates CombatTeam objects from team data

### Runtime Data Access

**Configuration Queries:**
```csharp
// In any state
string victoryCondition = GameContext.GameConfig.General.VictoryCondition;
List<TeamConfig> allTeams = GameContext.Teams;
MapConfig currentMap = GameContext.Map;
```

**Service Queries:**
```csharp
// Through StateManager
var unitService = stateManager.UnitService;
var board = stateManager.Board;
var allianceManager = stateManager.AllianceManager;
```

## Configuration Examples

### Simple 1v1 Setup

**Game Configuration:**
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
    "Configs/TeamConfigDirectory/human_team.json",
    "Configs/TeamConfigDirectory/ai_team.json"
  ],
  "map": {
    "mapFile": "Configs/MapConfigDirectory/arena_map.json"
  }
}
```

### Multi-Team Alliance

**Complex Alliance Configuration:**
```json
{
  "general": {
    "alliances": [
      {
        "0": {"1":"Allied", "2":"Enemy", "3":"Enemy"},
        "1": {"0":"Allied", "2":"Enemy", "3":"Enemy"},
        "2": {"0":"Enemy", "1":"Enemy", "3":"Allied"},
        "3": {"0":"Enemy", "1":"Enemy", "2":"Allied"}
      }
    ]
  },
  "teams": [
    "Configs/TeamConfigDirectory/team_alpha.json",
    "Configs/TeamConfigDirectory/team_beta.json",
    "Configs/TeamConfigDirectory/team_gamma.json",
    "Configs/TeamConfigDirectory/team_delta.json"
  ]
}
```

### AI Tournament Setup

**All-AI Configuration:**
```json
{
  "teams": [
    "Configs/TeamConfigDirectory/ai_aggressive.json",
    "Configs/TeamConfigDirectory/ai_defensive.json",
    "Configs/TeamConfigDirectory/ai_balanced.json"
  ],
  "general": {
    "victoryCondition": "LastTeamStanding",
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

## Configuration Best Practices

### File Organization

**Directory Structure:**
```
Configs/
├── GameConfigDirectory/
│   └── game_config.json
├── TeamConfigDirectory/
│   ├── team_sample.json
│   ├── team_sample_2.json
│   └── ai_teams/
│       ├── aggressive_ai.json
│       └── defensive_ai.json
└── MapConfigDirectory/
    ├── default_map.json
    └── arena_map.json
```

**Naming Conventions:**
- Use descriptive file names
- Separate AI and human team configs if desired
- Group related configurations in subdirectories

### Configuration Validation

**Pre-Game Validation:**
- Verify all referenced files exist
- Check JSON syntax and structure
- Validate numeric ranges (HP > 0, etc.)
- Ensure at least one unit per team

**Runtime Checks:**
- Validate alliance references point to valid teams
- Check map tiles have required properties
- Verify unit configurations are complete

### Extensibility Guidelines

**Adding New Configuration Options:**
1. Update appropriate model classes (GameConfig, TeamConfig, etc.)
2. Add default values for backward compatibility
3. Update loading logic in GameInitState
4. Document new options and their effects

**Configuration Versioning:**
- Consider adding version fields for future migrations
- Maintain backward compatibility when possible
- Provide clear migration guides for breaking changes

## Troubleshooting Common Issues

### Configuration Loading Errors

**File Not Found:**
- Check file paths are relative to project root
- Verify file names match exactly (case-sensitive on some systems)
- Ensure files are included in project/deployment

**JSON Parsing Errors:**
- Validate JSON syntax using online validators
- Check for missing commas or brackets
- Ensure all strings are properly quoted

**Invalid Data:**
- Verify numeric values are positive where required
- Check boolean values are true/false (lowercase)
- Ensure required fields are present

### Runtime Configuration Issues

**Teams Not Loading:**
- Check team array in game configuration
- Verify individual team files exist and are valid
- Look for loading warnings in console output

**Alliance Problems:**
- Ensure team IDs in alliances match loaded teams
- Check alliance strings match supported types
- Verify alliance configuration syntax

**Map Issues:**
- Confirm map file exists and loads successfully
- Check tile coordinates don't overlap inappropriately
- Verify required tile properties are set

This configuration system provides flexible and extensible game setup while maintaining clear separation of concerns and robust error handling.