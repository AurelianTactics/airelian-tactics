using System.Collections;
using System.Collections.Generic;


/// <summary>
/// In Combat and WA scene, the game board is represented by a board object which
/// holds a list of Tile object. The Tile objects have some GameObject properties
/// for visual displays and game related properties like units that are on the tile.
/// Tiles are created, called and manipulated from Board.cs
/// </summary>
public class Tile
{

	#region Const
	/// <summary>
	/// Used for scaling the height of a tile
	/// </summary>
	// public const float stepHeight = 0.25f;

	/// <summary>
	/// used for centering the indicator for the tile
	/// used for centering the height of units that walk around the map
	/// </summary>
	// public const float centerHeight = 0.45f;
	#endregion

	#region Fields / Properties
	/// <summary>
	/// Contains the x and y coordinates of a tile
	/// </summary>
	public Point pos;

	/// <summary>
	/// Has the z coordinates (height) of the tile
	/// </summary>
	public int height;

	/// <summary>
	/// When calculating valid movements in cref, use the prev field
	/// </summary>
	public Tile prev;

	/// <summary>
	/// WHen calculating valid movements in cref, use the distance to track how many
	/// additional tiles the unit can move to
	/// </summary>
	public int distance;

	/// <summary>
	/// walkAroundMode: multiple units can be moving around at the 
	/// same time so need dictionaries to distinguish unique prev and unique distance 
	/// </summary>
	//public Dictionary<int, Tile> prevDict = new Dictionary<int, Tile>();

	/// <summary>
	/// walkAroundMode: multiple units can be moving around at the 
	/// same time so need dictionaries to distinguish unique prev and unique distance 
	/// </summary>
	public Dictionary<int, int> distanceDict = new Dictionary<int, int>();

	private int? unitId = null;

	/// <summary>
	/// The ID of the unit occupying this tile, or null if the tile is empty.
	/// </summary>
	public int? UnitId
	{
		get { return unitId; }
		set { unitId = value; }
	}

	private int pickUpId = 0; //0 for nothing, 1 for crystals

	/// <summary>
	/// Can sometimes be objects on tiles
	/// for now, 0 for nothing, 1 for crystals
	/// to do: add enums for this type
	/// </summary>
	public int PickUpId
	{
		get { return pickUpId; }
		set { pickUpId = value; }
	}

	private TileTerrainType tileType = TileTerrainType.Ground;
	/// <summary>
	/// for WA mode Tile Type can be exit map as well
	/// </summary>
	public TileTerrainType TileType
	{
		get { return tileType; }
		set { tileType = value; }
	}
	#endregion

	#region Public

	/// <summary>
	/// Load the Tile. I think tiles are loaded when created by the board
	/// </summary>
	public void Load(Point p, int h)
	{
		pos = p;
		height = h;
	}

	/// <summary>
	///
	/// </summary>


	#endregion


	/// <summary>
	/// Get info on tile for debugging or display purposes in UI Target panel
	/// </summary>
	public string GetTileSummary()
	{
		return " (" + this.pos.x + "," + this.pos.y + ")" + " unitID: " + this.unitId + " height: " + this.height;
	}
}