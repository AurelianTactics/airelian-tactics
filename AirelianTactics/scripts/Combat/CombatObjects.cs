using System;
using System.Collections.Generic;
using AirelianTactics.Services;

namespace AirelianTactics.Combat
{
    /// <summary>
    /// CombatObject is a class that contains all the data for a combat session.
    /// Used in CombatEngine to process the game loop.  
    /// to do: mime queue, quick queue?, counter queue?
    /// </summary>
    public class CombatObject
{

    //private readonly IUnitService _unitService;
    
    public Phases CurrentPhase { get; set; }
    public int ActiveTurnUnitId { get; set; }
    public bool isReactionFlag { get; set; }
    public bool isMimeFlag { get; set; }
    public bool isQuickFlag { get; set; }


    public List<int> mimeQueue { get; set; }

    /// <summary>
    /// Get the potential reaction, mime, or quick phase
    /// If none exist then return the current phase, which will mean the next
    /// phase can be processed according to the current phase processing logic
    /// IE for slowactions, the next phase is CTIncrement
    /// </summary>
    public Phases getPotentialReactionMimeQuickPhase(Phases currentPhase)
    {

        // this.isReactionFlag = GetAnyPlayerUnitReactionFlag(PlayerManager.Instance);

        // if (this.isReactionFlag)
        // {
        //     return Phases.Reaction;
        // }

        // to do mime stuff and quicks tuff
        // else if (isMimeFlag)
        // {
        //     // to do: process the mime queue to see if is one
        //     newPhase = Phases.Mime;
        // }
        // else if (isQuickFlag && isMidActiveTurn)
        // {
        //     newPhase = Phases.Quick;
        // }
        
        return currentPhase;
        
    }

    /// <summary>
    /// See if there is a reaction. Stored in a player unit. in player manager.
    /// If there is a reaction, set the reaction flag and return true.
    /// If there is no reaction, return false.
    /// the existence of one reaction means that the reaction phase needs to be processed.
    /// </summary>
    public bool GetAnyPlayerUnitReactionFlag(IUnitService _unitService){
        // to do: implement this
        return false;
        // bool isReaction = _unitService.GetReactionFlag();
        
        // return isReaction;
    }

    // Phases CheckForFlag(Phases currentPhase, int midActiveTurn = 0)
	// {
	// 	//Debug.Log("checking for flag");
	// 	if (SpellManager.Instance.IsSpellReaction())
	// 	{
	// 		//Debug.Log("checking for flag is reaction");
	// 		return Phases.Reaction;
	// 	}
	// 	else if (SpellManager.Instance.GetNextMimeQueue() != null)
	// 	{
	// 		return Phases.Mime;
	// 	}
	// 	else if (PlayerManager.Instance.QuickFlagCheckPhase() && midActiveTurn == 0)
	// 	{
    // //   remember that logic here is different for slow phase vs. midActiveTurn phase. if a unit is mid active turn, quick doesn't go next.
	// 		//mid turn indicator is for ActiveTurns, can't jump from a midActiveTurn active turn into a Quick turn  
	// 		return Phases.Quick;

	// 	}
	// 	return currentPhase;
	// }
}
}