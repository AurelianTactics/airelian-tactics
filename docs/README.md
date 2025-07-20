# Airelian Tactics - Documentation

## Overview
Airelian Tactics is a turn-based tactics game, repurposed from OpenTactics/AurelianTactics codebase. This documentation provides comprehensive coverage of the project architecture, systems, and gameplay mechanics.

The game features a sophisticated state-driven architecture with AI opponents, configurable teams and maps, and a robust turn-based combat system powered by charge time (CT) mechanics.

## Core Documentation

### Architecture and Systems
- **[State Management System](StateManagement.md)** - State machine architecture, transitions, and lifecycle management
- **[Services Architecture](ServicesArchitecture.md)** - Service layer pattern, shared services, and dependency management
- **[Game Flow](GameFlow.md)** - Complete game execution from startup to conclusion, including all state transitions
- **[Game Time and Turn Order](GameTimeAndTurnOrder.md)** - CT system, phase-based processing, and turn resolution

### Game Setup and Configuration
- **[Game Initialization](CombatInitialization.md)** - Complete initialization process including configuration loading, unit creation, and combat setup
- **[Game Data and Configuration](GameDataAndConfiguration.md)** - JSON configuration system, team setup, and map definitions
- **[Map Creation and Loading](MapCreationAndLoading.md)** - Map structure, tile properties, and map configuration guide

### Combat and Gameplay
- **[AI System](AISystem.md)** - AI decision-making, team configuration, and behavior algorithms
- **[Combat Mechanics](CombatMechanics.md)** - Damage calculation, unit stats, incapacitation, and victory conditions
- **[Unit Actions and Abilities](UnitActionsAndAbilities.md)** - Available actions, SpellName system, and targeting mechanics

## Quick Start Guide

### Building and Running
1. **Prerequisites**: .NET 9.0 SDK (may work on earlier versions)
2. **Clone Repository**: `git clone <repository-url>`
3. **Navigate to Project**: `cd airelian-tactics`
4. **Run Application**: `dotnet run --project AirelianTactics`

### Basic Game Flow
1. **Initialization**: Game loads configurations and sets up the battlefield
2. **Combat**: Turn-based tactical combat with human and AI players
3. **Victory**: Game ends when only one team remains standing

## Architecture Overview

### State-Driven Design
The game uses a state machine architecture with the following core states:
- **GameInitState**: Loads configurations and initializes game components
- **CombatState**: Manages turn-based combat loop
- **UnitActionState**: Handles human player input and actions
- **AIActionState**: Processes AI decision-making and actions
- **GameEndState**: Displays results and handles game conclusion

### Services Layer
Shared services provide consistent data access across all states:
- **UnitService**: Unit management and turn processing
- **Board**: Map and position management
- **GameTimeManager**: Turn order and action timing
- **AllianceManager**: Team relationships and targeting
- **SpellService/StatusService**: Future ability and status systems

### Configuration System
JSON-based configuration enables flexible game setup:
- **Game Config**: Victory conditions, team references, and alliance definitions
- **Team Configs**: Unit statistics, AI flags, and team properties
- **Map Configs**: Battlefield layout and tile properties

## Key Features

### AI Opponents
- **Intelligent Decision Making**: Priority-based AI with target selection and movement
- **Mixed Teams**: Human and AI teams can play together in various configurations
- **Alliance Support**: Complex team relationships including allies, enemies, and neutrals

### Turn-Based Combat
- **Charge Time System**: Speed-based turn order with strategic timing
- **Action Variety**: Move, Attack, and Wait actions with future expansion planned
- **Tactical Positioning**: Unit placement and movement affect combat outcomes

### Flexible Configuration
- **JSON Setup**: Easy modification of teams, maps, and game rules
- **Snake Draft**: Fair unit ID assignment prevents gameplay imbalances
- **Extensible Design**: Framework supports future features and enhancements

## Development Information

### Project Goals
- Testing AI development tools and methodologies
- Combining existing codebase with modern development practices
- Exploring AI-augmented development workflows

### Minimum Viable Product (MVP)
- ✅ Basic turn-based combat system
- ✅ AI opponent implementation
- 🔄 Future: Online play capabilities
- 🔄 Future: AI-augmented development features

### Testing and Quality
- **Development Mode**: Comprehensive logging and state visualization
- **Debug Features**: Unit information display, board visualization, and action tracking
- **Testing Framework**: Unit tests and integration tests for core systems

## Contributing and Development

### Code Organization
```
AirelianTactics/
├── scripts/
│   ├── GameStates/      # State machine implementation
│   ├── Services/        # Shared service layer
│   ├── Combat/          # Combat mechanics and AI
│   ├── Models/          # Configuration models
│   └── Utils/           # Configuration loaders
├── Configs/             # JSON configuration files
└── Program.cs           # Application entry point
```

### Adding New Features
1. **Review Architecture**: Understand state management and service patterns
2. **Follow Patterns**: Use existing patterns for consistency
3. **Update Documentation**: Keep documentation current with changes
4. **Test Integration**: Verify compatibility with existing systems

### Documentation Standards
- **Comprehensive Coverage**: Document all major systems and features
- **Code Examples**: Include practical implementation examples
- **Clear Structure**: Logical organization with navigation aids
- **Current Information**: Keep documentation synchronized with code

## Advanced Topics

### Performance Considerations
- **Efficient Data Structures**: Dictionary-based lookups for O(1) access
- **Memory Management**: Proper cleanup and resource management
- **State Transitions**: Minimal overhead during state changes

### Extensibility Framework
- **Modular Design**: Clean separation between systems
- **Interface Patterns**: Testable and replaceable components
- **Configuration Driven**: Easy modification without code changes

### Future Enhancements
- **Advanced AI**: More sophisticated decision-making algorithms
- **Enhanced Combat**: Status effects, terrain effects, and special abilities
- **Networking**: Online multiplayer capabilities
- **UI Improvements**: Graphical interface and enhanced user experience

## Troubleshooting

### Common Issues
- **Configuration Errors**: Verify JSON syntax and file paths
- **Missing Dependencies**: Ensure .NET SDK is properly installed
- **Runtime Errors**: Check console output for detailed error messages

### Getting Help
- **Documentation**: Review relevant documentation sections
- **Code Examples**: Examine existing implementations for patterns
- **Testing**: Use development mode for detailed state information

This documentation provides comprehensive coverage of the Airelian Tactics codebase and should serve as a complete reference for understanding, modifying, and extending the game.
