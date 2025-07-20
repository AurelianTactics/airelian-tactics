# Game Flow

This document describes the complete game flow in Airelian Tactics, from application startup through game completion, including state transitions, input handling, combat loops, and system integration.

## Overview

The game follows a state-driven architecture where each phase of gameplay is managed by a specific state. The flow progresses through initialization, combat, and conclusion phases, with dynamic transitions based on game events and user actions.

**High-Level Flow:**
1. **Application Startup** → Program.cs initialization
2. **Game Initialization** → GameInitState loads configurations and sets up game
3. **Combat Loop** → CombatState manages turn-based combat with action states
4. **Game Conclusion** → GameEndState handles game completion

**Key Systems Integration:**
- **State Management**: Controls flow between game phases
- **Services Layer**: Provides persistent data and logic across states
- **Configuration System**: Loads game setup from JSON files
- **Time Management**: Coordinates turn order and action timing

## Application Startup Flow

### Program.cs Initialization

The application begins in Program.cs with comprehensive setup:

```csharp
static void Main(string[] args)
{
    Console.WriteLine("Airelian Tactics Game");
    
    // 1. Create state manager
    StateManager stateManager = new StateManager();
    
    // 2. Register event handlers
    stateManager.OnStateChanged += LogStateTransitions;
    
    // 3. Create and register all states
    RegisterAllStates(stateManager);
    
    // 4. Define state flow rules
    DefineStateTransitions(stateManager);
    
    // 5. Start with initialization
    stateManager.ChangeState<GameInitState>();
    
    // 6. Run main game loop
    RunGameLoop(stateManager);
}
```

### State Registration and Flow Definition

**State Registration:**
```csharp
GameInitState initState = new GameInitState(stateManager);
CombatState combatState = new CombatState(stateManager);
UnitActionState unitActionState = new UnitActionState(stateManager);
AIActionState aiActionState = new AIActionState(stateManager);
GameEndState endState = new GameEndState(stateManager);

stateManager.RegisterState(initState);
stateManager.RegisterState(combatState);
stateManager.RegisterState(unitActionState);
stateManager.RegisterState(aiActionState);
stateManager.RegisterState(endState);
```

**Flow Definitions:**
```csharp
// Linear progression
stateManager.DefineStateFlow<GameInitState, CombatState>();
stateManager.DefineStateFlow<CombatState, GameEndState>();

// Dynamic action state transitions (event-driven)
stateManager.DefineStateFlow<UnitActionState, CombatState>();
stateManager.DefineStateFlow<AIActionState, CombatState>();
```

### Main Game Loop

**Core Loop Structure:**
```csharp
bool isRunning = true;
while (isRunning)
{
    // Update current state
    stateManager.Update();
    
    // Handle state-specific input and termination
    if (stateManager.GetCurrentState() is GameEndState)
    {
        // Game completed - exit loop
        isRunning = false;
    }
    else if (stateManager.GetCurrentState() is UnitActionState unitActionState 
             && unitActionState.IsWaitingForInput)
    {
        // Handle user input for human players
        string input = Console.ReadLine();
        stateManager.HandleInput(input);
    }
    else
    {
        // Continue automatic processing
        Console.WriteLine("Continuing to update...");
    }
}
```

## Game Initialization Phase

### GameInitState Execution Flow

**Initialization Sequence:**
1. **Configuration Loading**
   - Load master game configuration from JSON
   - Load all team configurations referenced in game config
   - Load map configuration
   - Validate all loaded data

2. **Service Initialization**
   - Services already created by StateManager
   - Initialize Board with map data
   - Create PlayerUnits using snake draft algorithm
   - Place units on board tiles

3. **Combat Object Setup**
   - Initialize AllianceManager with team relationships
   - Configure VictoryCondition based on game settings
   - Create CombatTeam objects for team management

4. **State Completion**
   - Mark initialization as complete
   - Automatic transition to CombatState

### Data Flow During Initialization

**Configuration → Services → Combat Objects:**
```
JSON Files → GameContext → Services → Combat Ready
     ↓             ↓           ↓           ↓
game_config.json   ├─ UnitService ── PlayerUnits
team_*.json        ├─ Board ────── Map + Positions  
map_*.json         └─ Managers ─── Alliances, Victory
```

**Key Validations:**
- All configuration files exist and parse correctly
- At least one team loads successfully
- Map configuration is valid
- Units can be placed on available tiles

## Combat Phase Flow

### CombatState Main Loop

The CombatState manages the core gameplay through a sophisticated loop:

```csharp
public override void Update()
{
    while (true)
    {
        // 1. Check victory condition
        if (IsVictoryConditionMet())
        {
            CompleteState(); // Transition to GameEndState
            break;
        }
        
        // 2. Get next scheduled action
        GameTimeObject gameTimeObject = gameTimeManager.GetNextGameTimeObject();
        
        if (gameTimeObject != null)
        {
            // 3. Handle action based on type
            if (IsUnitTurn(gameTimeObject))
            {
                HandleUnitTurn(gameTimeObject);
                return; // Exit to action state
            }
            else
            {
                // 4. Process action immediately
                gameTimeManager.ProcessGameTimeObject(gameTimeObject);
            }
        }
        else
        {
            // 5. No actions queued - advance world time
            RunWorldTick();
        }
    }
}
```

### Turn Management Flow

**Unit Turn Detection:**
```csharp
private void HandleUnitTurn(GameTimeObject gameTimeObject)
{
    int unitId = gameTimeObject.ActorUnitId.Value;
    PlayerUnit currentUnit = unitService.GetUnit(unitId);
    CombatTeam team = combatTeamManager.GetTeamById(currentUnit.TeamId);
    
    if (team.IsAI)
    {
        // AI turn - transition to AIActionState
        var aiActionState = stateManager.GetState<AIActionState>();
        aiActionState.SetActorUnit(unitId);
        stateManager.ChangeState<AIActionState>();
    }
    else
    {
        // Human turn - transition to UnitActionState
        var unitActionState = stateManager.GetState<UnitActionState>();
        unitActionState.SetActorUnit(unitId);
        stateManager.ChangeState<UnitActionState>();
    }
}
```

**World Tick Processing:**
```csharp
private void RunWorldTick()
{
    // Increment global time counter
    stateManager.IncrementWorldTick();
    
    // TODO: Process status effects
    
    // Update all unit CT values
    unitService.IncrementCTAll();
    
    // Display current state for development
    PrintDevView();
}
```

## Action State Flow

### Human Player Turn Flow (UnitActionState)

**State Lifecycle:**
1. **Enter()**: Initialize turn, show action menu
2. **Update()**: Wait for user input or process selection
3. **Input Handling**: Process user commands and targeting
4. **Action Creation**: Generate GameTimeObject for chosen action
5. **Exit()**: Clean up and return to CombatState

**Input Processing Flow:**
```csharp
public override void HandleInput(string input)
{
    switch (currentPhase)
    {
        case ActionPhase.SelectingCommand:
            HandleCommandInput(input);  // "1" for Move, "2" for Act, "3" for Wait
            break;
        case ActionPhase.Targeting:
            HandleTargetingInput(input); // Coordinates or target selection
            break;
        case ActionPhase.Confirming:
            HandleConfirmationInput(input); // "y" to confirm, "n" to cancel
            break;
    }
}
```

**Action Phases:**
- **Command Selection**: Player chooses Move, Act, or Wait
- **Targeting**: Player selects destination or target
- **Confirmation**: Player confirms action before execution

### AI Player Turn Flow (AIActionState)

**Automated Decision Making:**
1. **Enter()**: Validate AI unit and initialize decision process
2. **Update()**: Execute AI decision algorithm immediately
3. **Decision Algorithm**: Priority-based action selection
4. **Action Creation**: Generate GameTimeObject for AI choice
5. **Exit()**: Complete turn and return to CombatState

**AI Decision Priority:**
```csharp
private void MakeAIDecision()
{
    // Priority 1: Attack available enemy
    var attackTarget = FindBestAttackTarget();
    if (attackTarget.HasValue)
    {
        CreateAttackAction(attackTarget.Value);
        return;
    }
    
    // Priority 2: Move toward nearest enemy
    var moveTarget = FindBestMoveTarget();
    if (moveTarget.HasValue)
    {
        CreateMoveAction(moveTarget.Value);
        return;
    }
    
    // Priority 3: Wait if no good options
    CreateWaitAction();
}
```

## Action Processing and Resolution

### GameTimeObject Processing

**Action Resolution Pipeline:**
1. **Action Creation**: UnitActionState or AIActionState creates GameTimeObject
2. **Queue Addition**: Action added to appropriate phase queue
3. **Retrieval**: GameTimeManager returns action when ready for processing
4. **Processing**: GameTimeObjectProcessor executes action effects
5. **State Updates**: Services update game state based on action results

**Processing Examples:**

**Move Action:**
```csharp
private bool ProcessMoveAction(GameTimeObject gto, PlayerUnit actor)
{
    Point targetPosition = gto.SpellName.TargetPoint;
    Point currentPosition = board.GetUnitPosition(actor.UnitId);
    
    // Execute movement
    bool success = board.MoveUnit(actor.UnitId, currentPosition, targetPosition);
    
    if (success)
    {
        unitService.EndUnitTurn(actor); // Reduce CT, clear turn flags
        return true;
    }
    
    return false;
}
```

**Attack Action:**
```csharp
private bool ProcessAttackAction(GameTimeObject gto, PlayerUnit actor)
{
    PlayerUnit target = GetTargetUnit(gto);
    
    // Calculate and apply damage
    int damage = CalculateDamage(actor, target, gto.SpellName);
    unitService.AlterHP(target, -damage);
    
    // End attacker's turn
    unitService.EndUnitTurn(actor);
    
    return true;
}
```

## State Transition Patterns

### Automatic Transitions

**Linear Progression:**
- GameInitState → CombatState (on completion)
- CombatState → GameEndState (on victory condition met)

**Transition Triggers:**
```csharp
// In State.Update()
if (stateWorkComplete)
{
    CompleteState(); // Triggers automatic transition
}

// In StateManager.Update()
if (currentState.IsCompleted)
{
    TransitionToNextState();
}
```

### Dynamic Transitions

**Event-Driven Transitions:**
- CombatState ↔ UnitActionState (human player turns)
- CombatState ↔ AIActionState (AI player turns)

**Context Passing:**
```csharp
// Before transition
var actionState = stateManager.GetState<UnitActionState>();
actionState.SetActorUnit(unitId); // Pass context

// Transition
stateManager.ChangeState<UnitActionState>();
```

### Transition Coordination

**State Data Persistence:**
- Services maintain state across transitions
- GameContext preserves configuration data
- No data loss during state changes

**Clean Transitions:**
```csharp
public override void Exit()
{
    // Cleanup state-specific resources
    RemoveEventListeners();
    
    // Services and shared data remain intact
    base.Exit();
}
```

## Input Handling System

### Input Routing

**Main Loop Input Handling:**
```csharp
IState currentState = stateManager.GetCurrentState();

if (currentState is UnitActionState unitActionState && unitActionState.IsWaitingForInput)
{
    // Get user input for human player turns
    string input = Console.ReadLine();
    stateManager.HandleInput(input);
}
else
{
    // States that don't require input continue automatically
    // or present simple continue/quit options
}
```

### State-Specific Input Processing

**UnitActionState Input Handling:**
- **Command Selection**: "1", "2", "3" for Move, Act, Wait
- **Targeting**: Coordinate input like "1,2" or target selection
- **Confirmation**: "y"/"n" for action confirmation

**Input Validation:**
```csharp
public override void HandleInput(string input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        ShowInputPrompt(); // Re-prompt user
        return;
    }
    
    // Process valid input based on current phase
    ProcessInputForCurrentPhase(input);
}
```

## Victory Conditions and Game Termination

### Victory Detection

**LastTeamStanding Victory:**
```csharp
public bool IsVictoryConditionMet(CombatTeamManager teamManager, UnitService unitService)
{
    // Update team defeat status
    teamManager.UpdateTeamStatus(unitService);
    
    // Count remaining teams
    int activeTeams = teamManager.combatTeams.Count(team => !team.IsDefeated);
    
    return activeTeams <= 1;
}
```

**Victory Check Integration:**
- Called at the beginning of each CombatState update cycle
- Triggers automatic transition to GameEndState when met
- Ensures immediate game conclusion when conditions are satisfied

### Game Conclusion Flow

**GameEndState Processing:**
1. **Victory Announcement**: Display game results
2. **Final State**: No further state transitions
3. **User Exit**: Wait for user confirmation to close application

**Application Termination:**
```csharp
// In main game loop
if (stateManager.GetCurrentState() is GameEndState)
{
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey(true);
    isRunning = false; // Exit main loop
}
```

## Error Handling and Edge Cases

### Configuration Errors

**Graceful Degradation:**
- Missing team files: Continue with loaded teams, log warnings
- Invalid JSON: Stop with detailed error message
- Missing map: Continue with empty board, log warning

### Runtime Errors

**Service Failures:**
- Unit service errors: Log and attempt recovery
- Board state inconsistencies: Validate and correct where possible
- Action processing failures: Log error, skip invalid actions

### State Transition Failures

**Error Recovery:**
- Invalid state transitions: Log error, maintain current state
- Service unavailability: Initialize missing services if possible
- Data corruption: Attempt to restore from last known good state

## Performance Considerations

### Loop Efficiency

**Combat Loop Optimization:**
- Early exit on victory condition
- Efficient GameTimeObject retrieval
- Minimal state validation overhead

### Memory Management

**Resource Cleanup:**
- States clean up temporary resources on exit
- Services maintain reasonable memory footprint
- No memory leaks during state transitions

### Responsive Input

**Input Handling:**
- Non-blocking input for responsive UI
- Quick state transitions for smooth gameplay
- Efficient input validation and processing

## Development and Testing Flow

### Debug Features

**Development Mode:**
- Comprehensive logging of state transitions
- Game state visualization (unit positions, CT values)
- Action processing details

**Testing Scenarios:**
- Single player vs AI
- Multiple AI teams
- Edge cases (single units, no valid moves)
- Configuration variations

### Extensibility

**Adding New States:**
1. Create state class extending State base
2. Register with StateManager
3. Define state flow rules
4. Implement state-specific logic
5. Update main loop if needed

**Modifying Game Flow:**
1. Update state transition rules
2. Modify victory conditions if needed
3. Add new input handling patterns
4. Test integration with existing states

This game flow provides a robust and flexible framework for turn-based tactical gameplay while maintaining clear separation of concerns and reliable state management throughout the entire game session. 