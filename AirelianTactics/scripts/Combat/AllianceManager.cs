using System;
using System.Collections.Generic;

/// <summary>
/// Manages alliances between teams in combat
/// </summary>
public class AllianceManager
{
    // Team alliances are stored in a nested dictionary:
    // - First key: Team ID (source team)
    // - Second key: Team ID (target team)
    // - Value: Alliance type (Neutral, Enemy, Allied, etc.)
    private Dictionary<int, Dictionary<int, Alliances>> teamAlliances = new Dictionary<int, Dictionary<int, Alliances>>();
    
    /// <summary>
    /// Constructor that initializes an empty alliance manager with default neutral alliances
    /// </summary>
    public AllianceManager()
    {
        // Empty alliance manager
    }
    
    /// <summary>
    /// Constructor that takes a list of alliance configurations from the game config
    /// </summary>
    /// <param name="allianceConfigs">List of dictionaries representing team alliances</param>
    public AllianceManager(List<Dictionary<string, Dictionary<string, string>>> allianceConfigs)
    {
        if (allianceConfigs != null)
        {
            // Process each alliance configuration
            foreach (var allianceConfig in allianceConfigs)
            {
                foreach (var teamEntry in allianceConfig)
                {
                    // Get the source team ID
                    if (int.TryParse(teamEntry.Key, out int sourceTeamId))
                    {
                        // Ensure the source team has an entry in the dictionary
                        if (!teamAlliances.ContainsKey(sourceTeamId))
                        {
                            teamAlliances[sourceTeamId] = new Dictionary<int, Alliances>();
                        }
                        
                        // Process target teams
                        foreach (var targetEntry in teamEntry.Value)
                        {
                            if (int.TryParse(targetEntry.Key, out int targetTeamId))
                            {
                                // Parse the alliance type
                                if (Enum.TryParse(targetEntry.Value, out Alliances allianceType))
                                {
                                    // Set the alliance
                                    teamAlliances[sourceTeamId][targetTeamId] = allianceType;
                                    Console.WriteLine($"Set alliance for team {sourceTeamId} to team {targetTeamId}: {allianceType}");
                                }
                                else
                                {
                                    Console.WriteLine($"Warning: Could not parse alliance type '{targetEntry.Value}'");
                                    // Default to neutral if not recognized
                                    teamAlliances[sourceTeamId][targetTeamId] = Alliances.Neutral;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Get the alliance type between two teams
    /// </summary>
    /// <param name="sourceTeamId">Source team ID</param>
    /// <param name="targetTeamId">Target team ID</param>
    /// <returns>Alliance type between the teams</returns>
    public Alliances GetAlliance(int sourceTeamId, int targetTeamId)
    {
        // Same team is always allied with itself
        if (sourceTeamId == targetTeamId)
        {
            return Alliances.Self;
        }
        
        // Check if the source team has alliances defined
        if (teamAlliances.TryGetValue(sourceTeamId, out var targetAlliances))
        {
            // Check if there's a specific alliance defined with the target team
            if (targetAlliances.TryGetValue(targetTeamId, out var alliance))
            {
                return alliance;
            }
        }
        
        // Default to neutral if no alliance is explicitly defined
        return Alliances.Neutral;
    }
    
    /// <summary>
    /// Set the alliance between two teams
    /// </summary>
    /// <param name="sourceTeamId">Source team ID</param>
    /// <param name="targetTeamId">Target team ID</param>
    /// <param name="alliance">Type of alliance</param>
    public void SetAlliance(int sourceTeamId, int targetTeamId, Alliances alliance)
    {
        // Ensure the source team has an entry
        if (!teamAlliances.ContainsKey(sourceTeamId))
        {
            teamAlliances[sourceTeamId] = new Dictionary<int, Alliances>();
        }
        
        // Set the alliance
        teamAlliances[sourceTeamId][targetTeamId] = alliance;
    }
    
    /// <summary>
    /// Checks if two teams are allied (Hero or Allied alliance types)
    /// </summary>
    /// <param name="team1Id">First team ID</param>
    /// <param name="team2Id">Second team ID</param>
    /// <returns>True if the teams are allied, false otherwise</returns>
    public bool AreTeamsAllied(int team1Id, int team2Id)
    {
        Alliances alliance = GetAlliance(team1Id, team2Id);
        return alliance == Alliances.Hero || alliance == Alliances.Allied;
    }
    
    /// <summary>
    /// Checks if two teams are enemies
    /// </summary>
    /// <param name="team1Id">First team ID</param>
    /// <param name="team2Id">Second team ID</param>
    /// <returns>True if the teams are enemies, false otherwise</returns>
    public bool AreTeamsEnemies(int team1Id, int team2Id)
    {
        return GetAlliance(team1Id, team2Id) == Alliances.Enemy;
    }
} 