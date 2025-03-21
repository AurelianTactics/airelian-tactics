/// <summary>
/// A collection of teams that are in combat.
/// </summary>
public class CombatTeamManager {

    private List<CombatTeam> combatTeams;

    public CombatTeamManager() {
        this.combatTeams = new List<CombatTeam>();
    }

    public void AddTeam(CombatTeam combatTeam) {
        this.combatTeams.Add(combatTeam);
    }

    public int GetTeamCount() {
        return this.combatTeams.Count;
    }

}


    
    
    
    
