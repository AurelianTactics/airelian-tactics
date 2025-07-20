using AirelianTactics.Services;
using System.Collections.Generic;

/// <summary>
/// A collection of teams that are in combat.
/// </summary>
public class CombatTeamManager {

    public List<CombatTeam> combatTeams;

    public CombatTeamManager() {
        this.combatTeams = new List<CombatTeam>();
    }

    public void AddTeam(CombatTeam combatTeam) {
        this.combatTeams.Add(combatTeam);
    }

    public int GetTeamCount() {
        return this.combatTeams.Count;
    }

    /// <summary>
    /// Get a combat team by its team ID
    /// </summary>
    /// <param name="teamId">The team ID to search for</param>
    /// <returns>The CombatTeam with the specified ID, or null if not found</returns>
    public CombatTeam GetTeamById(int teamId) {
        foreach (CombatTeam team in this.combatTeams) {
            if (team.TeamId == teamId) {
                return team;
            }
        }
        return null;
    }

    public void UpdateTeamStatus(UnitService unitService) {
        foreach (CombatTeam combatTeam in this.combatTeams) {
            combatTeam.SetDefeated(unitService.IsTeamDefeated(combatTeam.TeamId));
        }
    }

}


    
    
    
    
