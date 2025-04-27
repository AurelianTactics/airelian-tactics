# Map Creation and Loading Guide

## Map Creation Checklist
- [ ] Determine map dimensions (e.g., 3x3, 4x5)
- [ ] Create a new JSON file in `AirelianTactics/Configs/MapConfigDirectory/your_map_name.json`
- [ ] Define all required tile properties (coordinates, movement permissions, terrain types)
- [ ] Edit `AirelianTactics/Configs/GameConfigDirectory/game_config.json` and update the `mapFile` path

## Map Structure
Maps in Airelian Tactics are defined using JSON files. Each map consists of a collection of tiles arranged in a grid pattern with x, y, and z coordinates.

### Map Configuration Format
```json
{
    "general": {
        "mapName": "MyMapName"
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
        },
        // Additional tiles...
    ]
}
```

### Map Properties

#### General Section
- `mapName`: The display name of the map

#### Tile Properties
- `tileId`: Unique identifier for the tile
- `x`, `y`: Position coordinates on the grid
- `z`: Height/elevation of the tile
- `standable`: Whether units can occupy this tile
- `traversable`: Whether units can move through this tile
- `terrain`: Type of terrain (affects gameplay mechanics)
- `canPlayerStart`: Whether player units can be initially placed on this tile

## Creating a New Map

1. Create a new JSON file in the `AirelianTactics/Configs/MapConfigDirectory` directory
2. Define the map structure according to the format above
3. Include all necessary tiles with appropriate properties
4. Ensure proper connectivity between tiles for unit movement

Example of a minimal 2x2 map:
```json
{
    "general": {
        "mapName": "Small2x2Map"
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
        },
        {
            "tileId": 1,
            "x": 1,
            "y": 0,
            "z": 0,
            "standable": true,
            "traversable": true,
            "terrain": "grass",
            "canPlayerStart": true
        },
        {
            "tileId": 2,
            "x": 0,
            "y": 1,
            "z": 0,
            "standable": true,
            "traversable": true,
            "terrain": "grass",
            "canPlayerStart": true
        },
        {
            "tileId": 3,
            "x": 1,
            "y": 1,
            "z": 0,
            "standable": true,
            "traversable": true,
            "terrain": "grass",
            "canPlayerStart": true
        }
    ]
}
```

## Loading a Map

Maps are loaded through the game configuration system. To use your custom map:

1. In `AirelianTactics/Configs/GameConfigDirectory/game_config.json`, update the map reference:
   ```json
   "map": {
     "mapFile": "AirelianTactics/Configs/MapConfigDirectory/your_map_file.json"
   }
   ```

## Technical Map Loading Flow

The map loading process involves several components working together:

1. `GameInitState.cs` initializes the game and loads configurations:
   ```csharp
   // In GameInitState.Update()
   LoadGameConfiguration();
   LoadMapConfiguration();
   ```

2. `MapConfigLoader.cs` handles the actual file loading:
   ```csharp
   public static MapConfig LoadMapConfig(string filePath)
   {
       string jsonString = File.ReadAllText(filePath);
       return JsonSerializer.Deserialize<MapConfig>(jsonString, options);
   }
   ```

3. The loaded map is stored in `GameContext.cs`:
   ```csharp
   // Map is accessible via
   GameContext.GameConfig.Map
   // Or the shorthand property
   GameContext.Map
   ```

4. `MapSetupState.cs` uses the loaded map data to create the game map:
   ```csharp
   // In MapSetupState.Enter() the map configuration is accessed
   if (GameContext.GameConfig != null)
   {
       Console.WriteLine($"Map to load: {config.Map.MapFile}");
       // ... map setup logic ...
   }
   ```

5. Map data flows to other states through the shared `GameContext` instance, which is passed to each state through the `StateManager`.

## Map Usage in Game

When a game session starts:

1. The `GameInitState` loads the map configuration from the file specified in the game config
2. The `MapSetupState` uses this configuration to create the playable map
3. The map tiles determine where units can move and which actions they can perform
4. Player starting positions are determined by tiles with `canPlayerStart: true`

## Advanced Map Features

### Terrain Types
Different terrain types can affect gameplay mechanics:
- Default/grass: No special effects
- Water: May restrict movement
- Elevated terrain: Height differences can affect combat

### Map Size Considerations
- Larger maps increase memory usage and may affect performance
- Ensure logical connectivity between tiles for proper pathfinding
- Consider gameplay balance when designing larger maps 