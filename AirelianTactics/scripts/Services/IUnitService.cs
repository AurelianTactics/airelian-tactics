using System;
using System.Collections;
using System.Collections.Generic;
using AirelianTactics.Core.Logic;


namespace AirelianTactics.Services
{
    /// <summary>
    /// Interface for managing player units, player unit objects, and related functionality
    /// </summary>
    public interface IUnitService
    {
        // Unit Management
        List<PlayerUnit> PlayerUnits { get; }
        void AddUnit(PlayerUnit unit);
        void RemoveUnit(PlayerUnit unit);
        
    }
} 