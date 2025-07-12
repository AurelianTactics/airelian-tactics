using System;

/// <summary>
/// Represents a game time object that will be used by CombatState and other files.
/// Contains information about when and how the object should be resolved.
/// </summary>
public class GameTimeObject
{
    private static int nextGameTimeObjectId = 0;

    /// <summary>
    /// The tick at which this object should be resolved. Defaults to -1.
    /// </summary>
    public int ResolveTick { get; set; }

    /// <summary>
    /// Unique identifier for this game time object. Auto-increments and is assigned on creation.
    /// </summary>
    public int GameTimeObjectId { get; private set; }

    /// <summary>
    /// The phase in which this object should be processed. Can be null.
    /// </summary>
    public Phases? Phase { get; set; }

    /// <summary>
    /// The targetunit ID associated with this object. Can be null.
    /// </summary>
    public int? TargetUnitId { get; set; }

    /// <summary>
    /// The actor / caster unit ID associated with this object. Can be null.
    /// </summary>
    public int? ActorUnitId { get; set; }

    /// <summary>
    /// The spell name associated with this object. Can be null.
    /// </summary>
    public SpellName SpellName { get; set; }

    /// <summary>
    /// Base constructor. Creates a new GameTimeObject with auto-incremented ID.
    /// All parameters are optional and can be set to null/default values.
    /// </summary>
    /// <param name="resolveTick">The tick at which this object should be resolved. Defaults to -1.</param>
    /// <param name="phase">The phase in which this object should be processed. Can be null.</param>
    /// <param name="unitId">The unit ID associated with this object. Can be null.</param>
    public GameTimeObject(int? resolveTick = null, Phases? phase = null, int? targetUnitId = null, 
        int? actorUnitId = null, SpellName spellName = null)
    {
        GameTimeObjectId = GetNextGameTimeObjectId();
        ResolveTick = resolveTick ?? -1;
        Phase = phase;
        TargetUnitId = targetUnitId;
        ActorUnitId = actorUnitId;
        SpellName = spellName;
    }

    /// <summary>
    /// Gets the next unique game time object ID and increments the counter.
    /// </summary>
    /// <returns>The next unique game time object ID.</returns>
    private static int GetNextGameTimeObjectId()
    {
        return nextGameTimeObjectId++;
    }

    /// <summary>
    /// Resets the game time object ID counter. Useful for testing or game reset scenarios.
    /// </summary>
    public static void ResetGameTimeObjectIdCounter()
    {
        nextGameTimeObjectId = 0;
    }

    /// <summary>
    /// Returns a string representation of the GameTimeObject.
    /// </summary>
    /// <returns>A string containing the object's properties.</returns>
    public override string ToString()
    {
        return $@"GameTimeObject[
            ID: {GameTimeObjectId}
            ResolveTick: {ResolveTick}
            Phase: {Phase?.ToString() ?? "null"}
            TargetUnitId: {TargetUnitId?.ToString() ?? "null"}
            ActorUnitId: {ActorUnitId?.ToString() ?? "null"}
            SpellName: {SpellName?.ToString() ?? "null"}
        ]";

    }
} 