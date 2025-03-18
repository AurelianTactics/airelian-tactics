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
    }
} 