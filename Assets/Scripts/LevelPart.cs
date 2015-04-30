using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelPart : MonoBehaviour {

	public List<LevelTile> mLevelTileList;
	
	public Transform mTriggableListAnchor;
	public Transform mDecalObjectsAnchor;
	public Transform mItemObjectsAnchor;
	public Transform mTilesAnchor;
	public Transform mEnemiesAnchor;

	public LevelTile mStartingTile;

	public string levelPartName;

	void Awake()
	{
		awakeAnchors();
	}

	public void awakeAnchors()
	{
		// Find or create object anchors
		mTriggableListAnchor = transform.Find("TriggerableObjects");
		if(mTriggableListAnchor == null){
			GameObject obj = new GameObject();
			obj.name = "TriggerableObjects";
			obj.transform.parent = gameObject.transform;
			mTriggableListAnchor = obj.transform;
		}
		mDecalObjectsAnchor = transform.Find("DecalObjects");
		if(mDecalObjectsAnchor == null){
			GameObject obj = new GameObject();
			obj.name = "DecalObjects";
			obj.transform.parent = gameObject.transform;
			mDecalObjectsAnchor = obj.transform;
		}
		mItemObjectsAnchor = transform.Find("ItemObjects");
		if(mItemObjectsAnchor == null){
			GameObject obj = new GameObject();
			obj.name = "ItemObjects";
			obj.transform.parent = gameObject.transform;
			mItemObjectsAnchor = obj.transform;
		}
		mTilesAnchor = transform.Find("Tiles");
		if(mTilesAnchor == null){
			GameObject obj = new GameObject();
			obj.name = "Tiles";
			obj.transform.parent = gameObject.transform;
			mTilesAnchor = obj.transform;
		}
		mEnemiesAnchor = transform.Find("Enemies");
		if(mEnemiesAnchor == null){
			GameObject obj = new GameObject();
			obj.name = "Enemies";
			obj.transform.parent = gameObject.transform;
			mEnemiesAnchor = obj.transform;
		}
		
		UpdateLevelTileList();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Rotate()
	{
		// set to rotated position
		foreach(LevelTile tile in mLevelTileList)
		{
			if(tile.Pos != new Vector2(0,0)){
				tile.Reposition(new Vector2(tile.Pos.y * 1, tile.Pos.x * -1), this, false);
			}
		}

		//rotate tile orientation
		foreach(LevelTile tile in mLevelTileList)
		{
			tile.Rotate();
		}

		//set new neighbors for each tile
		foreach(LevelTile tile in mLevelTileList)
		{
			if(tile.Pos != new Vector2(0,0)){
				tile.updateNeighbors();
			}
		}
	}

	public void UpdateLevelTileList()
	{
		mLevelTileList = new List<LevelTile>();
		foreach(Transform child in transform.Find("Tiles"))
		{
			mLevelTileList.Add(child.GetComponent<LevelTile>());
		}
	}

	/// <summary>
	/// Creates a new level-tile.
	/// </summary>
	/// <returns>
	/// The new tile.
	/// </returns>
	/// <param name='pos'>
	/// Position where to place the new tile
	/// </param>
	/// <param name='tileOrigin'>
	/// the tile, from which the new tile is created from
	/// </param>
	public LevelTile CreateTile(Vector2 pos, LevelTile tileOrigin)
	{
		LevelTile tempTile = null;
		
		// hold neighbors
		LevelTile neighborNorth = null;
		LevelTile neighborWest = null;
		LevelTile neighborEast = null;
		LevelTile neighborSouth = null;
		
		bool isOccupied = false;
		
		foreach(LevelTile tile in mLevelTileList)
		{
			if(tile.Pos == pos)
			{
				isOccupied = true;
				//tempTile = tile;
			}
			
			// get neighbors
			if(tile.Pos == (pos + new Vector2(-1,0)))
			{
				neighborWest = tile;
			}
			if(tile.Pos == (pos + new Vector2(+1,0)))
			{
				neighborEast = tile;
			}
			if(tile.Pos == (pos + new Vector2(0,+1)))
			{
				neighborNorth = tile;
			}
			if(tile.Pos == (pos + new Vector2(0,-1)))
			{
				neighborSouth = tile;
			}
		}
		
		// create tile
		if(!isOccupied)
		{
			GameObject tempObj = Instantiate(World.Instance.mLevelTilePrefab) as GameObject; 
			tempObj.transform.parent = mTilesAnchor;
			tempObj.transform.localPosition = new Vector3(pos.x * World.TILE_SIZE, 0, pos.y * World.TILE_SIZE);
			tempObj.name = "Tile_" + mLevelTileList.Count;
			
			tempTile = tempObj.GetComponent<LevelTile>();
			tempTile.Pos = pos;
			tempTile.LevelPartBelongingTo = this;
			
			if(neighborNorth != null)
			{
				tempTile.NeighborNorth = neighborNorth;
				neighborNorth.NeighborSouth = tempTile;
				
				tempTile.WallNorth.SharedWall = neighborNorth.WallSouth;
				tempTile.NeighborNorth.WallSouth.SharedWall = tempTile.WallNorth;
				
				tempTile.SetWallStatus(LookingDirection.North, neighborNorth.WallSouthActive);
				tempTile.SetWallPassable(LookingDirection.North, neighborNorth.WallSouthPassable);
			}
			if(neighborWest != null)
			{
				tempTile.NeighborWest = neighborWest;
				neighborWest.NeighborEast = tempTile;
				
				tempTile.WallWest.SharedWall = neighborWest.WallEast;
				tempTile.NeighborWest.WallEast.SharedWall = tempTile.WallWest;
				
				tempTile.SetWallStatus(LookingDirection.West, neighborWest.WallEastActive);
				tempTile.SetWallPassable(LookingDirection.West, neighborWest.WallEastPassable);
			}
			if(neighborEast != null)
			{
				tempTile.NeighborEast = neighborEast;
				neighborEast.NeighborWest = tempTile;
				
				tempTile.WallEast.SharedWall = neighborEast.WallWest;
				tempTile.NeighborEast.WallWest.SharedWall = tempTile.WallEast;
				
				tempTile.SetWallStatus(LookingDirection.East, neighborEast.WallWestActive);
				tempTile.SetWallPassable(LookingDirection.East, neighborEast.WallWestPassable);
			}
			if(neighborSouth != null)
			{
				tempTile.NeighborSouth = neighborSouth;
				neighborSouth.NeighborNorth = tempTile;
				
				tempTile.WallSouth.SharedWall = neighborSouth.WallNorth;
				tempTile.NeighborSouth.WallNorth.SharedWall = tempTile.WallSouth;
				
				tempTile.SetWallStatus(LookingDirection.South, neighborSouth.WallNorthActive);
				tempTile.SetWallPassable(LookingDirection.South, neighborSouth.WallNorthPassable);
			}
			tempTile.InitVariables();
			
			// update wall textures, making them identical to the textures of the original tile:
			if(tileOrigin)
			{
				tempTile.WallNorth.SetTexture(tileOrigin.WallNorth.CurrentMaterial);
				tempTile.WallSouth.SetTexture(tileOrigin.WallSouth.CurrentMaterial);
				tempTile.WallEast.SetTexture(tileOrigin.WallEast.CurrentMaterial);
				tempTile.WallWest.SetTexture(tileOrigin.WallWest.CurrentMaterial);
				tempTile.Ceiling.SetTexture(tileOrigin.Ceiling.CurrentMaterial);
				tempTile.Floor.SetTexture(tileOrigin.Floor.CurrentMaterial);
			}
			
			mLevelTileList.Add(tempTile);
		}
		else
		{
			Debug.LogWarning("World: There already is a tile on that position");
		}
		
		return tempTile;
	}
	
	public LevelTile CreateTile(Vector2 pos, int texN, int texE, int texS, int texW, int texC, int texF, LevelTile tileOrigin)
	{
		LevelTile newTile = CreateTile( pos, tileOrigin);
		newTile.WallNorth.SetTexture(texN);
		newTile.WallSouth.SetTexture(texS);
		newTile.WallEast.SetTexture(texE);
		newTile.WallWest.SetTexture(texW);
		newTile.Ceiling.SetTexture(texC);
		newTile.Floor.SetTexture(texF);
		
		return newTile;
	}
	
	public LevelTile CreateTileInDirection(LookingDirection dir, LevelTile tile)
	{
		Vector2 pos;
		LevelTile selecTile = null;
		
		switch(dir)
		{
		case LookingDirection.North:
			pos = new Vector2(tile.Pos.x, tile.Pos.y + 1);
			selecTile = CreateTile(pos, tile);
			if(selecTile != null)
				tile.SetWallStatus(LookingDirection.North, false);
			break;
			
		case LookingDirection.East:
			pos = new Vector2(tile.Pos.x + 1, tile.Pos.y);
			selecTile = CreateTile(pos, tile);
			if(selecTile != null)
				tile.SetWallStatus(LookingDirection.East, false);
			break;
			
		case LookingDirection.South:
			pos = new Vector2(tile.Pos.x, tile.Pos.y - 1);
			selecTile = CreateTile(pos, tile);
			if(selecTile != null)
				tile.SetWallStatus(LookingDirection.South, false);
			break;
			
		case LookingDirection.West:
			pos = new Vector2(tile.Pos.x - 1, tile.Pos.y);
			selecTile = CreateTile(pos, tile);
			if(selecTile != null)
				tile.SetWallStatus(LookingDirection.West, false);
			break;
			
		default:
			Debug.LogWarning("createTileInDirection: something went wrong with the direction!");
			break;
		}
		
		return selecTile;
	}

	public LevelTile GetTileByPos(Vector2 pos)
	{
		foreach(LevelTile tile in mLevelTileList)
		{
			if(tile.Pos == pos)
			{
				return tile;
			}
		}
		
		return null;
	}

	public void DeleteLevel()
	{
		GameObject obj;

		if(transform.Find("Tiles") != null){
			DestroyImmediate(transform.Find("Tiles").gameObject);
			obj = new GameObject();
			obj.name = "Tiles";
			obj.transform.parent = transform;
		}

		if(transform.Find("DecalObjects") != null){
			DestroyImmediate(transform.Find("DecalObjects").gameObject);
			obj = new GameObject();
			obj.name = "DecalObjects";
			obj.transform.parent = transform;
		}

		if(transform.Find("ItemObjects") != null){
			DestroyImmediate(transform.Find("ItemObjects").gameObject);
			obj = new GameObject();
			obj.name = "ItemObjects";
			obj.transform.parent = transform;
		}

		if(transform.Find("TriggerableObjects") != null){
			DestroyImmediate(transform.Find("TriggerableObjects").gameObject);
			obj = new GameObject();
			obj.name = "TriggerableObjects";
			obj.transform.parent = transform;
		}

		if(transform.Find("Enemies") != null){
			DestroyImmediate(transform.Find("Enemies").gameObject);
			obj = new GameObject();
			obj.name = "Enemies";
			obj.transform.parent = transform;
		}
		
		// get new references
		mTriggableListAnchor = transform.Find("TriggerableObjects");
		mDecalObjectsAnchor = transform.Find("DecalObjects");
		mItemObjectsAnchor = transform.Find("ItemObjects");
		mTilesAnchor = transform.Find("Tiles");
		mEnemiesAnchor = transform.Find("Enemies");
		
		UpdateLevelTileList();
	}
}
