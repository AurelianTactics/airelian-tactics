# Combat Initialization

This document covers the initialization systems in `CombatState` that set up the combat environment, including alliances, victory conditions, teams, and unit population.

## Overview

When `CombatState` is entered, it performs several initialization steps to prepare the combat environment:

1. **Team Instantiation** - Creates `CombatTeam` objects from loaded team configurations
2. **Alliance Management** - Sets up relationships between teams based on game configuration
3. **Victory Condition Setup** - Initializes the win condition for the combat session
4. **Unit Population** - Creates `PlayerUnit` objects and assigns them fair unit IDs using a snake draft system

## Alliance System

### Components

- **`AllianceManager`** - Main class that manages team relationships
- **`Alliances` enum** - Defines relationship types between teams
- **Game Configuration** - JSON-based alliance definitions

### Alliance Types

The `Alliances` enum defines the following relationship types:

- `None` (0) - No alliance relationship
- `Neutral` (1 << 0) - Not hostile but not allied
- `Hero` (1 << 1) - Units under your control and on your side
- `Enemy` (1 << 2) - Hostile units
- `Allied` (1 << 3) - On your side but not under your control
- `Self` (1 << 4) - Always allied with self

### Configuration Format

Alliances are defined in the game configuration JSON file:

```json
{
  "general": {
    "alliances": [
      {
        "0": {"1": "Enemy"},
        "1": {"0": "Enemy"}
      }
    ]
  }
}
```

### Implementation Details

1. **Initialization in CombatState**:
   ```csharp
   private void InitializeAlliances()
   ```
   - Creates `AllianceManager` with alliance configurations from `GameContext`
   - Falls back to default neutral relationships if no configuration exists

2. **Alliance Storage**:
   - Uses nested `Dictionary<int, Dictionary<int, Alliances>>` structure
   - First key: Source team ID
   - Second key: Target team ID
   - Value: Alliance type

3. **Key Methods**:
   - `GetAlliance(int sourceTeamId, int targetTeamId)` - Returns alliance type between teams
   - `AreTeamsAllied(int team1Id, int team2Id)` - Checks if teams are allied (Hero or Allied)
   - `AreTeamsEnemies(int team1Id, int team2Id)` - Checks if teams are enemies

## Victory Condition System

### Components

- **`VictoryCondition`** - Main class that checks and manages win conditions
- **`VictoryType` enum** - Defines available victory condition types
- **`CombatTeamManager`** - Manages team status updates
- **`UnitService`** - Provides team defeat status information

### Victory Types

Currently supported victory conditions:
- `LastTeamStanding` - Game continues until all teams except one have all units incapacitated

### Configuration

Victory conditions are defined in the game configuration:

```json
{
  "general": {
    "victoryCondition": "LastTeamStanding"
  }
}
```

### Implementation Details

1. **Initialization in CombatState**:
   ```csharp
   private void InitializeVictoryCondition()
   ```
   - Creates `VictoryCondition` object with configuration string
   - Defaults to `LastTeamStanding` if no configuration exists

2. **Victory Checking Process**:
   - Called during combat update loop: `victoryCondition.IsVictoryConditionMet()`
   - Updates team status by checking if all units in each team are incapacitated
   - Returns `true` when only one team remains active

3. **Team Defeat Logic**:
   - `UnitService.IsTeamDefeated(int teamId)` checks if all team units are incapacitated
   - `CombatTeamManager.UpdateTeamStatus()` refreshes defeat status for all teams

## Team Instantiation

### Components

- **`CombatTeam`** - Represents a team in combat with basic properties
- **`CombatTeamManager`** - Collection and manager for all combat teams
- **`GameContext`** - Contains loaded team configurations

### Team Creation Process

1. **Loading in GameInitState**:
   - Teams are loaded from JSON files specified in game configuration
   - Stored as `List<TeamConfig>` in `GameContext.Teams`

2. **Instantiation in CombatState**:
   ```csharp
   private void InitializeTeams()
   ```
   - Creates `CombatTeam` objects for each loaded team configuration
   - Assigns sequential team IDs starting from 0
   - Adds teams to `CombatTeamManager`

### CombatTeam Properties

- `TeamId` - Unique identifier for the team
- `IsDefeated` - Boolean indicating if the team has been defeated

### Team Configuration Format

Teams are defined in separate JSON files:

```json
{
  "teamId": "0",
  "teamName": "Player Team",
  "units": [
    {
      "unitId": 1,
      "name": "Ralph",
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

## Unit Population and Snake Draft System

### Components

- **`PlayerUnit`** - Combat-ready unit objects with stats and properties
- **`UnitService`** - Dictionary-based storage and management of all units
- **Snake Draft Algorithm** - Fair unit ID assignment system

### Purpose of Snake Draft

Unit IDs serve as tiebreakers in turn order and other game mechanics. Lower unit IDs have priority, so fair distribution ensures no team has an inherent advantage.

### Snake Draft Algorithm

The algorithm distributes unit IDs fairly across teams:

1. **Collection Phase**: Gather all unit configurations from all teams
2. **Grouping Phase**: Organize units by team
3. **Assignment Phase**: Use alternating order per round:
   - **Even rounds** (0, 2, 4...): Team 0 → Team 1 → Team 2 → ...
   - **Odd rounds** (1, 3, 5...): Team N → Team N-1 → ... → Team 0

### Implementation Details

1. **Main Function**:
   ```csharp
   private void InitializeAllUnitsWithSnakeDraft()
   ```

2. **Unit Creation**:
   ```csharp
   private void CreateAndAddPlayerUnit(UnitConfig unitConfig, int teamId, int assignedUnitId)
   ```
   - Creates `PlayerUnit` from `UnitConfig`
   - Uses snake draft assigned ID instead of original unit ID
   - Adds to `UnitService` dictionary with specific ID

3. **Storage**:
   - Units stored in `Dictionary<int, PlayerUnit>` in `UnitService`
   - Key: Snake draft assigned unit ID
   - Value: `PlayerUnit` object

### Example Snake Draft

For 2 teams with 2 units each:

| Round | Order | Team 0 Unit | Team 1 Unit | Assigned IDs |
|-------|-------|-------------|-------------|--------------|
| 0 (even) | Forward | Unit A | Unit C | A=0, C=1 |
| 1 (odd) | Reverse | Unit B | Unit D | D=2, B=3 |

Final ID assignment: Team 0 gets IDs 0,3 and Team 1 gets IDs 1,2

### PlayerUnit Properties

Key properties set during creation:
- `UnitId` - Snake draft assigned ID (used for tiebreakers)
- `TeamId` - Team the unit belongs to
- `IsIncapacitated` - Health status (initially false)
- `IsMidActiveTurn` - Turn state (initially false)
- Combat stats from configuration (HP, Speed, PA, Move, Jump, Initial CT)

## Integration with Game Flow

These initialization systems work together to prepare combat:

1. **GameInitState** loads configurations from JSON files
2. **CombatState.Enter()** calls initialization methods in sequence:
   - `InitializeTeams()` - Creates teams and populates units with snake draft
   - `InitializeAlliances()` - Sets up team relationships
   - `InitializeVictoryCondition()` - Prepares win condition checking
3. **Combat Loop** uses these systems:
   - Alliance relationships affect targeting and AI behavior
   - Victory condition is checked each update cycle
   - Unit IDs determine turn order priority
   - Team status affects victory condition evaluation

## Configuration Files

### Game Configuration
Located at: `AirelianTactics/Configs/GameConfigDirectory/game_config.json`

### Team Configurations  
Located at: `AirelianTactics/Configs/TeamConfigDirectory/`
- `team_sample.json` - Player team
- `team_sample_2.json` - AI team

## Related Classes

- `GameContext` - Shared data container
- `GameConfig` - Game configuration model
- `TeamConfig` / `UnitConfig` - Team and unit configuration models
- `StateManager` - Manages game state transitions
- `State` base class - Provides `GameContext` access to all states 