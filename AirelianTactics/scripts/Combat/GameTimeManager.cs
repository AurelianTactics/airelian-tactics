using System;
using System.Collections.Generic;
using System.Linq;
using AirelianTactics.Services;

/// <summary>
/// Manages the game time loop and determines what part of the game time loop happens next.
/// Uses a dictionary of lists to handle GameTimeObjects for each phase.
/// </summary>
public class GameTimeManager
{
    private Dictionary<Phases, List<GameTimeObject>> phaseQueues;
    private List<Phases> phaseOrder;
    private UnitService unitService;
    private GameTimeObjectProcessor processor;

    /// <summary>
    /// Constructor that initializes the phase queues and order
    /// </summary>
    /// <param name="unitService">The unit service to check for midturn and activeturn units</param>
    /// <param name="spellService">The spell service for processing spell effects</param>
    /// <param name="statusService">The status service for processing status effects</param>
    /// <param name="board">The board for managing unit positions</param>
    public GameTimeManager(UnitService unitService, SpellService spellService, StatusService statusService, Board board)
    {
        this.unitService = unitService;
        this.processor = new GameTimeObjectProcessor(unitService, spellService, statusService, board);
        InitializePhaseOrder();
        InitializePhaseQueues();
    }

    /// <summary>
    /// Initialize the order of phases based on the game time phases from Phases.cs
    /// </summary>
    private void InitializePhaseOrder()
    {
        phaseOrder = new List<Phases>
        {
            // Game Time phases in order
            Phases.FasterThanFastAction,
            Phases.Reaction,
            Phases.Mime,
            Phases.MidTurn,
            Phases.QuickTurn,
            Phases.ActiveTurn,
            Phases.EndOfActiveTurn,
            Phases.SlowAction
        };
    }

    /// <summary>
    /// Initialize the phase queues dictionary with empty lists for each phase
    /// </summary>
    private void InitializePhaseQueues()
    {
        phaseQueues = new Dictionary<Phases, List<GameTimeObject>>();
        
        foreach (var phase in phaseOrder)
        {
            phaseQueues[phase] = new List<GameTimeObject>();
        }
    }

    /// <summary>
    /// Add a GameTimeObject to the appropriate phase queue
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject to add</param>
    public void AddGameTimeObject(GameTimeObject gameTimeObject)
    {
        if (gameTimeObject.Phase.HasValue && phaseQueues.ContainsKey(gameTimeObject.Phase.Value))
        {
            phaseQueues[gameTimeObject.Phase.Value].Add(gameTimeObject);
        }
        else
        {
            Console.WriteLine($"Warning: Cannot add GameTimeObject with invalid phase: {gameTimeObject.Phase}");
        }
    }

    /// <summary>
    /// Get the next GameTimeObject that should be processed
    /// Goes through each phase in order and checks if the first object meets criteria
    /// </summary>
    /// <returns>The next GameTimeObject to process, or null if none are available</returns>
    public GameTimeObject GetNextGameTimeObject()
    {
        foreach (var phase in phaseOrder)
        {
            GameTimeObject nextObject = null;

            // Special handling for MidTurn and ActiveTurn phases
            if (phase == Phases.MidTurn)
            {
                nextObject = HandleMidTurnPhase();
            }
            else if (phase == Phases.ActiveTurn)
            {
                nextObject = HandleActiveTurnPhase();
            }
            else
            {
                // Regular queue handling for other phases
                nextObject = HandleRegularPhase(phase);
            }

            if (nextObject != null)
            {
                return nextObject;
            }
        }

        return null; // No objects available
    }

    /// <summary>
    /// Handle MidTurn phase - check UnitService for midturn units instead of using queue
    /// </summary>
    /// <returns>GameTimeObject for midturn unit, or null if none available</returns>
    private GameTimeObject HandleMidTurnPhase()
    {
        if (unitService.IsAnyUnitMidActiveTurn())
        {
            var midTurnUnit = unitService.GetNextActiveTurnPlayerUnit();
            if (midTurnUnit != null && midTurnUnit.IsMidActiveTurn)
            {
                return new GameTimeObject(phase: Phases.MidTurn, actorUnitId: midTurnUnit.UnitId);
            }
        }
        return null;
    }

    /// <summary>
    /// Handle ActiveTurn phase - check UnitService for activeturn units instead of using queue
    /// </summary>
    /// <returns>GameTimeObject for activeturn unit, or null if none available</returns>
    private GameTimeObject HandleActiveTurnPhase()
    {
        var activeTurnUnit = unitService.GetNextActiveTurnPlayerUnit();
        if (activeTurnUnit != null && !activeTurnUnit.IsMidActiveTurn)
        {
            return new GameTimeObject(phase: Phases.ActiveTurn, actorUnitId: activeTurnUnit.UnitId);
        }
        return null;
    }

    /// <summary>
    /// Handle regular phases using queue logic
    /// </summary>
    /// <param name="phase">The phase to handle</param>
    /// <returns>GameTimeObject from queue, or null if none meet criteria</returns>
    private GameTimeObject HandleRegularPhase(Phases phase)
    {
        var queue = phaseQueues[phase];
        
        if (queue.Count > 0)
        {
            var firstObject = queue[0];
            
            // Check if first object meets criteria (for now, always return first object)
            // TODO: Add specific criteria checking logic here
            if (MeetsCriteria(firstObject))
            {
                queue.RemoveAt(0); // Pop from beginning of list
                return firstObject;
            }
        }
        
        return null;
    }

    /// <summary>
    /// Check if a GameTimeObject meets the criteria to be processed
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject to check</param>
    /// <returns>True if it meets criteria, false otherwise</returns>
    private bool MeetsCriteria(GameTimeObject gameTimeObject)
    {
        // TODO: Implement specific criteria checking logic
        // For now, always return true (process all objects)
        return true;
    }

    /// <summary>
    /// Get the count of objects in a specific phase queue
    /// </summary>
    /// <param name="phase">The phase to check</param>
    /// <returns>Number of objects in the phase queue</returns>
    public int GetPhaseQueueCount(Phases phase)
    {
        return phaseQueues.ContainsKey(phase) ? phaseQueues[phase].Count : 0;
    }

    /// <summary>
    /// Clear all phase queues
    /// </summary>
    public void ClearAllQueues()
    {
        foreach (var queue in phaseQueues.Values)
        {
            queue.Clear();
        }
    }

    /// <summary>
    /// Clear a specific phase queue
    /// </summary>
    /// <param name="phase">The phase queue to clear</param>
    public void ClearPhaseQueue(Phases phase)
    {
        if (phaseQueues.ContainsKey(phase))
        {
            phaseQueues[phase].Clear();
        }
    }

    /// <summary>
    /// Get all objects in a specific phase queue (for debugging/inspection)
    /// </summary>
    /// <param name="phase">The phase to inspect</param>
    /// <returns>List of GameTimeObjects in the phase queue</returns>
    public List<GameTimeObject> GetPhaseQueue(Phases phase)
    {
        return phaseQueues.ContainsKey(phase) ? new List<GameTimeObject>(phaseQueues[phase]) : new List<GameTimeObject>();
    }

    /// <summary>
    /// Process a specific GameTimeObject directly using the processor
    /// to do: should not process midturn or activeturn
    /// </summary>
    /// <param name="gameTimeObject">The GameTimeObject to process</param>
    /// <returns>True if processed successfully, false otherwise</returns>
    public bool ProcessGameTimeObject(GameTimeObject gameTimeObject)
    {
        if (gameTimeObject == null)
        {
            return false;
        }

        Console.WriteLine($"Processing {processor.GetActionSummary(gameTimeObject)}");
        bool processResult = processor.ProcessGameTimeObject(gameTimeObject);
        
        if (processResult)
        {
            Console.WriteLine($"Successfully processed GameTimeObject {gameTimeObject.GameTimeObjectId}");
        }
        else
        {
            Console.WriteLine($"Failed to process GameTimeObject {gameTimeObject.GameTimeObjectId}");
        }
        
        return processResult;
    }

} 