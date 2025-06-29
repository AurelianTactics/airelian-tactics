public enum Phases
{
    #region World Time

    /// <summary>
    /// Each status that is active and has a counter has a tick decremented from it. Statuses fall off and end at 0.
    /// </summary>
    StatusTick = 0,

    /// <summary>
    /// Each SlowAction in the queue has a tick decremented from it. When a SlowAction reaches 0, it resolves in SlowAction
    /// </summary>
    SlowActionTick,

    /// <summary>
    /// Slow actions that are ready to resolve
    /// </summary>
    SlowAction,

    /// <summary>
    /// Each Unit
    /// </summary>
    CTIncrement,

    /// <summary>
    /// Increment the tick variable by 1
    /// </summary>
    TickIncrement,

    #endregion

    #region Game Time

    // phases that are related to the order and resolution of game time related variables

    /// <summary>
    /// Faster than fast
    /// resolve these first
    /// </summary>
    FasterThanFastAction,

    /// <summary>
    /// reacts to many actions
    /// </summary>
    Reaction,

    /// <summary>
    /// occurs after slowaction or activeturn
    /// </summary>
    Mime,

    /// <summary>
    /// return to a turn that was started but not completed
    /// </summary>
    MidTurn,

    /// <summary>
    /// Quick flag causes unit to jump into next ActiveTurn
    /// </summary>
    QuickTurn,

    /// <summary>
    /// Unit is selecting thier turn
    /// </summary>
    ActiveTurn,

    /// <summary>
    /// Turn has ended but some statuses or other queued events can occur
    /// </summary>
    EndOfActiveTurn,

    #endregion

    #region Other
    // specialty phases mainly due to waiting

    /// <summary>
    /// used in MP, in GameLoopState waiting for opponent
    /// </summary>
    Standby,

    /// <summary>
    /// used in MP in prior version of this game. Phase GameLoop hangs in prior to phases being started
    /// </summary>
    Prephase,

    /// <summary>
    /// used in WalkAround, gives players time between ticks to input
    /// </summary>
    WaitTick,

    /// <summary>
    /// used in WalkAround, move around the map, can check menus and do actions
    /// </summary>
    NonCombat,

    /// <summary>
    /// sent notification to RL, waiting to receive input options back
    /// </summary>
    RLWait

    #endregion

    

}

