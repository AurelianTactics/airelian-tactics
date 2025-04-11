public class PlayerUnit {

    // stats can come from Unit or Item. Total is the sum of the two. In future maybe add more components   
    private int StatTotalCT { get; set; }
    private int StatTotalSpeed { get; set; }
    private int StatTotalPA { get; set; }
    private int StatTotalHP { get; set; }
    private int StatTotalMove { get; set; }
    private int StatTotalJump { get; set; }

    /// <summary>
    /// Units unique identifier. Lower number goes first in case of turn order and
    /// other tiebreakers.
    /// </summary>
    private int UnitId { get; set; }

    private int TeamId { get; set; }
    
    // if all units on a team are incapacitated, the team is defeated
    private bool IsIncapacitated { get; set; }

    private bool IsMidActiveTurn { get; set; }
    
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
}
