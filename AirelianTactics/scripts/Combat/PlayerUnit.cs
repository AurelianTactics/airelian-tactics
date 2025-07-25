public class PlayerUnit {

    // stats can come from Unit or Item. Total is the sum of the two. In future maybe add more components   
    public int StatTotalCT { get; set; }
    public int StatTotalSpeed { get; set; }
    public int StatTotalPA { get; set; }
    public int StatTotalHP { get; set; }
    public int StatTotalMove { get; set; }
    public int StatTotalJump { get; set; }

    /// <summary>
    /// Units unique identifier. Lower number goes first in case of turn order and
    /// other tiebreakers.
    /// </summary>
    public int UnitId { get; set; }

    public int TeamId { get; set; }
    
    // if all units on a team are incapacitated, the team is defeated
    public bool IsIncapacitated { get; set; }

    public bool IsMidActiveTurn { get; set; }
    
    public PlayerUnit(int ct, int speed, int pa, int hp, int move, int jump, int unitId, int teamId) {
        this.StatTotalCT = ct;
        this.StatTotalSpeed = speed;
        this.StatTotalPA = pa;
        this.StatTotalHP = hp;
        this.StatTotalMove = move;
        this.StatTotalJump = jump;

        this.UnitId = unitId;
        this.IsIncapacitated = false;
        this.IsMidActiveTurn = false;
        this.TeamId = teamId;
    }

    public bool IsEligibleForActiveTurn() {
        bool isEligible = !this.IsIncapacitated && this.StatTotalCT >= 100;

        return isEligible;
    }

    public void AddCT()
    {
        //doing simple version for now until statusManager is implemented
        if( this.StatTotalCT < 100)
        {
            this.StatTotalCT += this.StatTotalSpeed;

            // to do: calculate CT more correctly
            // if( !StatusManager.Instance.IsCTHalted(this.TurnOrder))
            // {
            //     int z1 = this.StatTotalSpeed;
            //     if( IsAbilityEquipped(NameAll.SUPPORT_NATURAL_HIGH, NameAll.ABILITY_SLOT_SUPPORT) && StatusManager.Instance.IsPositiveStatus(this.TurnOrder))
            //     {
            //         z1 += 1;
            //     }
            //     else if ( IsAbilityEquipped(NameAll.SUPPORT_UNNATURAL_HIGH, NameAll.ABILITY_SLOT_SUPPORT) && StatusManager.Instance.IsNegativeStatus(this.TurnOrder))
            //     {
            //         z1 += 2;
            //     }
            //     z1 = StatusManager.Instance.ModifySpeed(this.TurnOrder, z1);
            //     this.CT += z1;
            // }  
        }
    }

    public void AlterHP(int hp) {
        this.StatTotalHP += hp;

        if (this.StatTotalHP <= 0) {
            this.IsIncapacitated = true;
            this.StatTotalHP = 0;
        }
    }

    public void SetEligibleForActiveTurn() {
        // This method was missing but is called in UnitService
        // Implementation depends on what this method should do
        // For now, setting a basic implementation
        if (this.StatTotalCT >= 100 && !this.IsIncapacitated) {
            // Unit is eligible for an active turn
            // Any additional logic can be added here
        }
    }

    public void EndTurn(){
        this.IsMidActiveTurn = false;
        // to do: use act/wait to potentially change CT
        this.StatTotalCT -= 100;
        if(this.StatTotalCT < 0){
            this.StatTotalCT = 0;
        }
    }
}
