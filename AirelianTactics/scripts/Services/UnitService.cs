using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AirelianTactics.Core.Logic;


namespace AirelianTactics.Services
{
    /// <summary>
    /// Implementation of IUnitService that manages player units, player unit objects, and related functionality
    /// </summary>
    public class UnitService : IUnitService
    {
        

        private Dictionary<int, PlayerUnit> _unitDict = new Dictionary<int, PlayerUnit>();

        private int _nextUnitId = 0;

        public Dictionary<int, PlayerUnit> unitDict => _unitDict;

        public void AddUnit(PlayerUnit unit)
        {
            _unitDict.Add(_nextUnitId, unit);
            _nextUnitId++;
        }
        
        public void RemoveUnit(PlayerUnit unit)
        {
            _unitDict.Remove(unit.UnitId);
        }

        
        public void IncrementCTAll()
        {
            foreach (var unit in _unitDict.Values) {
                unit.AddCT();
            }
        }

        public void AlterHP(PlayerUnit unit, int hp)
        {
            unit.AlterHP(hp);
        }

        public void SetEligibleForActiveTurn()
        {
            foreach (var unit in _unitDict.Values) {
                unit.SetEligibleForActiveTurn();
            }
        }

        /// <summary>
        /// Gets the Mid Active Turn Unit that should go next or
        /// the first eligible unit if no unit is mid active turn
        /// or null if no units are eligible
        /// May want a list version of this
        /// </summary>
        /// <returns></returns>
        public PlayerUnit GetNextActiveTurnPlayerUnit()
        {
            PlayerUnit nextUnit = null;

            foreach (var unit in _unitDict.Values) {
                // mid active turn units are prioritized
                if (unit.IsMidActiveTurn) {
                    if (nextUnit == null) {
                        nextUnit = unit;
                    }
                    else {
                        // unit id is the tiebreaker
                        if (unit.UnitId < nextUnit.UnitId) {
                            nextUnit = unit;
                        }
                    }
                }
                else {
                    if( nextUnit != null && nextUnit.IsMidActiveTurn == true) {
                        // mid active turn units are prioritized
                        continue;
                    }
                    else {
                        if (unit.IsEligibleForActiveTurn()) {
                            if (nextUnit == null || unit.UnitId < nextUnit.UnitId) {
                                nextUnit = unit;
                            }
                        }
                    }
                }
                
            }
            
            return nextUnit;
        }

        public bool IsAnyUnitMidActiveTurn()
        {
            foreach (var unit in _unitDict.Values) {
                if (unit.IsMidActiveTurn) {
                    return true;
                }
            }
            return false;
        }

        // not sure if going to use this or direct PlayerUnit. Not sure what arg will be ifused
        public void SetUnitMidActiveTurn(PlayerUnit unit)
        {
            unit.IsMidActiveTurn = true;
        }

        public bool IsTeamDefeated(int teamId){
            foreach (var unit in _unitDict.Values) {
                if (unit.TeamId == teamId && !unit.IsIncapacitated) {
                    return false;
                }
            }
            return true;
        }
    }
} 