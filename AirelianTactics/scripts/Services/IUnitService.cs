using System;
using System.Collections;
using System.Collections.Generic;

namespace AirelianTactics.Services
{
    /// <summary>
    /// Interface for managing player units, player unit objects, and related functionality
    /// </summary>
    public interface IUnitService
    {
        // Unit Management
        Dictionary<int, PlayerUnit> unitDict { get; }

        int nextUnitId { get; }

        void AddUnit(PlayerUnit unit);
        
        void RemoveUnit(PlayerUnit unit);
        
        void IncrementCTAll();

        void AlterHP(PlayerUnit unit, int hp);

        void SetEligibleForActiveTurn();

        /// <summary>
        /// Gets the Mid Active Turn Unit that should go next or
        /// the first eligible unit if no unit is mid active turn
        /// or null if no units are eligible
        /// May want a list version of this
        /// </summary>
        /// <returns></returns>
        PlayerUnit GetNextActiveTurnPlayerUnit();

        bool IsAnyUnitMidActiveTurn();

        // not sure if going to use this or direct PlayerUnit. Not sure what arg will be if used
        void SetUnitMidActiveTurn(PlayerUnit unit);
    }

} 