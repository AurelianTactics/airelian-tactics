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
         private List<PlayerUnit> _playerUnits = new();
        
        public List<PlayerUnit> PlayerUnits => _playerUnits;
        
        public void AddUnit(PlayerUnit unit)
        {
            // Implementation logic here
            _playerUnits.Add(unit);
        }
        
        public void RemoveUnit(PlayerUnit unit)
        {
            // Implementation logic here
            _playerUnits.Remove(unit);
        }

        
        void IncrementCTAll(){
            foreach (var unit in _playerUnits) {
                unit.AddCT();
            }
        }

        void AlterHP(PlayerUnit unit, int hp){
            unit.AlterHP(hp);
        }

        void SetEligibleForActiveTurn(){
            foreach (var unit in _playerUnits) {
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
        PlayerUnit GetNextActiveTurnPlayerUnit(){
            PlayerUnit nextUnit = null;

            foreach (var unit in _playerUnits) {
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
                        if (unit.IsEligibleForActiveTurn) {
                            if (unit.UnitId < nextUnit.UnitId) {
                                nextUnit = unit;
                            }
                        }
                    }
                }
                
            }
            
            return nextUnit;
        }

        bool IsAnyUnitMidActiveTurn(){
            foreach (var unit in _playerUnits) {
                if (unit.IsMidActiveTurn) {
                    return true;
                }
            }
            return false;
        }

        // not sure if going to use this or direct PlayerUnit. Not sure what arg will be ifused
        void SetUnitMidActiveTurn(PlayerUnit unit){
            unit.IsMidActiveTurn = true;
        }
    }
} 