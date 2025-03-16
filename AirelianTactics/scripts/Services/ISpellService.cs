using System.Collections.Generic;
using AirelianTactics.Core.Logic;

namespace AirelianTactics.Services
{
    /// <summary>
    /// Interface for managing spells in combat and walk around scenes
    /// </summary>
    public interface ISpellService
    {
        // // Core Spell Management
        // SpellName GetSpellNameByIndex(int index);
        // SpellName GetSpellAttackByWeaponType(int weaponType, int classId);
        // SpellName GetSpellAttackByWeaponId(int weaponId, int classId);
        // List<SpellName> GetSpellNamesByCommandSet(int commandSet, PlayerUnit pu, int mathSkillOnly = 0);
        // List<SpellName> GetSpellNamesByCommandSet(int commandSet);
        // int GetSpellElementalByIndex(int index);
        // void SetSpellLearnedType(int type);

        // // Spell Slow Actions
        // List<SpellSlow> GetSpellSlowList();
        // void SlowActionTickPhase();
        // void ProcessSlowActionTick();
        // SpellSlow GetNextSlowAction();
        // bool ProcessNextSlowAction();
        // void RemoveSpellSlowByObject(SpellSlow ss);
        // void RemoveSpellSlowByUnitId(int unitId);
        // bool RemoveArcherCharge(int unitId);
        // void CreateSpellSlow(CombatTurn turn, bool isWAMode = false);
        // void CreateSpellSlow(SpellSlow ss);
        // bool IsSpellTargetable(WalkAroundActionObject waao);
        // bool IsTargetUnitMap(SpellName sn, int targetId);

        // // Spell Reactions
        // SpellReaction GetNextSpellReaction();
        // void RemoveSpellReactionByObject(SpellReaction sr);
        // void RemoveSpellReactionByUnitId(int unitId);
        // void AddSpellReaction(SpellReaction sr);
        // bool IsSpellReaction();

        // // Mime Queue
        // SpellReaction GetNextMimeQueue();
        // void RemoveMimeQueueByObject(SpellReaction sr);
        // void RemoveMimeQueueByUnit(int unitId);
        // void AddMimeQueue(SpellReaction sr);
        // bool IsMimeQueue();
        // void RemoveSpellMimeByUnitId(int unitId);

        // // Spell Properties
        // void DecrementInventory(SpellName sn, int teamId);
        // bool IsSpellPositive(SpellName sn);
        // void SendReactionDetails(int actorId, int spellIndex, int targetX, int targetY, string displayName);

        // // Spell Learning
        // void AddToSpellLearnedList(int unitId, List<AbilityLearnedListObject> tempList);
        // void AlterSpellLearnedList(int turnOrder, int listOrder);
        // List<AbilityObject> GetSpellNamesToAbilityObject(int commandSetId, bool isCustomClass);

        // // Walk Around Mode
        // void AddWalkAroundSpellSlow(SpellSlow ss, bool isCTRZero = true);
        // bool IsWalkAroundSpellSlow();
    }
} 