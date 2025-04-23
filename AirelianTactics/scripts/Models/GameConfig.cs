using System.Collections.Generic;

/// <summary>
/// Represents the main game configuration.
/// </summary>
public class GameConfig
{
    /// <summary>
    /// General game settings.
    /// </summary>
    public GeneralConfig General { get; set; } = new GeneralConfig();
    
    /// <summary>
    /// List of team files to load.
    /// </summary>
    public List<string> Teams { get; set; } = new List<string>();
    
    /// <summary>
    /// Map configuration.
    /// </summary>
    public MapConfig Map { get; set; } = new MapConfig();
}

/// <summary>
/// General game settings.
/// </summary>
public class GeneralConfig
{
    /// <summary>
    /// The victory condition for the game.
    /// </summary>
    public string VictoryCondition { get; set; } = string.Empty;
    
    /// <summary>
    /// Team alliances.
    /// </summary>
    public List<TeamAlliance> Alliances { get; set; } = new List<TeamAlliance>();
}

/// <summary>
/// Represents an alliance between teams.
/// </summary>
public class TeamAlliance
{
    /// <summary>
    /// IDs of the teams in this alliance.
    /// </summary>
    public List<int> TeamIds { get; set; } = new List<int>();
}

/// <summary>
/// Map configuration.
/// </summary>
public class MapConfig
{
    /// <summary>
    /// The path to the map file.
    /// </summary>
    public string MapFile { get; set; } = string.Empty;
} 