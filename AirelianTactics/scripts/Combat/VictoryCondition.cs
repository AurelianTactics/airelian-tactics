public class VictoryCondition {

    private VictoryType victoryType;

    /// <summary>
    /// Default constructor for VictoryCondition.
    /// </summary>
    public VictoryCondition() {
        this.victoryType = VictoryType.LastTeamStanding;
    }

    public bool IsVictoryConditionMet(CombatTeamManager combatTeamManager) {
        bool isVictoryConditionMet = false;

        if (this.victoryType == VictoryType.LastTeamStanding) {
            int teamCount = combatTeamManager.GetTeamCount();
            
            foreach (CombatTeam team in combatTeamManager.GetTeams()) {
                if (team.IsDefeated) {
                    teamCount--;
                }
            }

            if( teamCount <= 1){
                isVictoryConditionMet = true;
            }
        }
        return isVictoryConditionMet;
    }

}

