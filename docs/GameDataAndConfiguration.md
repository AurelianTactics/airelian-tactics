
## Game Data and Configuration

### JSON Configuration Files
The game uses JSON files to configure various game elements, including teams and units. Configuration files are stored in the `AirelianTactics/Configs/` directory.

### GameContext System
The `GameContext` system provides a way to share data between different game states:

1. **GameContext Class**: A container for shared game data accessible across all states.
   - Holds the `TeamConfig` object loaded from JSON
   - Can be extended to store other game objects like maps, player data, etc.

2. **StateManager**: Initializes and manages the GameContext.
   - Creates a new GameContext instance when initialized
   - Passes the GameContext to each state when registered or activated
   - Provides a `GetGameContext()` method to access the context directly

3. **State Data Flow**: All states have access to the shared GameContext:
   - Each state implements the `IState` interface with a `GameContext` property
   - The `State` base class provides a default implementation of this property
   - Data stored in the context by one state is accessible to all other states

### Data Loading in GameInitState
The `GameInitState` loads initial game data:

1. **Team Configuration Loading**:
   - The file path is defined as `AirelianTactics/Configs/team_sample.json`
   - During the `Update()` method, `LoadTeamConfiguration()` is called
   - Uses `TeamConfigLoader.LoadTeamConfig()` to deserialize the JSON into a `TeamConfig` object
   - Stores the loaded `TeamConfig` in the `GameContext`

2. **Accessing Data in States**:
   - In states, the team configuration is accessed via `GameContext.TeamConfig`
   - The state can read information about the team and its units

### Adding New Configuration Data
To add new configuration data to the system:

1. Create a new model class for your data (similar to `TeamConfig`)
2. Add a property for your model in the `GameContext` class
3. Create a loader utility if needed (similar to `TeamConfigLoader`)
4. Load the data in an appropriate state and store it in the `GameContext`
5. Access the data from any state that needs it via the `GameContext` property

#### Team Configuration
TBD. This will likely be heavily updated until MVP. Not sure if I want an outdated version in this file yet.