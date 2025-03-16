using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AirelianTactics.Core.Logic;


namespace AirelianTactics.Services
{
    /// <summary>
    /// Implementation of ISpellService that manages spells in combat and walk around scenes
    /// </summary>
    public class SpellService : ISpellService
    {
        // // Private fields (replace static fields from SpellManager)
        // private List<SpellSlow> _spellSlowList;
        // private List<SpellReaction> _spellReactionList;
        // private List<SpellReaction> _spellMimeList;
        // private List<SpellCommandSet> _spellCommandSetList;
        // private List<SpellLearnedSet> _spellLearnedList;

        // // Inventory state
        // private int _greenElixir;
        // private int _redElixir;
        // private int _greenAsura;
        // private int _greenBizen;
        // private int _greenChiri;
        // private int _greenHeaven;
        // private int _greenKiku;
        // private int _greenKiyo;
        // private int _greenKoutetsu;
        // private int _greenMasamune;
        // private int _greenMuramasa;
        // private int _greenMurasame;
        // private int _redAsura;
        // private int _redBizen;
        // private int _redChiri;
        // private int _redHeaven;
        // private int _redKiku;
        // private int _redKiyo;
        // private int _redKoutetsu;
        // private int _redMasamune;
        // private int _redMuramasa;
        // private int _redMurasame;

        // private int _spellLearnedType;

        // // Dependencies
        // private readonly IStatusService _statusService;

        // public SpellService(IStatusService statusService)
        // {
        //     _statusService = statusService;

        //     // Initialize collections and fields
        //     _spellSlowList = new List<SpellSlow>();
        //     _spellReactionList = new List<SpellReaction>();
        //     _spellMimeList = new List<SpellReaction>();
        //     _spellCommandSetList = new List<SpellCommandSet>();
        //     _spellLearnedList = new List<SpellLearnedSet>();

        //     _greenElixir = 1;
        //     _redElixir = 1;
        //     _greenAsura = 1;
        //     _greenBizen = 1;
        //     _greenChiri = 1;
        //     _greenHeaven = 1;
        //     _greenKiku = 1;
        //     _greenKiyo = 1;
        //     _greenKoutetsu = 1;
        //     _greenMasamune = 1;
        //     _greenMuramasa = 1;
        //     _greenMurasame = 1;
        //     _redAsura = 1;
        //     _redBizen = 1;
        //     _redChiri = 1;
        //     _redHeaven = 1;
        //     _redKiku = 1;
        //     _redKiyo = 1;
        //     _redKoutetsu = 1;
        //     _redMasamune = 1;
        //     _redMuramasa = 1;
        //     _redMurasame = 1;

        //     _spellLearnedType = 0; // NameAll.SPELL_LEARNED_TYPE_NONE

        //     // Load spell command set list from data file
        //     string filePath = Application.dataPath + "/Custom/CSV/SpellNameDataCSV.csv";
        //     _spellCommandSetList = GenerateSpellCommandSetList(filePath);
        // }

        // #region Core Spell Management

        // public SpellName GetSpellNameByIndex(int index)
        // {
        //     // Implementation would match SpellManager.GetSpellNameByIndex()
        //     if (index < 10000)
        //     {
        //         SpellNameData snd = Resources.Load<SpellNameData>("SpellNames/sn_" + index);
        //         if (snd == null)
        //         {
        //             Debug.Log("ERROR snd is null...");
        //             return null;
        //         }
        //         SpellName sn = new SpellName(snd);
        //         Resources.UnloadAsset(snd);
        //         return sn;
        //     }
        //     else // custom created spellNames
        //     {
        //         string fileName = Application.dataPath + "/Custom/Spells/" + index + "_sn.dat";
        //         if (File.Exists(fileName))
        //         {
        //             SpellName sn = Serializer.Load<SpellName>(fileName);
        //             return sn;
        //         }
        //         else
        //         {
        //             Debug.Log("Error, spellName not found");
        //             return null;
        //         }
        //     }
        // }

        // public SpellName GetSpellAttackByWeaponType(int weaponType, int classId)
        // {
        //     // Implementation would match SpellManager.GetSpellAttackByWeaponType()
        //     int index = GetSpellIndexByWeaponType(weaponType, classId);
        //     return GetSpellNameByIndex(index);
        // }

        // public SpellName GetSpellAttackByWeaponId(int weaponId, int classId)
        // {
        //     // Implementation would match SpellManager.GetSpellAttackByWeaponId()
        //     return null; // Placeholder
        // }

        // public List<SpellName> GetSpellNamesByCommandSet(int commandSet, PlayerUnit pu, int mathSkillOnly = 0)
        // {
        //     // Implementation would match SpellManager.GetSpellNamesByCommandSet() with PlayerUnit parameter
        //     return new List<SpellName>(); // Placeholder
        // }

        // public List<SpellName> GetSpellNamesByCommandSet(int commandSet)
        // {
        //     // Implementation would match SpellManager.GetSpellNamesByCommandSet() without PlayerUnit parameter
        //     List<SpellName> retList = new List<SpellName>();
        //     IEnumerable<SpellCommandSet> enumerable = _spellCommandSetList.Where(scs => scs.CommandSet == commandSet);
        //     List<SpellCommandSet> asList = enumerable.ToList();
        //     for (int i = 0; i < asList.Count; i++)
        //     {
        //         retList.Add(GetSpellNameByIndex(asList[i].SpellIndex));
        //     }
        //     return retList;
        // }

        // public int GetSpellElementalByIndex(int index)
        // {
        //     // Implementation would match SpellManager.GetSpellElementalByIndex()
        //     return GetSpellNameByIndex(index).ElementType;
        // }

        // public void SetSpellLearnedType(int type)
        // {
        //     // Implementation would match SpellManager.SetSpellLearnedType()
        //     _spellLearnedType = type;
        // }

        // private int GetSpellIndexByWeaponType(int weaponType, int classId)
        // {
        //     // Implementation would match NameAll.GetSpellIndexByWeaponType()
        //     return 0; // Placeholder
        // }

        // #endregion

        // #region Spell Slow Actions

        // public List<SpellSlow> GetSpellSlowList()
        // {
        //     // Implementation would match SpellManager.GetSpellSlowList()
        //     return _spellSlowList;
        // }

        // public void SlowActionTickPhase()
        // {
        //     // Implementation would match SpellManager.SlowActionTickPhase()
        //     foreach (SpellSlow s in _spellSlowList)
        //     {
        //         s.DecrementCtrOnly();
        //     }
        // }

        // public void ProcessSlowActionTick()
        // {
        //     // Implementation for CombatEngine.ProcessSlowActionTick()
        //     SlowActionTickPhase();
        // }

        // public SpellSlow GetNextSlowAction()
        // {
        //     // Implementation would match SpellManager.GetNextSlowAction()
        //     int z1 = 9999;
        //     int zUniqueId = 999999;
        //     SpellSlow s1 = null;
        //     foreach (SpellSlow ss in _spellSlowList)
        //     {
        //         if (ss.CTR == 0)
        //         {
        //             if (ss.UnitId <= z1)
        //             {
        //                 if (ss.UnitId == z1)
        //                 {
        //                     if (ss.UniqueId < zUniqueId)
        //                     {
        //                         z1 = ss.UnitId;
        //                         zUniqueId = ss.UniqueId;
        //                         s1 = ss;
        //                     }
        //                 }
        //                 else
        //                 {
        //                     z1 = ss.UnitId;
        //                     zUniqueId = ss.UniqueId;
        //                     s1 = ss;
        //                 }
        //             }
        //         }
        //     }
        //     return s1;
        // }

        // public bool ProcessNextSlowAction()
        // {
        //     // Implementation for CombatEngine.ProcessNextSlowAction()
        //     SpellSlow ss = GetNextSlowAction();
        //     if (ss != null)
        //     {
        //         // Process the slow action
        //         RemoveSpellSlowByObject(ss);
        //         return true;
        //     }
        //     return false;
        // }

        // public void RemoveSpellSlowByObject(SpellSlow ss)
        // {
        //     // Implementation would match SpellManager.RemoveSpellSlowByObject()
        //     _spellSlowList.Remove(ss);
        // }

        // public void RemoveSpellSlowByUnitId(int unitId)
        // {
        //     // Implementation would match SpellManager.RemoveSpellSlowByUnitId()
        //     foreach (SpellSlow s in _spellSlowList.ToList())
        //     {
        //         if (s.UnitId == unitId)
        //         {
        //             _spellSlowList.Remove(s);
        //         }
        //     }
        // }

        // public bool RemoveArcherCharge(int unitId)
        // {
        //     // Implementation would match SpellManager.RemoveArcherCharge()
        //     bool isRemoved = false;
        //     foreach (SpellSlow s in _spellSlowList.ToList())
        //     {
        //         if (s.UnitId == unitId)
        //         {
        //             SpellName sn = GetSpellNameByIndex(s.SpellIndex);
        //             if (sn.CommandSet == 37) // NameAll.COMMAND_SET_CHARGE
        //             {
        //                 _spellSlowList.Remove(s);
        //                 return true;
        //             }
        //         }
        //     }
        //     return isRemoved;
        // }

        // public void CreateSpellSlow(CombatTurn turn, bool isWAMode = false)
        // {
        //     // Implementation would match SpellManager.CreateSpellSlow() with CombatTurn parameter
        //     SpellSlow ss = new SpellSlow(turn);
        //     if (isWAMode)
        //         AddWalkAroundSpellSlow(ss);
        //     else
        //         AddSpellSlow(ss);

        //     // Add appropriate status based on spell type
        //     if (turn.spellName.CommandSet == 18 || turn.spellName.CommandSet == 19 // SING, DANCE
        //         || turn.spellName.CommandSet == 45 && (turn.spellName.CTR % 10) == 1) // ARTS & condition
        //     {
        //         _statusService.AddStatusAndOverrideOthers(0, turn.actor.TurnOrder, 13); // STATUS_ID_PERFORMING
        //     }
        //     else if (turn.spellName.CommandSet == 17) // JUMP
        //     {
        //         _statusService.AddStatusAndOverrideOthers(0, turn.actor.TurnOrder, 17); // STATUS_ID_JUMPING
        //     }
        //     else
        //     {
        //         _statusService.AddStatusAndOverrideOthers(0, turn.actor.TurnOrder, 8); // STATUS_ID_CHARGING
        //     }
        // }

        // public void CreateSpellSlow(SpellSlow ss)
        // {
        //     // Implementation would match SpellManager.CreateSpellSlow() with SpellSlow parameter
        //     SpellSlow ss2 = new SpellSlow(ss);
        //     AddSpellSlow(ss2);
        // }

        // private void AddSpellSlow(SpellSlow ss)
        // {
        //     // Implementation would match SpellManager.AddSpellSlow()
        //     _spellSlowList.Add(ss);
        // }

        // public bool IsSpellTargetable(WalkAroundActionObject waao)
        // {
        //     // Implementation would match SpellManager.IsSpellTargetable()
        //     return true; // Placeholder
        // }

        // public bool IsTargetUnitMap(SpellName sn, int targetId)
        // {
        //     // Implementation would match SpellManager.IsTargetUnitMap()
        //     return false; // Placeholder
        // }

        // #endregion

        // #region Spell Reactions

        // public SpellReaction GetNextSpellReaction()
        // {
        //     // Implementation would match SpellManager.GetNextSpellReaction()
        //     int z1 = 9999;
        //     SpellReaction s1 = null;
        //     foreach (SpellReaction sr in _spellReactionList)
        //     {
        //         if (sr.ActorId < z1)
        //         {
        //             z1 = sr.ActorId;
        //             s1 = sr;
        //         }
        //     }
        //     return s1;
        // }

        // public void RemoveSpellReactionByObject(SpellReaction sr)
        // {
        //     // Implementation would match SpellManager.RemoveSpellReactionByObject()
        //     RemoveSpellReaction(sr);
        // }

        // public void RemoveSpellReactionByUnitId(int unitId)
        // {
        //     // Implementation would match SpellManager.RemoveSpellReactionByUnitId()
        //     foreach (SpellReaction sr in _spellReactionList.ToList())
        //     {
        //         if (sr.ActorId == unitId)
        //         {
        //             RemoveSpellReaction(sr);
        //         }
        //     }
        // }

        // public void AddSpellReaction(SpellReaction sr)
        // {
        //     // Implementation would match SpellManager.AddSpellReaction()
        //     _spellReactionList.Add(sr);
        // }

        // private void RemoveSpellReaction(SpellReaction sr)
        // {
        //     // Implementation would match SpellManager.RemoveSpellReaction()
        //     _spellReactionList.Remove(sr);
        // }

        // public bool IsSpellReaction()
        // {
        //     // Implementation would match SpellManager.IsSpellReaction()
        //     return _spellReactionList.Count > 0;
        // }

        // #endregion

        // #region Mime Queue

        // public SpellReaction GetNextMimeQueue()
        // {
        //     // Implementation would match SpellManager.GetNextMimeQueue()
        //     int z1 = 9999;
        //     SpellReaction s1 = null;
        //     foreach (SpellReaction sr in _spellMimeList)
        //     {
        //         if (sr.ActorId < z1)
        //         {
        //             z1 = sr.ActorId;
        //             s1 = sr;
        //         }
        //     }
        //     return s1;
        // }

        // public void RemoveMimeQueueByObject(SpellReaction sr)
        // {
        //     // Implementation would match SpellManager.RemoveMimeQueueByObject()
        //     RemoveMimeQueue(sr);
        // }

        // public void RemoveMimeQueueByUnit(int unitId)
        // {
        //     // Implementation would match SpellManager.RemoveMimeQueueByUnit()
        //     foreach (SpellReaction sr in _spellMimeList.ToList())
        //     {
        //         if (sr.ActorId == unitId)
        //         {
        //             RemoveMimeQueue(sr);
        //         }
        //     }
        // }

        // public void AddMimeQueue(SpellReaction sr)
        // {
        //     // Implementation would match SpellManager.AddMimeQueue()
        //     _spellMimeList.Add(sr);
        // }

        // private void RemoveMimeQueue(SpellReaction sr)
        // {
        //     // Implementation would match SpellManager.RemoveMimeQueue()
        //     _spellMimeList.Remove(sr);
        // }

        // public bool IsMimeQueue()
        // {
        //     // Implementation would match SpellManager.IsMimeQueue()
        //     return _spellMimeList.Count > 0;
        // }

        // public void RemoveSpellMimeByUnitId(int unitId)
        // {
        //     // Implementation would match SpellManager.RemoveSpellMimeByUnitId()
        //     foreach (SpellReaction sr in _spellMimeList.ToList())
        //     {
        //         if (sr.ActorId == unitId)
        //             _spellMimeList.Remove(sr);
        //     }
        // }

        // #endregion

        // #region Spell Properties

        // public void DecrementInventory(SpellName sn, int teamId)
        // {
        //     // Implementation would match SpellManager.DecrementInventory()
        //     if (sn.CommandSet == 25) // NameAll.COMMAND_SET_DRAW_OUT
        //     {
        //         DecrementDrawOut(sn, teamId);
        //     }
        //     else if (sn.SpellId == 225) // NameAll.SPELL_INDEX_ELIXIR
        //     {
        //         if (teamId == 1) // NameAll.TEAM_ID_GREEN
        //         {
        //             _greenElixir -= 1;
        //         }
        //         else
        //         {
        //             _redElixir -= 1;
        //         }
        //     }
        // }

        // private void DecrementDrawOut(SpellName sn, int teamId)
        // {
        //     // Implementation would match the logic in SpellManager.DecrementInventory()
        //     if (teamId == 1) // Green team
        //     {
        //         if (sn.SpellId == 130) // NameAll.SPELL_INDEX_ASURA
        //             _greenAsura -= 1;
        //         else if (sn.SpellId == 131)
        //             _greenBizen -= 1;
        //         // And so on for other items
        //     }
        //     else // Red team
        //     {
        //         if (sn.SpellId == 130) // NameAll.SPELL_INDEX_ASURA
        //             _redAsura -= 1;
        //         else if (sn.SpellId == 131)
        //             _redBizen -= 1;
        //         // And so on for other items
        //     }
        // }

        // public bool IsSpellPositive(SpellName sn)
        // {
        //     // Implementation would match SpellManager.IsSpellPositive()
        //     return sn.RemoveStat == 1; // NameAll.REMOVE_STAT_HEAL
        // }

        // public void SendReactionDetails(int actorId, int spellIndex, int targetX, int targetY, string displayName)
        // {
        //     // Implementation would match SpellManager.SendReactionDetails()
        //     // This would typically send the details to UI or other systems
        // }

        // #endregion

        // #region Spell Learning

        // public void AddToSpellLearnedList(int unitId, List<AbilityLearnedListObject> tempList)
        // {
        //     // Implementation would match SpellManager.AddToSpellLearnedList()
        //     foreach (AbilityLearnedListObject a in tempList)
        //     {
        //         _spellLearnedList.Add(new SpellLearnedSet(unitId + 100, a.ClassId, a.AbilityId));
        //     }
        // }

        // public void AlterSpellLearnedList(int turnOrder, int listOrder)
        // {
        //     // Implementation would match SpellManager.AlterSpellLearnedList()
        //     _spellLearnedList.Where(s => s.TurnOrder == listOrder + 100).ToList().ForEach(s2 => s2.TurnOrder = turnOrder);
        // }

        // public List<AbilityObject> GetSpellNamesToAbilityObject(int commandSetId, bool isCustomClass)
        // {
        //     // Implementation would match SpellManager.GetSpellNamesToAbilityObject()
        //     List<AbilityObject> retValue = new List<AbilityObject>();

        //     if (isCustomClass)
        //         GenerateSpellNamesByCustomCommandSet(commandSetId);

        //     var tempList = _spellCommandSetList.Where(s => s.CommandSet == commandSetId);
        //     foreach (SpellCommandSet scs in tempList)
        //     {
        //         SpellName sn = GetSpellNameByIndex(scs.SpellIndex);
        //         if (sn != null)
        //             retValue.Add(new AbilityObject(sn));
        //     }
        //     return retValue;
        // }

        // #endregion

        // #region Walk Around Mode

        // public void AddWalkAroundSpellSlow(SpellSlow ss, bool isCTRZero = true)
        // {
        //     // Implementation would match SpellManager.AddWalkAroundSpellSlow()
        //     if (isCTRZero)
        //         ss.CTR = 0;
        //     AddSpellSlow(ss);
        // }

        // public bool IsWalkAroundSpellSlow()
        // {
        //     // Implementation would match SpellManager.isWalkAroundSpellSlow()
        //     return _spellSlowList.Any();
        // }

        // #endregion

        // #region Private Helper Methods

        // private List<SpellCommandSet> GenerateSpellCommandSetList(string filePath)
        // {
        //     // Implementation would match SpellManager.GenerateSpellCommandSetList()
        //     string[] readText = File.ReadAllLines(filePath);
        //     var retValue = new List<SpellCommandSet>();
        //     for (int i = 1; i < readText.Length; ++i)
        //     {
        //         var line = readText[i];
        //         var values = line.Split(',');
        //         SpellCommandSet sc = new SpellCommandSet(Int32.Parse(values[0]), Int32.Parse(values[4]));
        //         retValue.Add(sc);
        //     }
        //     return retValue;
        // }

        // private void GenerateSpellNamesByCustomCommandSet(int commandSet)
        // {
        //     // Implementation would match SpellManager.GenerateSpellNamesByCustomCommandSet()
        //     IEnumerable<SpellCommandSet> enumerable = _spellCommandSetList.Where(scs => scs.CommandSet == commandSet);
        //     if (enumerable.Count() > 0) // already been loaded
        //         return;
        //     else
        //     {
        //         var snList = new List<SpellName>(); // CalcCode.LoadCustomSpellNameList();
        //         foreach (SpellName sn in snList)
        //         {
        //             if (sn.CommandSet == commandSet)
        //             {
        //                 SpellCommandSet scs = new SpellCommandSet(sn.Index, commandSet);
        //                 _spellCommandSetList.Add(scs);
        //             }
        //         }
        //     }
        // }

        // private void RemoveDrawOut(List<SpellName> snList, int number, int spellIndex)
        // {
        //     // Implementation would match SpellManager.RemoveDrawOut()
        //     if (number <= 0)
        //     {
        //         snList.Remove(GetSpellNameByIndex(spellIndex));
        //     }
        // }

        // #endregion
    }
} 