using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// In CombatScene and mapbuilder manages the game board
/// holds the tiles and allows manipulation
/// in combatscene accessible through to do
/// </summary>
public class Board
{
	#region Fields / Properties

	public Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();
	

	/// <summary>
	/// Gets the maximum bounds of the board 
	/// Represents the largest X and Y coordinates of any tile or combination of tiles on the board.
	/// Updated automatically when tiles are loaded via Load().
	/// </summary>
	public Point maxBoardPoint { get; private set; }
	
	/// <summary>
	/// Pminimum board bounds.
	/// Initialized to the first tile's position during Load(), then updated to track
	/// the smallest X and Y coordinates encountered across all tiles or combination of tiles.
	/// </summary>
	public Point minBoardPoint { get; private set; }
	
	
	Point[] dirs = new Point[4]
	{
		new Point(0, 1),
		new Point(0, -1),
		new Point(1, 0),
		new Point(-1, 0)
	};

    // to do: add back in spawn points at some point
    //spawn points and teamID that can spawn there //(point.x,point.y, teamId) fucking serialization

	//public List<SerializableVector3> spawnList; 
	#endregion

	#region Public

    /// <summary>
    /// Load the board from MapConfig data.
    /// Creates tiles from the configuration and calculates board bounds.
    /// 
    /// Bounds Calculation:
    /// - _min is set to the smallest X,Y coordinates found across all tiles (bottom-left corner)
    /// - _max is set to the largest X,Y coordinates found across all tiles (top-right corner)

    /// </summary>
    /// <param name="mapConfig">The map configuration containing tile data</param>
    public void Load(MapConfig mapConfig)
    {
        if (mapConfig == null || mapConfig.Tiles == null)
        {
            Console.WriteLine("No map configuration or tiles found");
            return;
        }

        tiles.Clear();
        
        // Initialize bounds tracking - will be set from first tile
        bool firstTile = true;
        
        foreach (var tileConfig in mapConfig.Tiles)
        {
            Point position = new Point(tileConfig.X, tileConfig.Y);
            Tile tile = new Tile();
            tile.Load(position, tileConfig.Z);
            
            // Set tile properties from config
            tile.TileType = DetermineTileTypeFromConfig(tileConfig);
            
            tiles[position] = tile;
            
            // Update board bounds to encompass all tiles
            if (firstTile)
            {
                // Initialize bounds with the first tile's position
                minBoardPoint = position;
                maxBoardPoint = position;
                firstTile = false;
            }
            else
            {
                // Expand bounds to include this tile's position
                // Track minimum and maximum coordinates independently
                int minX = Math.Min(minBoardPoint.x, position.x);
                int minY = Math.Min(minBoardPoint.y, position.y);
                int maxX = Math.Max(maxBoardPoint.x, position.x);
                int maxY = Math.Max(maxBoardPoint.y, position.y);
                
                // Update bounds with new values
                minBoardPoint = new Point(minX, minY);
                maxBoardPoint = new Point(maxX, maxY);
            }
        }
        
        Console.WriteLine($"Loaded {tiles.Count} tiles. Bounds: {minBoardPoint} to {maxBoardPoint}");
    }

    /// <summary>
    /// Determine tile type from TileConfig
    /// </summary>
    /// <param name="tileConfig">The tile configuration</param>
    /// <returns>Integer representing tile type</returns>
    private TileTerrainType DetermineTileTypeFromConfig(TileConfig tileConfig)
    {
        // to do: get terrain type from tileConfig

        if(tileConfig.Terrain == "default")
        {
            return TileTerrainType.Ground;
        }

        return TileTerrainType.Ground;
    }

	public Tile GetTile(int x, int y)
	{
		Point p = new Point(x, y);
		return tiles.ContainsKey(p) ? tiles[p] : null;
	}

	public Tile GetTile(Point p)
	{
		return tiles.ContainsKey(p) ? tiles[p] : null;
	}

	public Tile GetTile(Tile t)
	{
		return tiles.ContainsKey(t.pos) ? tiles[t.pos] : null;
	}

	/// <summary>
	/// Place a unit on a specific tile.
	/// </summary>
	/// <param name="position">The position of the tile to place the unit on</param>
	/// <param name="unitId">The ID of the unit to place</param>
	/// <returns>True if placement was successful, false if tile doesn't exist or is occupied</returns>
	public bool PlaceUnitOnTile(Point position, int unitId)
	{
		Tile tile = GetTile(position);
		if (tile == null)
		{
			return false; // Tile doesn't exist
		}

		if (tile.UnitId != null)
		{
			return false; // Tile is already occupied
		}

		tile.UnitId = unitId;
		return true;
	}

	/// <summary>
	/// Place a unit on a specific tile.
	/// </summary>
	/// <param name="tile">The tile to place the unit on</param>
	/// <param name="unitId">The ID of the unit to place</param>
	/// <returns>True if placement was successful, false if tile is occupied</returns>
	public bool PlaceUnitOnTile(Tile tile, int unitId)
	{
		if (tile == null)
		{
			return false;
		}

		if (tile.UnitId != null)
		{
			return false; // Tile is already occupied
		}

		tile.UnitId = unitId;
		return true;
	}

	/// <summary>
	/// Remove a unit from a specific tile.
	/// </summary>
	/// <param name="position">The position of the tile to remove the unit from</param>
	/// <returns>True if removal was successful, false if tile doesn't exist or is empty</returns>
	public bool RemoveUnitFromTile(Point position)
	{
		Tile tile = GetTile(position);
		if (tile == null || tile.UnitId == null)
		{
			return false;
		}

		tile.UnitId = null;
		return true;
	}

	/// <summary>
	/// Check if a tile is empty (contains no unit).
	/// </summary>
	/// <param name="position">The position to check</param>
	/// <returns>True if tile exists and is empty, false otherwise</returns>
	public bool IsTileEmpty(Point position)
	{
		Tile tile = GetTile(position);
		return tile != null && tile.UnitId == null;
	}

	/// <summary>
	/// Check if a tile is occupied by a unit.
	/// </summary>
	/// <param name="position">The position to check</param>
	/// <returns>True if tile exists and contains a unit, false otherwise</returns>
	public bool IsTileOccupied(Point position)
	{
		Tile tile = GetTile(position);
		return tile != null && tile.UnitId != null;
	}

	/// <summary>
	/// Get the unit ID at a specific position, if any
	/// </summary>
	/// <param name="position">The position to check</param>
	/// <returns>The unit ID at the position, or null if no unit is there</returns>
	public int? GetUnitAtPosition(Point position)
	{
		Tile tile = GetTile(position);
		return tile?.UnitId;
	}

	/// <summary>
	/// Get the current position of a unit on the board
	/// </summary>
	/// <param name="unitId">The ID of the unit to find</param>
	/// <returns>The position of the unit, or null if not found</returns>
	public Point? GetUnitPosition(int unitId)
	{
		// Search through all tiles to find the unit
		foreach (var kvp in tiles)
		{
			if (kvp.Value.UnitId == unitId)
			{
				return kvp.Key;
			}
		}
		
		// Unit not found on board
		return null;
	}

	/// <summary>
	/// Get valid move tiles for a unit from a given position with a movement range
	/// </summary>
	/// <param name="startPosition">The starting position</param>
	/// <param name="moveRange">The maximum movement range</param>
	/// <returns>List of tiles the unit can move to</returns>
	public List<Tile> GetValidMoveTiles(Point startPosition, int moveRange)
	{
		Tile startTile = GetTile(startPosition);
		if (startTile == null)
		{
			return new List<Tile>();
		}

		// Use the board's search functionality to find valid move tiles
		List<Tile> validTiles = Search(startTile, (currentTile, nextTile) =>
		{
			// Can move to a tile if:
			// 1. It's not occupied by another unit
			// 2. The height difference is not too great (basic jump check)
			// 3. It's within movement range (handled by search algorithm)
			
			if (nextTile.UnitId != null)
			{
				return false; // Tile is occupied
			}
			
			// Basic height check - allow movement if height difference is 1 or less
			int heightDiff = Math.Abs(nextTile.height - currentTile.height);
			if (heightDiff > 1)
			{
				return false; // Too high to jump
			}
			
			return true; // Valid tile to move to
		});

		// Filter out tiles that are beyond movement range
		List<Tile> tilesInRange = new List<Tile>();
		foreach (Tile tile in validTiles)
		{
			if (tile.distance <= moveRange && tile != startTile)
			{
				tilesInRange.Add(tile);
			}
		}

		return tilesInRange;
	}

	// public Tile GetTile(PlayerUnit target)
	// {
	// 	Point p = new Point();
	// 	p.x = target.TileX;
	// 	p.y = target.TileY;
	// 	return GetTile(p);
	// }

	public List<Tile> Search(Tile start, Func<Tile, Tile, bool> addTile)
	{
		List<Tile> retValue = new List<Tile>();
		retValue.Add(start);

		ClearSearch();
		Queue<Tile> checkNext = new Queue<Tile>();
		Queue<Tile> checkNow = new Queue<Tile>();

		start.distance = 0;
		checkNow.Enqueue(start);

		while (checkNow.Count > 0)
		{
			Tile t = checkNow.Dequeue();
			for (int i = 0; i < 4; ++i)
			{
				Tile next = GetTile(t.pos + dirs[i]);
				if (next == null || next.distance <= t.distance + 1)
					continue;

				if (addTile(t, next))
				{
					next.distance = t.distance + 1;
					next.prev = t;
					checkNext.Enqueue(next);
					retValue.Add(next);
				}
			}

			if (checkNow.Count == 0)
				SwapReference(ref checkNow, ref checkNext);
		}

		return retValue;
	}

	// //Combat uses search
	// //WalkAroundMode has multiple units so needs distanceDict and prevDict for each unitId
	// public List<Tile> SearchWalkAround(Tile start, int unitId, Func<Tile, Tile, bool> addTile)
	// {
	// 	List<Tile> retValue = new List<Tile>();
	// 	retValue.Add(start);
	// 	//Debug.Log("reaching here " + unitId);
	// 	ClearSearchWalkAround(unitId);
	// 	Queue<Tile> checkNext = new Queue<Tile>();
	// 	Queue<Tile> checkNow = new Queue<Tile>();

	// 	//start.distance = 0;
	// 	start.distanceDict[unitId] = 0;
	// 	checkNow.Enqueue(start);

	// 	while (checkNow.Count > 0)
	// 	{
	// 		Tile t = checkNow.Dequeue();
	// 		for (int i = 0; i < 4; ++i)
	// 		{
	// 			Tile next = GetTile(t.pos + dirs[i]);
	// 			//if (next == null || next.distance <= t.distance + 1)
	// 			//    continue;
	// 			//continue if tile does not exist or if the tile has already been added to the distanceDict
	// 			if (next == null)
	// 				continue;
	// 			if (next.distanceDict.ContainsKey(unitId) && t.distanceDict.ContainsKey(unitId) && next.distanceDict[unitId] <= t.distanceDict[unitId] + 1)
	// 				continue;

	// 			if (addTile(t, next))
	// 			{
	// 				//next.distance = t.distance + 1;
	// 				//next.prev = t;
	// 				next.distanceDict[unitId] = t.distanceDict[unitId] + 1;
	// 				next.prevDict[unitId] = t;
	// 				checkNext.Enqueue(next);
	// 				retValue.Add(next);
	// 			}
	// 		}

	// 		if (checkNow.Count == 0)
	// 			SwapReference(ref checkNow, ref checkNext);
	// 	}

	// 	return retValue;
	// }



	// public void UpdatePlayerUnitTile(PlayerUnit pu, Tile newTile)
	// {
	// 	Point p = new Point(pu.TileX, pu.TileY);
	// 	GetTile(p).UnitId = NameAll.NULL_UNIT_ID;
	// 	GetTile(newTile.pos).UnitId = pu.TurnOrder;
	// }

	public void UpdatePlayerUnitTileSwap(PlayerUnit pu, Tile newTile)
	{
		//Point p = new Point(pu.TileX, pu.TileY); //this tile has already been updated with the actor who swapped in
		//GetTile(p).UnitId = NameAll.NULL_UNIT_ID;
		GetTile(newTile.pos).UnitId = pu.UnitId;
	}


	#endregion

	#region Private
	void ClearSearch()
	{
		foreach (Tile t in tiles.Values)
		{
			t.prev = null;
			t.distance = int.MaxValue;
		}
	}

	// void ClearSearchWalkAround(int unitId)
	// {
	// 	foreach (Tile t in tiles.Values)
	// 	{
	// 		//t.prev = null;
	// 		//t.distance = int.MaxValue;
	// 		t.prevDict.Remove(unitId);
	// 		t.distanceDict.Remove(unitId);
	// 	}
	// }

	void SwapReference(ref Queue<Tile> a, ref Queue<Tile> b)
	{
		Queue<Tile> temp = a;
		a = b;
		b = temp;
	}
	#endregion

}