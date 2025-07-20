/// <summary>
/// Stores team related information and alliances with other teams
/// </summary>
public class CombatTeam {
    public int TeamId { get; set; }

    public bool IsDefeated { get; set; }

    /// <summary>
    /// Whether this team is controlled by AI.
    /// </summary>
    public bool IsAI { get; set; }

    public CombatTeam(int teamId, bool isAI = false) {
        this.TeamId = teamId;
        this.IsDefeated = false;
        this.IsAI = isAI;
    }

    public void SetDefeated(bool isDefeated) {
        this.IsDefeated = isDefeated;
    }
    
}
