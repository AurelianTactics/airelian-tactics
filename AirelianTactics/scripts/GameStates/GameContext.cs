using System.Collections.Generic;

/// <summary>
/// Contains shared game data that can be accessed across different game states.
/// </summary>
public class GameContext
{
    /// <summary>
    /// Gets or sets the game configuration loaded from JSON.
    /// </summary>
    public GameConfig GameConfig { get; set; }

    /// <summary>
    /// Gets or sets the collection of loaded team configurations.
    /// </summary>
    public List<TeamConfig> Teams { get; set; } = new List<TeamConfig>();

    /// <summary>
    /// Gets or sets the single team configuration (for backward compatibility).
    /// </summary>
    public TeamConfig TeamConfig 
    { 
        get => Teams.Count > 0 ? Teams[0] : null; 
        set 
        {
            if (Teams.Count > 0)
                Teams[0] = value;
            else
                Teams.Add(value);
        }
    }

    /// <summary>
    /// Gets or sets the map configuration.
    /// </summary>
    public MapConfig Map => GameConfig?.Map;

    /// <summary>
    /// Constructor initializes an empty GameContext.
    /// </summary>
    public GameContext()
    {
        GameConfig = new GameConfig();
    }
} 