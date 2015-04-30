using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LevelTile : MonoBehaviour {

	// Walls references
	private LevelWall mWallNorth;
	public LevelWall WallNorth {get {return mWallNorth;}}
	private LevelWall mWallSouth;
	public LevelWall WallSouth {get {return mWallSouth;}}
	private LevelWall mWallEast;
	public LevelWall WallEast {get {return mWallEast;}}
	private LevelWall mWallWest;
	public LevelWall WallWest {get {return mWallWest;}}
	private LevelWall mFloor;
	public LevelWall Floor { get {return mFloor;} }
	private LevelWall mCeiling;
	public LevelWall Ceiling { get {return mCeiling;} }

	// test room prefaB
	public GameObject prefabRoom;

	// holds whether or not walls are active
	public bool WallNorthActive {get {return mWallNorth.IsActive;}}
	public bool WallSouthActive {get {return mWallSouth.IsActive;}}
	public bool WallWestActive {get {return mWallWest.IsActive;}}
	public bool WallEastActive {get {return mWallEast.IsActive;}}
	public bool WallNorthPassable { get {return mWallNorth.IsPassable;} set {mWallNorth.IsPassable = value;}}
	public bool WallSouthPassable { get {return mWallSouth.IsPassable;} set {mWallSouth.IsPassable = value;}}
	public bool WallWestPassable { get {return mWallWest.IsPassable;} set {mWallWest.IsPassable = value;}}
	public bool WallEastPassable { get {return mWallEast.IsPassable;} set {mWallEast.IsPassable = value;}}

	// Neighbor References
	[SerializeField]
	private LevelTile mNeighborNorth;
	public LevelTile NeighborNorth {get {return mNeighborNorth;} set {mNeighborNorth = value;}}
	[SerializeField]
	private LevelTile mNeighborSouth;
	public LevelTile NeighborSouth {get {return mNeighborSouth;} set {mNeighborSouth = value;}}
	[SerializeField]
	private LevelTile mNeighborEast;
	public LevelTile NeighborEast {get {return mNeighborEast;} set {mNeighborEast = value;}}
	[SerializeField]
	private LevelTile mNeighborWest;
	public LevelTile NeighborWest {get {return mNeighborWest;} set {mNeighborWest = value;}}
	
	// item placed on tile
	[SerializeField]
	private ItemObject mItemPlaced;
	public ItemObject ItemPlaced { get {return mItemPlaced;} set {mItemPlaced = value;}}
	public List<GameObject> AttachedObjects;
	
	// Actor positioned on this tile
	private MovableActor mActorOnTile;
	public MovableActor ActorOnTile {get {return mActorOnTile;} set {mActorOnTile = value;}}

	// World coordinates
	[SerializeField]
	private Vector2 mPos;
	public Vector2 Pos { get{return mPos;} set{mPos = value;}}

	// LevelPart this tile belongs to
	[SerializeField]
	private LevelPart mLevelPartBelongingTo;
	public LevelPart LevelPartBelongingTo {get {return mLevelPartBelongingTo;} set{mLevelPartBelongingTo = value;}}

	void Awake()
	{
		mWallNorth = transform.FindChild("WallNorth").GetComponent<LevelWall>();
		mWallSouth = transform.FindChild("WallSouth").GetComponent<LevelWall>();
		mWallWest = transform.FindChild("WallWest").GetComponent<LevelWall>();
		mWallEast = transform.FindChild("WallEast").GetComponent<LevelWall>();
		mFloor = transform.FindChild("Floor").GetComponent<LevelWall>();
		mCeiling = transform.FindChild("Ceiling").GetComponent<LevelWall>();
	}
	
	// Use this for initialization
	void Start () 
	{
		InitVariables();


	}
	
	public void InitVariables()
	{
		// INIT //
	}

	public void UpdateAttachedObjectsList()
	{
		//Update AttachedObjects list, some objects may have benn deleted
		for(int i = 0; i < AttachedObjects.Count; i++)
		{
			if (AttachedObjects[i] == null)
			{
				AttachedObjects.RemoveAt(i);
			}
		}
	}

	// rotates tile by 90 degrees, updating textures and wall activations
	public void Rotate()
	{
		int tempTex = mWallNorth.CurrentMaterial;
		bool tempAct = mWallNorth.IsActive;
		bool tempPass = mWallNorth.IsPassable;
		UpdateAttachedObjectsList();
		
		// rotate Textures
		mWallNorth.SetTexture(mWallWest.CurrentMaterial);
		mWallWest.SetTexture(mWallSouth.CurrentMaterial);
		mWallSouth.SetTexture(mWallEast.CurrentMaterial);
		mWallEast.SetTexture(tempTex);
		
		// rotate Wall activation & passbility
		SetWallStatus(LookingDirection.North, mWallWest.IsActive, mWallWest.IsPassable);
		SetWallStatus(LookingDirection.West, mWallSouth.IsActive, mWallSouth.IsPassable);
		SetWallStatus(LookingDirection.South, mWallEast.IsActive, mWallEast.IsPassable);
		SetWallStatus(LookingDirection.East, tempAct, tempPass);

		// Rotate attached Objects
		foreach(GameObject obj in AttachedObjects)
		{
			Vector2 relPos = new Vector2(obj.transform.localPosition.x - transform.localPosition.x, obj.transform.localPosition.z - transform.localPosition.z);
			Vector2 newRelPos = new Vector2(relPos.y * 1, relPos.x * -1);
			obj.transform.position = new Vector3(transform.localPosition.x + newRelPos.x, obj.transform.position.y , transform.localPosition.z + newRelPos.y);
			obj.transform.Rotate(0,90,0);
			// rotate wall facing is object has attachedToWall component
			AttachedToWall attachObj = obj.GetComponent<AttachedToWall>();
			if(attachObj != null)
				attachObj.Rotate(this);
		}
	}

	// Places tile on new space in LevelPart and updated references of shared walls and neighbors
	public void Reposition(Vector2 newPos, LevelPart newBelonging, bool setNeigh)
	{
		UpdateAttachedObjectsList();
		Vector2 movedDistance = new Vector2(newPos.x - Pos.x, newPos.y - Pos.y);

		// set LevelPart belonging to
		mLevelPartBelongingTo = newBelonging;

		// Delete Neighbors refrence to this tile, and delete own references
		DeleteNeighborRelation(LookingDirection.North);
		DeleteNeighborRelation(LookingDirection.East);
		DeleteNeighborRelation(LookingDirection.South);
		DeleteNeighborRelation(LookingDirection.West);
		
		// reposition on LevelPart
		gameObject.transform.localPosition = new Vector3(newPos.x * World.TILE_SIZE, 0, newPos.y * World.TILE_SIZE);
		mPos = newPos;

		// get new neighbors from LevelPart
		if(setNeigh)
		{
			SetNeighbor(LookingDirection.North, mLevelPartBelongingTo.GetTileByPos(new Vector2(newPos.x, newPos.y + 1)));
			SetNeighbor(LookingDirection.West, mLevelPartBelongingTo.GetTileByPos(new Vector2(newPos.x - 1, newPos.y)));
			SetNeighbor(LookingDirection.South, mLevelPartBelongingTo.GetTileByPos(new Vector2(newPos.x, newPos.y - 1)));
			SetNeighbor(LookingDirection.East, mLevelPartBelongingTo.GetTileByPos(new Vector2(newPos.x + 1, newPos.y)));
		}

		// Take objects belonging to tile with it
		foreach(GameObject obj in AttachedObjects)
		{
			obj.transform.position = new Vector3(obj.transform.position.x + movedDistance.x * World.TILE_SIZE, obj.transform.position.y , obj.transform.position.z + movedDistance.y * World.TILE_SIZE);
		}
	}

	public void updateNeighbors()
	{
		Vector2 newPos = Pos;
		SetNeighbor(LookingDirection.North, mLevelPartBelongingTo.GetTileByPos(new Vector2(newPos.x, newPos.y + 1)));
		SetNeighbor(LookingDirection.West, mLevelPartBelongingTo.GetTileByPos(new Vector2(newPos.x - 1, newPos.y)));
		SetNeighbor(LookingDirection.South, mLevelPartBelongingTo.GetTileByPos(new Vector2(newPos.x, newPos.y - 1)));
		SetNeighbor(LookingDirection.East, mLevelPartBelongingTo.GetTileByPos(new Vector2(newPos.x + 1, newPos.y)));
	}

	// Deletes a neighbor reference
	public void DeleteNeighborRelation(LookingDirection dir)
	{
		switch(dir)
		{
		case LookingDirection.North:
			if(mNeighborNorth != null)
			{
				mNeighborNorth.WallSouth.SharedWall = null;
				mNeighborNorth.NeighborSouth = null;
				mNeighborNorth = null;
			}
			mWallNorth.SharedWall = null;
			break;

		case LookingDirection.East:
			if(mNeighborEast != null)
			{
				mNeighborEast.WallWest.SharedWall = null;
				mNeighborEast.NeighborWest = null;
				mNeighborEast = null;
			}
			mWallEast.SharedWall = null;
			break;

		case LookingDirection.South:
			if(mNeighborSouth != null)
			{
				mNeighborSouth.WallNorth.SharedWall = null;
				mNeighborSouth.NeighborNorth = null;
				mNeighborSouth = null;
			}
			mWallSouth.SharedWall = null;
			break;

		case LookingDirection.West:
			if(mNeighborWest != null)
			{
				mNeighborWest.WallEast.SharedWall = null;
				mNeighborWest.NeighborEast = null;
				mNeighborWest = null;
			}
			mWallWest.SharedWall = null;
			break;
		}
	}

	// sets a new neighbor and updates references for shared walls
	public void SetNeighbor(LookingDirection dir, LevelTile tile)
	{
		switch (dir)
		{
		case LookingDirection.North:
			mNeighborNorth = tile;
			if(tile != null)
			{
				tile.NeighborSouth = this;
				tile.WallSouth.SharedWall = this.mWallNorth;
				mWallNorth.SharedWall = tile.WallSouth;
			}
			else
			{
				mWallNorth.SharedWall = null;
			}

		break;
			
		case LookingDirection.East:
			mNeighborEast = tile;
			if(tile != null)
			{
				tile.NeighborWest = this;
				tile.WallWest.SharedWall = this.mWallEast;
				mWallEast.SharedWall = tile.WallWest;
			}
			else
			{
				mWallEast.SharedWall = null;
			}
		
		break;
			
		case LookingDirection.South:
			mNeighborSouth = tile;
			if(tile != null)
			{
				tile.NeighborNorth = this;
				mWallSouth.SharedWall = tile.WallNorth;
				tile.WallNorth.SharedWall = this.mWallSouth;
			}
			else
			{
				mWallSouth.SharedWall = null;
			}

		break;
			
		case LookingDirection.West:
			mNeighborWest = tile;
			if(tile != null)
			{
				tile.NeighborEast = this;
				mWallWest.SharedWall = tile.WallEast;
				tile.WallEast.SharedWall = this.mWallWest;
			}
			else
			{
				mWallWest.SharedWall = null;
			}

		break;
		}
	}
	
	public void SetWallStatus(LookingDirection direc, bool status)
	{
		//Debug.Log("happened " + direc + "/" + status);
		
		switch(direc)
		{
		case LookingDirection.North:
			mWallNorth.SetActivation(status);
			break;
			
		case LookingDirection.South:
			mWallSouth.SetActivation(status);
			break;
			
			case LookingDirection.East:
				mWallEast.SetActivation(status);
			break;
			
			case LookingDirection.West:
				mWallWest.SetActivation(status);
			break;
		}
		//Debug.Log("reached it " + status + "/" + mWallNorthActive + "|" + mWallSouthActive + "|" + mWallWestActive + "|" + mWallEastActive);
	}
	public void SetWallStatus(LookingDirection direc, bool status, bool pass)
	{
		//Debug.Log("happened " + direc + "/" + status);
		
		switch(direc)
		{
		case LookingDirection.North:
			mWallNorth.SetActivation(status);
			mWallNorth.SetPassabble(pass);
			break;
			
		case LookingDirection.South:
			mWallSouth.SetActivation(status);
			mWallSouth.SetPassabble(pass);
			break;
			
		case LookingDirection.East:
			mWallEast.SetActivation(status);
			mWallEast.SetPassabble(pass);
			break;
			
		case LookingDirection.West:
			mWallWest.SetActivation(status);
			mWallWest.SetPassabble(pass);
			break;
		}
		//Debug.Log("reached it " + status + "/" + mWallNorthActive + "|" + mWallSouthActive + "|" + mWallWestActive + "|" + mWallEastActive);
	}
	
	// paramters:
	// obj - obj to create
	// dest - if true, destroy middle object, if false create obj
	public void AddObject(int obj, ObjectTypes objType, TilePositions pos, bool attachToSide)
	{
		//mMiddleObject = obj;
		GameObject tempObj = null;
		Vector3 objPos = new Vector3(0,(float)World.TILE_HEIGHT/2,0) + transform.position;
		float posFactor = 5;
		if(attachToSide)
			posFactor = 10f;
		
		// Instantitate correctly by tyoe
		switch(objType)
		{
		case ObjectTypes.Decals:
			tempObj = Instantiate(World.Instance.mDecalsPrefabList[obj]) as GameObject;
			tempObj.name = gameObject.name + "_" + World.Instance.mDecalsPrefabNameList[obj];	
			break;
		case ObjectTypes.Items:
			tempObj = Instantiate(World.Instance.mItemsPrefabList[obj]) as GameObject;
			tempObj.name = gameObject.name + "_" + World.Instance.mItemsPrefabNameList[obj];	
			break;
		case ObjectTypes.Triggerables:
			tempObj = Instantiate(World.Instance.mTriggerablesPrefabList[obj]) as GameObject;
			tempObj.name = gameObject.name + "_" + World.Instance.mTriggerablesPrefabNameList[obj];	
			break;
		case ObjectTypes.Enemies:
			tempObj = Instantiate(World.Instance.mEnemiesPrefabList[obj]) as GameObject;
			tempObj.name = gameObject.name + "_" + World.Instance.mEnemiesPrefabNameList[obj];	
			break;
		}
		
		// attach to wall and get name
		AttachedToWall attachObj = tempObj.GetComponent<AttachedToWall>();
		
		// calculate position in world and wall attachment
		switch(pos)
		{
			case TilePositions.NorthWest:
				objPos += new Vector3(-posFactor, 0 , posFactor);
			break;
			
			case TilePositions.North:
				objPos += new Vector3(0, 0 , posFactor);
				if(attachObj != null)
				{
					attachObj.mWallAttachedTo = WallNorth;
					attachObj.mFacing=LookingDirection.South;
				}
			break;
			
			case TilePositions.NorthEast:
				objPos += new Vector3(posFactor, 0 , posFactor);
			break;
			
			case TilePositions.West:
				objPos += new Vector3(-posFactor, 0 , 0);
				tempObj.transform.Rotate(Vector3.up * -90);
				if(attachObj != null)
				{
					attachObj.mWallAttachedTo = WallWest;
					attachObj.mFacing = LookingDirection.East;
				}
			break;
			
			case TilePositions.East:
				objPos += new Vector3(posFactor, 0 , 0);
				tempObj.transform.Rotate(Vector3.up * 90);
				if(attachObj != null)
				{
					attachObj.mWallAttachedTo = WallEast;
					attachObj.mFacing = LookingDirection.West;
				}
			break;
			
			case TilePositions.SouthWest:
				objPos += new Vector3(-posFactor, 0 , -posFactor);
			break;
			
			case TilePositions.South:
				objPos += new Vector3(0, 0 , -posFactor);
				tempObj.transform.Rotate(Vector3.up * 180);
				if(attachObj != null)
				{
					attachObj.mWallAttachedTo = WallSouth;
					attachObj.mFacing = LookingDirection.North;
				}
			break;
			
			case TilePositions.SouthEast:
				objPos += new Vector3(posFactor, 0 , -posFactor);
			break;
			
		case TilePositions.RandomPos:
			float posXRandom = Random.Range(-1f, 1f);
			float posYRandom = Random.Range(-1f, 1f);
			objPos += new Vector3(posFactor * posXRandom, 0 , -posFactor * posYRandom);
			break;
			
			case TilePositions.Middle:
				objPos += new Vector3(0, 0 , 0);
			break;
		}
		
		// Place in world and attach to correct anchor
		switch(objType)
		{
		case ObjectTypes.Decals:
			tempObj.transform.localPosition = objPos;
			tempObj.transform.parent = mLevelPartBelongingTo.mDecalObjectsAnchor;
			break;
		case ObjectTypes.Items:
			if(mItemPlaced)
				DestroyImmediate(mItemPlaced.gameObject);
			ItemObject itemObj = tempObj.GetComponent<ItemObject>();
			itemObj.mTilePlacedOn = this;
			tempObj.transform.localPosition = objPos;
			tempObj.transform.parent = mLevelPartBelongingTo.mItemObjectsAnchor;
			mItemPlaced = tempObj.GetComponent<ItemObject>();
			break;
		case ObjectTypes.Triggerables:
			tempObj.transform.localPosition = objPos;
			tempObj.transform.parent = mLevelPartBelongingTo.mTriggableListAnchor;
			break;
		case ObjectTypes.Enemies:
			tempObj.transform.localPosition = objPos;
			tempObj.transform.parent = mLevelPartBelongingTo.mEnemiesAnchor;
			tempObj.GetComponent<MovableActor>().mStartingTile = this;
			break;
		}

		// add to AttachedObjects List
		AttachedObjects.Add(tempObj);
	}
	
	/// <summary>
	/// Sets the texture of the specified wall 
	/// </summary>
	/// <param name='wall'>
	/// Target wall (N, S, W, E)
	/// </param>
	/// <param name='tex'>
	/// Specified Texture
	/// </param>
	public void SetWallTexture(int wall, int tex)
	{
		switch(wall)
		{
			case (int)LookingDirection.North:
				mWallNorth.SetTexture(tex);
			break;
			case (int)LookingDirection.South:
				mWallSouth.SetTexture(tex);
			break;
			case (int)LookingDirection.West:
				mWallWest.SetTexture(tex);
			break;
			case (int)LookingDirection.East:
				mWallEast.SetTexture(tex);
			break;
			case 4: //ceiling
				mCeiling.SetTexture(tex);
			break;
			case 5: //floor
				mFloor.SetTexture(tex);
			break;
		}
		
	}
	
	public void SetWallPassable(LookingDirection wall, bool pas)
	{
		switch(wall)
		{
			case LookingDirection.North:
				WallNorth.SetPassabble(pas);
			break;
			case LookingDirection.South:
				WallSouth.SetPassabble(pas);
			break;
			case LookingDirection.West:
				WallWest.SetPassabble(pas);
			break;
			case LookingDirection.East:
				WallEast.SetPassabble(pas);
			break;
		}
	}
	
	/*public void UseWall(LookingDirection wall)
	{
		switch(wall)
		{
			case LookingDirection.North:
				WallNorth.OnUse();
			break;
			case LookingDirection.South:
				WallSouth.OnUse();
			break;
			case LookingDirection.West:
				WallWest.OnUse();
			break;
			case LookingDirection.East:
				WallEast.OnUse();
			break;
		}
	}*/
	
	// Update is called once per frame
	void Update () 
	{
	}
}
