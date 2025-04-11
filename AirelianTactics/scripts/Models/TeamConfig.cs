using System.Collections.Generic;

/// <summary>
/// Represents the configuration for a team in the game.
/// </summary>
public class TeamConfig
{
    /// <summary>
    /// The unique identifier for the team.
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// The name of the team.
    /// </summary>
    public string TeamName { get; set; }

    /// <summary>
    /// The units belonging to this team.
    /// </summary>
    public List<UnitConfig> Units { get; set; } = new List<UnitConfig>();
}

/// <summary>
/// Represents the configuration for a unit in the game.
/// </summary>
public class UnitConfig
{
    /// <summary>
    /// The unique identifier for the unit.
    /// </summary>
    public int UnitId { get; set; }

    /// <summary>
    /// The name of the unit.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The unit's hit points.
    /// </summary>
    public int HP { get; set; }

    /// <summary>
    /// The unit's speed.
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// The unit's physical attack power.
    /// </summary>
    public int PA { get; set; }

    /// <summary>
    /// The unit's movement range.
    /// </summary>
    public int Move { get; set; }

    /// <summary>
    /// The unit's jump height.
    /// </summary>
    public int Jump { get; set; }

    /// <summary>
    /// The initial charge time value.
    /// </summary>
    public int InitialCT { get; set; } = 0;
} 