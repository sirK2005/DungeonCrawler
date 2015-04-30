using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum ObjectTypes
{
	Decals,
	Items,
	Triggerables,
	Enemies
}

public enum LookingDirection
{
	North = 0,
	East = 1,
	South = 2,
	West = 3,
}

[ExecuteInEditMode]
public class World : LevelPart {
	
	public const int TILE_SIZE = 20;
	public const int TILE_HEIGHT = 15;

	public LookingDirection mStartingDirection;
	// tile prefab
	public GameObject mLevelTilePrefab;
	
	// RESSOURCES //
	// tile objects prefabs list
	public List<GameObject> RoomPrefabList;
	public List<string> TileSetList;
	public int activeTileset = -1;
	public List<GameObject> mDecalsPrefabList;
	public string[] mDecalsPrefabNameList;
	public List<GameObject> mItemsPrefabList;
	public string[] mItemsPrefabNameList;
	public List<GameObject> mTriggerablesPrefabList;
	public string[] mTriggerablesPrefabNameList;
	public List<GameObject> mEnemiesPrefabList;
	public string[] mEnemiesPrefabNameList;
	
	public List<Material> mMaterialList;
	public string[] mMaterialNameList;
	
	private static World _instance;
	public static World Instance { get {return _instance;} }
	
	public string mLevelName;
	
	void OnLevelWasLoaded(int level) {
		// Create a random level, if we are not in the tech demo level
		if(Application.loadedLevelName.Equals("GenericLevel"))
		{
			CreateRandomLevel();
		}

		Player.Instance.PlayerInitOnLevelLoad();
    }
	
	void Awake()
	{
		_instance = this;
		
		// Load Ressource Lists
		// Decals
		RoomPrefabList = new List<GameObject>();
		foreach(GameObject obj in Resources.LoadAll("Rooms"))
			RoomPrefabList.Add(obj);

		mDecalsPrefabList = new List<GameObject>();
		foreach(GameObject obj in Resources.LoadAll("TilePrefabs/Decals"))
			mDecalsPrefabList.Add(obj);
		
		mDecalsPrefabNameList = new string[mDecalsPrefabList.Count];
		for(int count = 0; count < mDecalsPrefabList.Count; count++)
			mDecalsPrefabNameList[count] = mDecalsPrefabList[count].name;
		
		// Items
		mItemsPrefabList = new List<GameObject>();
		foreach(GameObject obj in Resources.LoadAll("TilePrefabs/ItemObjects"))
			mItemsPrefabList.Add(obj);
		
		mItemsPrefabNameList = new string[mItemsPrefabList.Count];
		for(int count = 0; count < mItemsPrefabList.Count; count++)
			mItemsPrefabNameList[count] = mItemsPrefabList[count].name;
		
		// Triggerables
		mTriggerablesPrefabList = new List<GameObject>();
		foreach(GameObject obj in Resources.LoadAll("TilePrefabs/TriggerLogic"))
			mTriggerablesPrefabList.Add(obj);
		
		mTriggerablesPrefabNameList = new string[mTriggerablesPrefabList.Count];
		for(int count = 0; count < mTriggerablesPrefabList.Count; count++)
			mTriggerablesPrefabNameList[count] = mTriggerablesPrefabList[count].name;
		
		// Enemies
		mEnemiesPrefabList = new List<GameObject>();
		foreach(GameObject obj in Resources.LoadAll("TilePrefabs/Enemies"))
			mEnemiesPrefabList.Add(obj);
		
		mEnemiesPrefabNameList = new string[mEnemiesPrefabList.Count];
		for(int count = 0; count < mEnemiesPrefabList.Count; count++)
			mEnemiesPrefabNameList[count] = mEnemiesPrefabList[count].name;

		// TileSets
		TileSetList = new List<string>();
		foreach(string direct in Directory.GetDirectories(Application.dataPath + "/Resources/Materials/Walls/"))
			TileSetList.Add(direct.Substring(direct.LastIndexOf("/")+1));

		if(activeTileset == -1)
			activeTileset = 0;

		// Materials
		mMaterialList = new List<Material>();
		foreach (Material mat in Resources.LoadAll("Materials/Walls/" + TileSetList[activeTileset] + "/"))
		{
			mMaterialList.Add(mat);
		}
		
		mMaterialNameList = new string[mMaterialList.Count];
		for(int count = 0; count < mMaterialList.Count; count++)
		{
			mMaterialNameList[count] = mMaterialList[count].name;
		}
	}

	public void setTileSet(int tileset)
	{
		mMaterialList = new List<Material>();
		foreach (Material mat in Resources.LoadAll("Materials/Walls/" + TileSetList[tileset] + "/"))
		{
			mMaterialList.Add(mat);
		}
		
		mMaterialNameList = new string[mMaterialList.Count];
		for(int count = 0; count < mMaterialList.Count; count++)
		{
			mMaterialNameList[count] = mMaterialList[count].name;
		}
	}

	void Start()
	{
		_instance = this;
		
		UpdateLevelTileList();
		
		if(GameMainHUDScreen.Instance)
			GameMainHUDScreen.Instance.AddHistoryText(mLevelName);
		string welcome = 	"Welcome Traveler, \n" + 
							"This is a TechDemo of my upcoming Game " +
							"-- Learn To Crawl --" +
#if UNITY_STANDALONE
							"CONTROLS: \n" +
							"A - Turn Left | W - Move Forward | D - Turn Right | S - Turn Backward \n" +
#endif
							"So feel free to explore. \n \n" +
							"News: " +
							"Version 0.0.321a \n" +
							"- major player Input-Handling code improvements \n" +
							"- Portals! No longer simple linear ways, yay. \n" +
							"Have Fun!";
		if(GameMainHUDScreen.Instance)
			GameMainHUDScreen.Instance.ShowTextWindow(welcome);
	}

	/// <summary>
	/// Adds a Room to the world
	/// </summary>
	/// <returns><c>true</c>, if room could be added, <c>false</c> if world space is, at least partially, occupied</returns>
	/// <param name="objRoom">Object room.</param>
	/// <param name="pos">Position.</param>
	/// <param name="dir">Dir.</param>
	public bool AddRoom(GameObject objRoom, Vector2 pos, LookingDirection dir)
	{
		// create instance
		GameObject newRoomObj = Instantiate(objRoom) as GameObject;
		LevelRoom newRoom = newRoomObj.GetComponent<LevelRoom>();

		// rotate to correct orientation
		for(int i = 0; i < (int) dir; i++)
			newRoom.Rotate();
		
		// test if there is enough space for the room in worldspace
		LevelTile testTile;
		foreach(LevelTile tile in newRoom.mLevelTileList)
		{
			testTile = GetTileByPos(pos + tile.Pos);
			if(testTile != null)
			{
				// delete level room transform
				if(Application.isEditor)
					DestroyImmediate(newRoom.gameObject);
				else
					Destroy(newRoom.gameObject);

				return false;
			}
		}

		// add tile to correct position in worldspace, and add to world tile list
		foreach(LevelTile tile in newRoom.mLevelTileList)
		{
			mLevelTileList.Add(tile);
			tile.Reposition(pos + tile.Pos, this, true);
		}

		// rearrange gameobjects to world object
		List<Transform> transList = new List<Transform>();

		// Tiles
		foreach(Transform trans in newRoom.mTilesAnchor)
			transList.Add(trans);
		for(int i = 0; i < transList.Count; i++)
		{
			transList[i].parent = World.Instance.mTilesAnchor;
		}
		transList.Clear();

		// Decals
		foreach(Transform trans in newRoom.mDecalObjectsAnchor)
			transList.Add(trans);
		for(int i = 0; i < transList.Count; i++)
		{
			transList[i].parent = World.Instance.mDecalObjectsAnchor;
		}
		transList.Clear();

		// Triggerables
		foreach(Transform trans in newRoom.mTriggableListAnchor)
			transList.Add(trans);
		for(int i = 0; i < transList.Count; i++)
		{
			transList[i].parent = World.Instance.mTriggableListAnchor;
		}
		transList.Clear();

		// Enemies
		foreach(Transform trans in newRoom.mEnemiesAnchor)
			transList.Add(trans);
		for(int i = 0; i < transList.Count; i++)
		{
			transList[i].parent = World.Instance.mEnemiesAnchor;
		}
		transList.Clear();

		// Items
		foreach(Transform trans in newRoom.mItemObjectsAnchor)
			transList.Add(trans);
		for(int i = 0; i < transList.Count; i++)
		{
			transList[i].parent = World.Instance.mItemObjectsAnchor;
		}

		// delete level room transform
		if(Application.isEditor)
			DestroyImmediate(newRoom.gameObject);
		else
			Destroy(newRoom.gameObject);

		//finish up
		// set connectiong wall to not existent
		GetTileByPos(pos).SetWallStatus(GetOppositeDirection(dir), false);

		return true;
	}

	/// <summary>
	/// Gets the opposite looking direction.
	/// </summary>
	/// <returns>
	/// The opposite direction.
	/// </returns>
	/// <param name='direc'>
	/// Direction whose opposite side needs to be calculated
	/// </param>
	public static LookingDirection GetOppositeDirection(LookingDirection direc)
	{
		return (LookingDirection)(((int)direc + 2) % 4);
	}

	/// <summary>
	/// Gets the opposite looking direction.
	/// </summary>
	/// <returns>
	/// The opposite direction.
	/// </returns>
	/// <param name='direc'>
	/// Direction whose opposite side needs to be calculated
	/// </param>
	public static LookingDirection GetOppositeDirection(int dirInt)
	{
		LookingDirection direc = (LookingDirection) dirInt;
		return (LookingDirection)(((int)direc + 2) % 4);
	}
	
	/// <summary>
	/// Gets looking direction to the side of given direction.
	/// </summary>
	/// <returns>
	/// The direction at side.
	/// </returns>
	/// <param name='direc'>
	/// direction to calculate side from.
	/// </param>
	/// <param name='side'>
	/// true - calculate late right side of direc, false - calculate left side of direc
	/// </param>
	public static LookingDirection GetDirectionAtSide(LookingDirection direc, bool side)
	{
		if(side) //rotate right
		{
			return (LookingDirection) (((int)direc + 1) % 4);
		}
		else // rotate left
		{
			return (LookingDirection) ((4 + (int)direc - 1) % 4);
		}
	}

	/// <summary>
	/// Gets looking direction to the side of given direction.
	/// </summary>
	/// <returns>
	/// The direction at side.
	/// </returns>
	/// <param name='direc'>
	/// direction to calculate side from.
	/// </param>
	/// <param name='side'>
	/// true - calculate late right side of direc, false - calculate left side of direc
	/// </param>
	public static LookingDirection GetDirectionAtSide(int dirInt, bool side)
	{
		LookingDirection direc = (LookingDirection) dirInt;
		if(side) //rotate right
		{
			return (LookingDirection) (((int)direc + 1) % 4);
		}
		else // rotate left
		{
			return (LookingDirection) ((4 + (int)direc - 1) % 4);
		}
	}

	public Vector2 DirectionToVector(LookingDirection dir)
	{
		if(dir == LookingDirection.North)
			return new Vector2(0, 1);
		if(dir == LookingDirection.East)
			return new Vector2(1, 0);
		if(dir == LookingDirection.South)
			return new Vector2(0, -1);
		return new Vector2(-1, 0); // West
	}

	public Vector2 DirectionToVector(int dirInt)
	{
		LookingDirection dir = (LookingDirection) dirInt;

		if(dir == LookingDirection.North)
			return new Vector2(0, 1);
		if(dir == LookingDirection.East)
			return new Vector2(1, 0);
		if(dir == LookingDirection.South)
			return new Vector2(0, -1);
		return new Vector2(-1, 0); // West
	}

	public LookingDirection MirrorDirection(LookingDirection dir, int axis)
	{
		if(axis == 0) //X-axis
		{
			if(dir == LookingDirection.North)
				return LookingDirection.South;
			if(dir == LookingDirection.South)
				return LookingDirection.North;
		}

		if(axis == 1) //Y-Axis
		{
			if(dir == LookingDirection.East)
				return LookingDirection.West;
			if(dir == LookingDirection.West)
				return LookingDirection.East;
		}

		return dir;
	}

	public LookingDirection MirrorDirection(int dirInt, int axis)
	{
		LookingDirection dir = (LookingDirection) dirInt;
		if(axis == 0 || axis == 2) //X-axis
		{
			if(dir == LookingDirection.North)
				return LookingDirection.South;
			if(dir == LookingDirection.South)
				return LookingDirection.North;
		}
		
		if(axis == 1 || axis == 2) //Y-Axis
		{
			if(dir == LookingDirection.East)
				return LookingDirection.West;
			if(dir == LookingDirection.West)
				return LookingDirection.East;
		}

		return dir;
	}

	void CreateRandomLevel ()
	{
		//ToDo
		mStartingTile = CreateTile(new Vector2(0,0), null);

		mStartingTile.WallNorth.SetTexture(0);
		mStartingTile.WallEast.SetTexture(0);
		mStartingTile.WallSouth.SetTexture(0);
		mStartingTile.WallWest.SetTexture(0);
		mStartingTile.Ceiling.SetTexture(1);
		mStartingTile.Floor.SetTexture(2);


		LevelTile tempTile = null;
		LevelTile tile = mStartingTile;
		LevelTile mirrorTile = mStartingTile;

		// Hallway type
		int hallways = Random.Range(0, 4) + 5;
		int hallWayLength = 0;
		int dir = 0;
		int mirrorDir = 0;
		int mirrorAxis = -1; // 0 - X, 1 - Y

		// create randomed amount of hallways
		for(int hallwayCnt = 0; hallwayCnt < hallways; hallwayCnt++)
		{
			Debug.Log("Hallways:" + hallways + "/Cnt:" + hallwayCnt);

			// create randomed length of current hallway
			hallWayLength = Random.Range(0,7) + 3;
			dir = (int)GetDirectionAtSide(dir, Random.Range(0,2) == 0);
			if(mirrorAxis == -1)
				mirrorAxis = dir % 2;
			mirrorDir = (int) MirrorDirection(dir, mirrorAxis);

			for(int wayLengthCnt = 0; wayLengthCnt < hallWayLength; wayLengthCnt++)
			{
				tempTile = CreateTileInDirection((LookingDirection)dir, tile);
				if(tempTile != null)
					tile = tempTile;

				// mirror hallway
				tempTile = CreateTileInDirection((LookingDirection)mirrorDir, mirrorTile);
				if(tempTile != null)
					mirrorTile = tempTile;

				Debug.Log(dir + "/" + mirrorDir);

				// random if we try to make a new room
				int newRoom = Random.Range(0,3);
				if(newRoom == 0)
				{
					bool side = (Random.Range(0,2) == 0);
					int roomDir = (int)GetDirectionAtSide(dir, side);
					int randRoom = Random.Range(0,RoomPrefabList.Count);
					AddRoom(RoomPrefabList[randRoom], tile.Pos + DirectionToVector(roomDir), (LookingDirection) roomDir);

					//mirror room
					int roomDirMirror = (int)MirrorDirection(roomDir, mirrorAxis);
					AddRoom(RoomPrefabList[randRoom], mirrorTile.Pos + DirectionToVector(roomDirMirror), (LookingDirection) roomDirMirror);
				}
			}
		}

		/*for(int i = 0; i < 100; i++)
		{
			if(Random.Range(0,10) == 1)
			{
				if(tile == null)
					continue;

				int dir = Random.Range(0,4);
				int room = Random.Range(0,RoomPrefabList.Count);
				AddRoom(RoomPrefabList[room], tile.Pos + DirectionToVector((LookingDirection) dir), (LookingDirection) dir);
			}
			else
			{
				tempTile = CreateTileInDirection((LookingDirection)Random.Range(0,4), tile);
				if(tempTile != null)
					tile = tempTile;
			}
		}*/
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetInstacne()
	{
		Debug.Log ("Done");
		if(_instance == null)
			_instance = this;
	}
}
