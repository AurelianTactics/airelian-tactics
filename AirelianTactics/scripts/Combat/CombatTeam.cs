/// <summary>
/// Stores team related information and alliances with other teams
/// </summary>
public class CombatTeam {
    public int TeamId { get; set; }

    public bool IsDefeated { get; set; }

    public CombatTeam(int teamId) {
        this.TeamId = teamId;
        this.IsDefeated = false;
    }

    public void SetDefeated(bool isDefeated) {
        this.IsDefeated = isDefeated;
    }
    
}
