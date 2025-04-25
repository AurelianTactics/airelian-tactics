using System.Collections.Generic;

/// <summary>
/// Represents a complete map configuration.
/// </summary>
public class MapConfig
{
    /// <summary>
    /// The path to the map file.
    /// </summary>
    public string MapFile { get; set; } = string.Empty;
    
    /// <summary>
    /// General map settings.
    /// </summary>
    public MapGeneralConfig General { get; set; } = new MapGeneralConfig();
    
    /// <summary>
    /// The collection of tiles that make up the map.
    /// </summary>
    public List<TileConfig> Tiles { get; set; } = new List<TileConfig>();
}

/// <summary>
/// General map settings.
/// </summary>
public class MapGeneralConfig
{
    /// <summary>
    /// The name of the map.
    /// </summary>
    public string MapName { get; set; } = string.Empty;
}

/// <summary>
/// Represents a single tile on the map.
/// </summary>
public class TileConfig
{
    /// <summary>
    /// The unique identifier for the tile.
    /// </summary>
    public int TileId { get; set; }
    
    /// <summary>
    /// The X coordinate of the tile.
    /// </summary>
    public int X { get; set; }
    
    /// <summary>
    /// The Y coordinate of the tile.
    /// </summary>
    public int Y { get; set; }
    
    /// <summary>
    /// The Z coordinate (height) of the tile.
    /// </summary>
    public int Z { get; set; }
    
    /// <summary>
    /// Whether units can stand on this tile.
    /// </summary>
    public bool Standable { get; set; }
    
    /// <summary>
    /// Whether units can move through this tile.
    /// </summary>
    public bool Traversable { get; set; }
    
    /// <summary>
    /// The type of terrain for this tile.
    /// </summary>
    public string Terrain { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether a player unit can start on this tile.
    /// </summary>
    public bool CanPlayerStart { get; set; }
} 