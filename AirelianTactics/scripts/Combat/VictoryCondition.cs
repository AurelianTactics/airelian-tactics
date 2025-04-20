using AirelianTactics.Services;
using System.Collections.Generic;

public class VictoryCondition {

    private VictoryType victoryType;

    /// <summary>
    /// Default constructor for VictoryCondition.
    /// </summary>
    public VictoryCondition() {
        this.victoryType = VictoryType.LastTeamStanding;
    }

    public bool IsVictoryConditionMet(CombatTeamManager combatTeamManager, UnitService unitService) {
        bool isVictoryConditionMet = false;

        if (this.victoryType == VictoryType.LastTeamStanding) {

            // update team status of incapaciated or not from unitService
            combatTeamManager.UpdateTeamStatus(unitService);

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

