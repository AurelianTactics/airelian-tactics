# Services Architecture

This document covers the services architecture in Airelian Tactics, including the service layer pattern, shared service management through StateManager, service responsibilities, and integration with the game's state system.

## Overview

The services architecture provides a clean separation of concerns by organizing game logic into specialized service classes. These services are managed by the StateManager and shared across all game states, ensuring consistent data access and behavior throughout the game session.

**Key Benefits:**
- **Separation of Concerns**: Each service handles a specific domain of game logic
- **State Independence**: Services persist across state transitions
- **Consistent Access**: All states access the same service instances
- **Centralized Management**: StateManager coordinates service lifecycle

**Core Services:**
- **UnitService**: Manages all player units and their states
- **SpellService**: Handles spell and ability processing (future implementation)
- **StatusService**: Manages status effects on units (future implementation)
- **Board**: Manages map tiles and unit positions
- **GameTimeManager**: Controls turn order and action timing

## Service Management Pattern

### StateManager as Service Container

The StateManager acts as a service container, creating and maintaining shared service instances:

```csharp
public class StateManager
{
    // Shared services that persist across all game states
    public UnitService UnitService { get; private set; }
    public SpellService SpellService { get; private set; }
    public StatusService StatusService { get; private set; }
    public Board Board { get; private set; }
    public GameTimeManager GameTimeManager { get; private set; }
    
    public StateManager()
    {
        // Initialize shared services
        UnitService = new UnitService();
        SpellService = new SpellService();
        StatusService = new StatusService();
        Board = new Board();
        
        // GameTimeManager requires other services
        GameTimeManager = new GameTimeManager(UnitService, SpellService, StatusService, Board);
    }
}
```

### Service Lifecycle

**Initialization:**
1. **Service Creation**: Services instantiated in StateManager constructor
2. **Dependency Injection**: Services that depend on others receive references
3. **State Registration**: All states gain access to services through StateManager

**Runtime Access:**
```csharp
// In any state class
public class ExampleState : State
{
    public override void Enter()
    {
        base.Enter();
        
        // Access services through StateManager
        var unitService = stateManager.UnitService;
        var board = stateManager.Board;
        var gameTimeManager = stateManager.GameTimeManager;
    }
}
```

**Persistence:**
- Services maintain state across game state transitions
- Data persists throughout the entire game session
- No service cleanup required during state changes

## UnitService Architecture

### Core Responsibilities

The UnitService manages all aspects of player unit data and behavior:

**Primary Functions:**
- **Unit Storage**: Maintains dictionary of all PlayerUnit objects
- **State Management**: Tracks unit health, CT, and status flags
- **Turn Management**: Handles turn eligibility and completion
- **Team Queries**: Provides team-based unit operations

### Data Storage

**Unit Dictionary:**
```csharp
public class UnitService : IUnitService
{
    public Dictionary<int, PlayerUnit> unitDict { get; private set; }
    
    public UnitService()
    {
        unitDict = new Dictionary<int, PlayerUnit>();
    }
}
```

**Key-Value Structure:**
- **Key**: Unit ID (assigned by snake draft during initialization)
- **Value**: PlayerUnit object with all unit data
- **Benefits**: O(1) lookup time, unique unit identification

### Unit Management Operations

**Unit Addition:**
```csharp
public void AddUnitWithId(PlayerUnit playerUnit, int unitId)
{
    playerUnit.UnitId = unitId;  // Ensure ID consistency
    unitDict[unitId] = playerUnit;
}
```

**Unit Queries:**
```csharp
public PlayerUnit GetNextActiveTurnPlayerUnit()
{
    return unitDict.Values
        .Where(unit => unit.IsEligibleForActiveTurn())
        .OrderByDescending(unit => unit.StatTotalCT)
        .ThenBy(unit => unit.UnitId)  // Tiebreaker
        .FirstOrDefault();
}
```

**State Checks:**
```csharp
public bool IsTeamDefeated(int teamId)
{
    return unitDict.Values
        .Where(unit => unit.TeamId == teamId)
        .All(unit => unit.IsIncapacitated);
}
```

### Turn Order Management

**CT-Based Turn Eligibility:**
```csharp
public bool IsEligibleForActiveTurn(PlayerUnit unit)
{
    return !unit.IsIncapacitated && unit.StatTotalCT >= 100;
}
```

**Turn Completion:**
```csharp
public void EndUnitTurn(PlayerUnit unit)
{
    unit.EndTurn();  // Reduces CT by 100, clears turn flags
}
```

**CT Updates:**
```csharp
public void IncrementCTAll()
{
    foreach (var unit in unitDict.Values)
    {
        if (!unit.IsIncapacitated)
        {
            unit.AddCT();  // Adds Speed to CT
        }
    }
}
```

## Board Service Architecture

### Map and Position Management

The Board service manages the game map and unit positioning:

**Core Functions:**
- **Map Loading**: Loads map structure from configuration
- **Position Tracking**: Maintains unit positions on tiles
- **Movement Validation**: Checks movement legality
- **Spatial Queries**: Provides position-based lookups

### Data Structure

**Tile Storage:**
```csharp
public class Board
{
    public Dictionary<Point, Tile> tiles { get; private set; }
    
    public Board()
    {
        tiles = new Dictionary<Point, Tile>();
    }
}
```

**Position-Unit Mapping:**
- Each tile can contain zero or one unit
- Unit positions tracked through tile occupancy
- Bidirectional lookups (position→unit, unit→position)

### Position Operations

**Unit Placement:**
```csharp
public bool PlaceUnitOnTile(Tile tile, int unitId)
{
    if (tile.occupyingUnitId == null)
    {
        tile.occupyingUnitId = unitId;
        return true;
    }
    return false;  // Tile already occupied
}
```

**Movement Execution:**
```csharp
public bool MoveUnit(int unitId, Point fromPosition, Point toPosition)
{
    // Validate movement
    if (!IsValidMove(fromPosition, toPosition)) return false;
    
    // Clear old position
    tiles[fromPosition].occupyingUnitId = null;
    
    // Set new position
    tiles[toPosition].occupyingUnitId = unitId;
    
    return true;
}
```

**Position Queries:**
```csharp
public Point? GetUnitPosition(int unitId)
{
    return tiles.FirstOrDefault(kvp => kvp.Value.occupyingUnitId == unitId).Key;
}

public int? GetUnitAtPosition(Point position)
{
    return tiles.TryGetValue(position, out Tile tile) ? tile.occupyingUnitId : null;
}
```

## GameTimeManager Service

### Time and Action Coordination

The GameTimeManager coordinates timing, turn order, and action processing:

**Service Dependencies:**
```csharp
public class GameTimeManager
{
    private UnitService unitService;
    private GameTimeObjectProcessor processor;
    
    public GameTimeManager(UnitService unitService, SpellService spellService, 
        StatusService statusService, Board board)
    {
        this.unitService = unitService;
        this.processor = new GameTimeObjectProcessor(unitService, spellService, statusService, board);
    }
}
```

**Integration Benefits:**
- **Unit Queries**: Direct access to unit turn eligibility
- **Action Processing**: Coordinates with other services for action execution
- **State Persistence**: Maintains action queues across state transitions

## SpellService Architecture (Future Implementation)

### Planned Responsibilities

The SpellService will handle all spell and ability-related operations:

**Intended Functions:**
- **Spell Registration**: Catalog of available spells and abilities
- **Effect Calculation**: Damage, healing, and status effect computation
- **Range Validation**: Spell targeting and range checking
- **Resource Management**: MP, spell cooldowns, and usage tracking

### Current Implementation Status

**Placeholder Implementation:**
```csharp
namespace AirelianTactics.Services
{
    public class SpellService : ISpellService
    {
        // Currently contains commented-out placeholder code
        // Future implementation will add comprehensive spell system
    }
}
```

**Future Integration:**
- Will integrate with GameTimeObjectProcessor for spell effects
- Will provide spell validation for AI and player actions
- Will support complex spell mechanics and interactions

## StatusService Architecture (Future Implementation)

### Planned Responsibilities

The StatusService will manage status effects and temporary modifiers:

**Intended Functions:**
- **Status Tracking**: Active status effects on units
- **Effect Processing**: Status effect updates each turn/tick
- **Modifier Calculation**: Stat modifications from status effects
- **Duration Management**: Status effect expiration and removal

### Current Implementation Status

**Placeholder Implementation:**
```csharp
namespace AirelianTactics.Services
{
    public class StatusService : IStatusService
    {
        // Currently contains commented-out placeholder code
        // Future implementation will add status effect system
    }
}
```

## Service Interface Pattern

### Interface Definitions

Services implement interfaces for testability and flexibility:

**Example Interface:**
```csharp
namespace AirelianTactics.Services
{
    public interface IUnitService
    {
        Dictionary<int, PlayerUnit> unitDict { get; }
        void AddUnitWithId(PlayerUnit playerUnit, int unitId);
        PlayerUnit GetNextActiveTurnPlayerUnit();
        bool IsTeamDefeated(int teamId);
        void EndUnitTurn(PlayerUnit unit);
        void IncrementCTAll();
    }
}
```

**Benefits:**
- **Testability**: Interfaces enable mock service creation for testing
- **Flexibility**: Alternative implementations possible without changing consumers
- **Dependency Injection**: Clear contracts between services and consumers

### Service Dependencies

**Dependency Graph:**
```
StateManager
├── UnitService (no dependencies)
├── SpellService (no dependencies) 
├── StatusService (no dependencies)
├── Board (no dependencies)
└── GameTimeManager (depends on: UnitService, SpellService, StatusService, Board)
```

**Dependency Management:**
- Services with no dependencies created first
- Dependent services created after their dependencies
- Circular dependencies avoided through careful design

## Integration with State System

### State Access Pattern

All states gain automatic access to services through StateManager:

**Base State Access:**
```csharp
public abstract class State : IState
{
    protected StateManager stateManager;
    
    public State(StateManager stateManager)
    {
        this.stateManager = stateManager;
    }
    
    // States can access services via stateManager properties
}
```

**Example Usage in States:**
```csharp
public class CombatState : State
{
    public override void Enter()
    {
        base.Enter();
        
        // Access shared services
        var unitService = stateManager.UnitService;
        var gameTimeManager = stateManager.GameTimeManager;
        var board = stateManager.Board;
        
        // Use services for combat logic
        var nextUnit = unitService.GetNextActiveTurnPlayerUnit();
    }
}
```

### Service State Persistence

**Cross-State Data Continuity:**
- Unit data persists from GameInitState through CombatState to GameEndState
- Board state maintained across action state transitions
- GameTimeManager queues preserved during state changes

**Benefits:**
- **Seamless Transitions**: No data loss during state changes
- **Consistent State**: All states see the same game world
- **Simplified Logic**: States don't need to manage persistent data

## Service Testing and Development

### Testing Strategies

**Unit Testing Services:**
```csharp
[Test]
public void UnitService_GetNextActiveTurnUnit_ReturnsHighestCTUnit()
{
    // Arrange
    var unitService = new UnitService();
    var unit1 = new PlayerUnit(/* parameters */);
    var unit2 = new PlayerUnit(/* parameters */);
    
    // Act
    var nextUnit = unitService.GetNextActiveTurnPlayerUnit();
    
    // Assert
    Assert.AreEqual(expectedUnit, nextUnit);
}
```

**Integration Testing:**
```csharp
[Test]
public void StateManager_ServicesIntegration_AllStatesCanAccessServices()
{
    // Test that all states can successfully access and use services
}
```

### Development Tools

**Service Debugging:**
- **Unit Information Logging**: UnitService provides debug output for unit states
- **Board Visualization**: Board service can print map layout with unit positions
- **Action Tracking**: GameTimeManager logs action processing

**Performance Monitoring:**
- **Dictionary Operations**: O(1) lookups for unit and tile access
- **Memory Usage**: Services maintain reasonable memory footprint
- **State Transition Time**: Services don't significantly impact state change performance

## Best Practices and Guidelines

### Service Design Principles

**Single Responsibility:**
- Each service handles one domain of game logic
- Clear boundaries between service responsibilities
- Minimal overlap in functionality

**Dependency Management:**
- Keep service dependencies minimal and explicit
- Avoid circular dependencies
- Use dependency injection where appropriate

**State Management:**
- Services maintain game state, not UI state
- Thread-safe operations where necessary
- Clear data ownership responsibilities

### Extension Guidelines

**Adding New Services:**
1. Create service interface first
2. Implement service class
3. Add to StateManager initialization
4. Update dependent services if needed
5. Document service responsibilities

**Modifying Existing Services:**
1. Consider backward compatibility
2. Update interface if needed
3. Test integration with dependent services
4. Update documentation

### Performance Considerations

**Efficient Data Structures:**
- Use dictionaries for O(1) lookups
- Avoid unnecessary enumeration operations
- Cache frequently accessed data

**Memory Management:**
- Clean up resources when appropriate
- Avoid memory leaks in service operations
- Monitor service memory usage during development

This services architecture provides a robust foundation for game logic organization while maintaining clean separation of concerns and efficient data access patterns throughout the game session. 