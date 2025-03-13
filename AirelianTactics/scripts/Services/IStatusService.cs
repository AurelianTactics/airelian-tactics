using System.Collections.Generic;
using AirelianTactics.Core.Logic;

namespace AirelianTactics.Services
{
    /// <summary>
    /// Interface for managing statuses on PlayerUnits
    /// </summary>
    public interface IStatusService
    {
        // Core status management
        void ClearLists();
        void StatusCheckPhase();
        void ProcessStatusEffects(PlayerManager playerManager);
        bool CheckStatusAtBeginningOfTurn(int unitId);
        void CheckStatusAtEndOfTurn(int unitId);
        
        // Status Adding/Removing
        void RemoveStatusTickByUnit(int unitId, int statusId, bool isBeingCalledFromPlayerUnit = false);
        void RemoveStatusLastingByUnit(int unitId, int itemId);
        void AddStatusLastingById(int unitId, int itemId, int slot);
        void AddStatusLastingByStatusObject(StatusObject so);
        void AddStatusLastingByString(int unitId, int statusId);
        void GenerateAllStatusLasting();
        void RemoveStatusLastingByUnitAndString(int unitId, int statusId);
        void AddStatusAndOverrideOthers(int effect, int unitId, int statusId);
        void RemoveStatus(int unitId, int statusId, bool isTickList = true);
        void RemoveStatusNegative(int unitId, int numberToRemove, bool isTickList = true);
        void RemoveStatusList(int unitId, List<int> statusIdList);
        void RemoveStatusListArray(int unitId, int[] statusIdArray);
        void RemoveStatusItems(int unitId, int itemId, int slot);
        
        // Status Modifiers and Checks
        int ModifySpeed(int unitId, int tempSpeed);
        long GetModFaith(int unitId, int unitFaith);
        bool IfStatusByUnitAndId(int unitId, int statusId, bool isTickType = true);
        bool IfNoEvade(int unitId, int whichArray = 1);
        bool IfNoHit(int unitId);
        bool IfStatusCuredBySpell(int statusId, SpellName sn);
        bool IfSpellCuresAnyStatus(SpellName sn, int unitId);
        bool IsCTHalted(int unitId);
        bool IsTurnActable(int unitId);
        bool IsStatusBlockByUnit(int unitId, int statusId);
        bool IsUnitStrengthenByElement(PlayerUnit unit, int element);
        bool IsUnitWeakByElement(int unitId, int element);
        bool IsUnitHalfByElement(PlayerUnit unit, int element);
        bool IsUnitAbsorbByElement(int unitId, int element);
        bool CheckIfReflect(int unitId);
        bool IsUndead(int unitId);
        bool CheckIfFloat(int unitId);
        bool IsAbleToReact(int unitId);
        bool IsAbleToSecondSwing(int unitId);
        bool IsAIControlledStatus(int unitId);
        bool UnitCantResolveSlowSpell(int unitId);
        bool UnitCantResolveMovement(int unitId);
        
        // Golem Management
        bool IsGolem(int targetTeamId);
        int DecrementGolem(int effect, int targetTeamId, int targetId);
        
        // Status Utility
        bool IfStatusLastingBlocksStatus(int unitId, int statusId);
        void AddDead(int unitId, SpellName sn = null, PlayerUnit actor = null, int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919);
        void AddLife(int effect, int unitId);
        int GetActiveTurnStatus(int actorId);
        void GetUnitStatusList(int unitId, List<string> statusList);
        void AddPUStatusLasting(int unitId, int abilityId, int abilitySlot);
        
        // Special Status Checks
        bool IsSadist(int unitId);
        bool IsShowMust(int targetId, int statusId);
        bool IsContinueAbility(PlayerUnit pu);
        bool IsPositiveStatus(int unitId);
        bool IsNegativeStatus(int unitId);
        bool IsNegativeStatusId(int statusId);
        bool IsUnitPassable(int unitId);
        
        // Death Mode
        int GetDeathMode();
        void SetDeathMode(int dMode);
        
        // Charge/Jump Status
        void CheckChargingJumping(int actorId, SpellName sn);
    }
} 