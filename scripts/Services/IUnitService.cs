using System;
using System.Collections;
using System.Collections.Generic;
using AirelianTactics.Core.Logic;
using UnityEngine;

namespace AirelianTactics.Services
{
    /// <summary>
    /// Interface for managing player units, player unit objects, and related functionality
    /// </summary>
    public interface IUnitService
    {
        // Unit Management
        void ClearLists();
        void AddPlayerUnit(PlayerUnit pu);
        void AddPlayerObject(GameObject po);
        PlayerUnit GetPlayerUnit(int unitId);
        GameObject GetPlayerUnitObject(int unitId);
        PlayerUnitObject GetPlayerUnitObjectComponent(int unitId);
        List<PlayerUnit> GetPlayerUnitList();
        void DisableUnit(int unitId);

        // Team Management
        void SetTeamList(List<PlayerUnit> puList, int teamId);
        void EditTeamLists(PlayerUnit pu, int teamId, int type);
        List<PlayerUnit> GetTeamList(int teamId);
        void ClearTeamLists();
        bool IsOnTeam(int actorId, int targetId);
        bool IsMatch(int actorId, int targetId, Targets targets);
        List<PlayerUnit> GetAllUnitsByTeamId(int teamId, bool sameTeam = true);
        List<PlayerUnit> GetAIList(int actorCharmTeam, int aiType);

        // CT and Turn Management
        void IncrementCTPhase();
        PlayerUnit GetNextActiveTurnPlayerUnit(bool isSetQuickFlagToFalse);
        void EndCombatTurn(CombatTurn turn, bool isDead = false);
        void ProcessCTIncrement();

        // Unit Actions and State
        void SetPlayerObjectAnimation(int unitId, string animation, bool isIdle);
        void KnockbackPlayer(Board board, int unitId, Tile moveTile, SpellName sn = null, PlayerUnit actor = null, int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919);
        void SetUnitTile(Board board, int unitId, Tile t, bool isSetMarker = false, bool isAddCombatLog = false);
        void SetUnitTileSwap(Board board, int unitId, Tile t);
        void SetAbleToFight(int unitId, bool able);
        void SetFacingDirectionMidTurn(int unitId, Directions dir, bool isSend = true);
        void SetFacingDirectionAttack(Board board, PlayerUnit actor, Tile targetTile);
        
        // Unit Stats and Status
        void AddLife(int effect, int unitId);
        void RemoveMPById(int unitId, int mp);
        void AlterUnitStat(int alterStat, int effect, int statType, int unitId, int elementType = 0, SpellName sn = null, PlayerUnit actor = null, int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919);
        void RemoveLife(int effect, int removeStat, int unitId, int elementalType, bool removeAll = false, SpellName sn = null, PlayerUnit actor = null, int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919);
        void AlterReactionFlag(int unitId, bool reactionFlag);
        
        // Mime and Quick functionality
        bool IsMimeOnTeam(int teamId);
        void SetMimeOnTeam();
        List<PlayerUnit> GetMimeList(int actorCharmTeam);
        bool QuickFlagCheckPhase();
        int GetQuickFlagUnitId();
        void SetQuickFlag(int unitId, bool isQuick);

        // Equipment and Abilities
        bool IsAbilityEquipped(int unitId, int abilityId, int abilitySlot);
        int GetWeaponPower(int unitId, bool battleSkill = false, bool twoHandsTypeAllowed = false);
        void EquipMovementAbility(int unitId, int abilityId);
        
        // Game State Checks
        Teams CheckForDefeat();
        int CheckEndGameConditions();
        void CheckForEndCombat(PlayerUnit pu, bool isAbleToFight);

        // Status and Status Effects
        void AddToStatusList(int unitId, int statusId);
        void RemoveFromStatusList(int unitId, int statusId);
        void EndOfTurnTick(int unitId, int type);
        void ToggleJumping(int unitId, bool isJumping);
        
        // Combat Logging and Stats
        bool IsRollSuccess(int rollChance, int rollLow = 1, int rollSpread = 100, int logType = -1919, int logSubType = -1919, SpellName sName = null, PlayerUnit puActor = null, PlayerUnit puTarget = null, int effectValue = -1919, int statusValue = -1919);
        int GetRollResult(int rollChance, int rollLow = 1, int rollSpread = 100, int logType = -1919, int logSubType = -1919, SpellName sName = null, PlayerUnit puActor = null, PlayerUnit puTarget = null, int effectValue = -1919, int statusValue = -1919);
        void AddCombatLogSaveObject(int logType, int logSubType = -1919, CombatTurn cTurn = null, int rollResult = -1919, int rollChance = -1919, int effectValue = -1919, int statusValue = -1919);
        
        // Walk Around Mode
        bool GetWalkAroundMoveAllowed();
        void SetWalkAroundMoveAllowed(bool zBool);
        void WalkAroundCombatStartCheck(PlayerUnit actor, PlayerUnit target, SpellName sn);
        void SetGameMode(int mode);
        int GetGameMode();
    }
} 