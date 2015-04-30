using UnityEngine;
using System.Collections;

public enum ActorType
{
	Enemy,
	Player
}

public class MovableActor : MonoBehaviour {
	
	public bool mTriggersObjects;
	public LevelTile mStartingTile;
	private ActorType mType;
	public ActorType Type {get {return mType;} set{mType = value;}}
	private bool mDead;
	public bool Dead { get {return mDead;} set {mDead = value;}}
	
	// World Placement related Variables
	private LookingDirection mFacing;
	public LookingDirection Facing { get {return mFacing;} set {mFacing = value;}}
	private LevelTile mTilePlacedOn;
	public LevelTile TilePlacedOn { get {return mTilePlacedOn;} set{mTilePlacedOn = value;}}
	private Transform mWorldPos;
	public Transform WorldPos { get {return mWorldPos;} set {mWorldPos = value;} }
	public float mPosOffsetX;
	public float mPosOffsetY;
	public float mPosOffsetZ;
	
	// Tile Transition Variables
	private LevelTile mGoalTile;
	private float mTransitionSpeed;
	private Vector3 mGoalPos;
	private Vector3 mLastPos;
	private LookingDirection mGoalFacing;
	private bool mInTransition;
	public bool InTransition {get {return mInTransition;}}
	private float mTransitionTime;
	private bool mInTransRotate;
	private bool mRotateTransDirec;
	
	// Actor Stats
	public int mElementalDamage;
	public int mMagicDamage;
	private int mHealth;
	public int Health {get {return mHealth;} set {mHealth = value;}}
	public int mMaxHealth;
	public int MaxHealth { get {return mMaxHealth;} set {mMaxHealth = value;}} 
	
	void Awake()
	{
		InitializeActor();
	}
	
	public virtual void InitializeActor()
	{
		mWorldPos = transform;
		mFacing = LookingDirection.North;
		
		mPosOffsetX = mPosOffsetZ = 0;
		mPosOffsetY = 7.5f;
		
		mInTransition = false;
		mInTransRotate = false;
		mRotateTransDirec = false;
		mTransitionSpeed = 2.5f;
		
		mHealth = mMaxHealth;
		mDead = false;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		UpdateActor();
	}
	
	public virtual void UpdateActor()
	{
		// Test for and apply Tile-transition
		if(mInTransition)
		{
			if(!mInTransRotate)
			{
				mWorldPos.position += (mGoalPos - mLastPos) * Time.deltaTime * mTransitionSpeed;
			}
			else
			{
				if(mRotateTransDirec)
					mWorldPos.RotateAround(mTilePlacedOn.transform.position, Vector3.up, Time.deltaTime * 90 * mTransitionSpeed);
				else
					mWorldPos.RotateAround(mTilePlacedOn.transform.position, Vector3.down, Time.deltaTime * 90 * mTransitionSpeed);
			}
			
			mTransitionTime += Time.deltaTime * mTransitionSpeed;
			
			if(mTransitionTime > 1f)
			{
				PlaceOnTile(mGoalTile, mGoalFacing);
				
				mInTransition = false;
				mInTransRotate = false;
				mTransitionTime = 0;
			}
		}
	}
	
	public virtual void AdjustRotation(LookingDirection direc)
	{
		mWorldPos.transform.eulerAngles = new Vector3(0, (int)direc * 90, 0);
	}
	
	public virtual void UpdateMap()
	{
		LevelTile tempTile;
		if(!mTilePlacedOn.WallNorthActive){
			if(mTilePlacedOn.NeighborNorth)
			{
				tempTile = mTilePlacedOn.NeighborNorth;
				GameMainHUDScreen.Instance.AddMapTile((int)tempTile.transform.position.x / 20, (int)tempTile.transform.position.z / 20, tempTile.WallNorthActive, tempTile.WallEastActive, tempTile.WallSouthActive, tempTile.WallWestActive);  
			}
		}
		if(!mTilePlacedOn.WallEastActive){
			if(mTilePlacedOn.NeighborEast)
			{
				tempTile = mTilePlacedOn.NeighborEast;
				GameMainHUDScreen.Instance.AddMapTile((int)tempTile.transform.position.x / 20, (int)tempTile.transform.position.z / 20, tempTile.WallNorthActive, tempTile.WallEastActive, tempTile.WallSouthActive, tempTile.WallWestActive);  
			}
		}
		if(!mTilePlacedOn.WallSouthActive){
			if(mTilePlacedOn.NeighborSouth)
			{
				tempTile = mTilePlacedOn.NeighborSouth;
				GameMainHUDScreen.Instance.AddMapTile((int)tempTile.transform.position.x / 20, (int)tempTile.transform.position.z / 20, tempTile.WallNorthActive, tempTile.WallEastActive, tempTile.WallSouthActive, tempTile.WallWestActive);  
			}
		}
		if(!mTilePlacedOn.WallWestActive){
			if(mTilePlacedOn.NeighborWest)
			{
				tempTile = mTilePlacedOn.NeighborWest;
				GameMainHUDScreen.Instance.AddMapTile((int)tempTile.transform.position.x / 20, (int)tempTile.transform.position.z / 20, tempTile.WallNorthActive, tempTile.WallEastActive, tempTile.WallSouthActive, tempTile.WallWestActive);  
			}
		}
		
		tempTile = mTilePlacedOn;
		GameMainHUDScreen.Instance.AdjustMapPosToPos((int)tempTile.transform.position.x / 20, (int)tempTile.transform.position.z / 20);
		
		// We may have to adjust player map looking direction:
		GameMainHUDScreen.Instance.AdjustMapPlayerDirec(mGoalFacing);
	}
	
	public virtual void DealDamage(MovableActor targetActor, LookingDirection direction)
	{
		targetActor.ReceiveDamage(mElementalDamage, mMagicDamage, direction);
	}
	
	public virtual void AttackVisualistion()
	{
		// must be done in a derived class
		return;
	}
	
	public virtual void ReceiveDamage(int elementalDmg, int magicalDmg, LookingDirection direction)
	{
		// Damage taken Animation
		mHealth -= elementalDmg + magicalDmg;
		if(mHealth <= 0)
			Die();
	}
	
	public virtual void Die()
	{
		//die animation
		Destroy(gameObject);
	}
	
	/// <summary>
	/// Move actor along direction
	/// </summary>
	/// <param name='movForward'>
	/// True - Move Player forward, else move backwards
	/// </param>
	public void move(bool movForward)
	{
		Vector3 distanceMoved = new Vector3(0,0,0);		// hold how far player is supposed to move
		LookingDirection direc;							// hold direction we want to move
		int movAmount = 1;								// factor that effects result of distanceMoved
		
		// get moving direction
		if(movForward)
		{
			direc = mFacing;
		}
		else
		{
			direc = World.GetOppositeDirection(mFacing);
		}
		
		
		// calculate distanceMoved
		switch(direc)
		{
			case LookingDirection.North:
				distanceMoved.z += movAmount;
				if(mTilePlacedOn != null)
				{
					if(!mTilePlacedOn.WallNorthPassable)
					{
						distanceMoved = Vector3.zero;
					}
					else
					{
						// get new tile
						if(mTilePlacedOn.NeighborNorth)
						{
							mGoalTile = mTilePlacedOn.NeighborNorth;
						}
					}
				}
			break;
			case LookingDirection.South:
				distanceMoved.z -= movAmount;
				if(mTilePlacedOn != null)
				{
					if(!mTilePlacedOn.WallSouthPassable)
					{
						distanceMoved = Vector3.zero;
					}
					else
					{
						// get new tile
						if(mTilePlacedOn.NeighborSouth)
						{
							mGoalTile = mTilePlacedOn.NeighborSouth;
						}
					}
				}
				
			break;
			case LookingDirection.East:
				distanceMoved.x += movAmount;
				if(mTilePlacedOn != null)
				{
					if(!mTilePlacedOn.WallEastPassable)
					{
						distanceMoved = Vector3.zero;
					}
					else
					{
						// get new tile
						if(mTilePlacedOn.NeighborEast)
						{
							mGoalTile = mTilePlacedOn.NeighborEast;
						}
					}
				}
				
			break;
			case LookingDirection.West:
				distanceMoved.x -= movAmount;
				if(mTilePlacedOn != null)
				{
					if(!mTilePlacedOn.WallWestPassable)
					{
						distanceMoved = Vector3.zero;
					}
					else
					{
						// get new tile
						if(mTilePlacedOn.NeighborWest)
						{
							mGoalTile = mTilePlacedOn.NeighborWest;
						}
					}
				}
				
			break;
			
			default:
				Debug.LogWarning("Something went wrong moving.");
			break;
		}
		
		// check if occupied by other actors
		if(mGoalTile)
			if(mGoalTile.ActorOnTile != null)
				distanceMoved = Vector3.zero;
			
		// check if we actually moved
		if(distanceMoved != Vector3.zero)
		{
			mGoalPos = mWorldPos.position + distanceMoved * World.TILE_SIZE; // apply vector
			mLastPos = mWorldPos.position;
			mInTransition = true;
			mGoalFacing = mFacing;
		}
	}
	
	/// <summary>
	/// Rotates the actor camera either left or right
	/// </summary>
	/// <param name='turnRight'>
	/// True - rotate Camera to the right, false - rotate camera to the left
	/// </param>
	public void turn(bool turnRight)
	{
		mRotateTransDirec = turnRight;
		mInTransition = mInTransRotate = true;
		mGoalTile = mTilePlacedOn;
		//rotate looking direction
		if(turnRight)
			mGoalFacing = World.GetDirectionAtSide(mFacing, true);
		else
			mGoalFacing = World.GetDirectionAtSide(mFacing, false);
	}
	
	public void PlaceOnTile(LevelTile tile, LookingDirection goalDirec)
	{
		if(mTilePlacedOn)
			mTilePlacedOn.ActorOnTile = null; 	// tile before we moved
		mTilePlacedOn = tile;				// tile we moved to
		mTilePlacedOn.ActorOnTile = this;
		
		switch(goalDirec)
		{
		case LookingDirection.East:
			mWorldPos.position = mTilePlacedOn.transform.localPosition + new Vector3(mPosOffsetX, mPosOffsetY, 0);
			mFacing = LookingDirection.East;
			break;
			
		case LookingDirection.North:
			mWorldPos.position = mTilePlacedOn.transform.localPosition + new Vector3(0, mPosOffsetY, mPosOffsetZ);
			mFacing = LookingDirection.North;
			break;
			
		case LookingDirection.West:
			mWorldPos.position = mTilePlacedOn.transform.localPosition + new Vector3(-mPosOffsetX, mPosOffsetY, 0);
			mFacing = LookingDirection.West;
			break;
			
		case LookingDirection.South:
			mWorldPos.position = mTilePlacedOn.transform.localPosition + new Vector3(0, mPosOffsetY, -mPosOffsetZ);
			mFacing = LookingDirection.South;
			break;
		}
		
		AdjustRotation(goalDirec);
		
		//Map Update
		UpdateMap();
		
		// we moved so ask every with player collidable object, if player
		// collides with it
		if(mTriggersObjects)
		{
			if(!mInTransRotate)
			{
				foreach(GameObject obj in GameObject.FindGameObjectsWithTag("ReceivePlayerMovedEvent"))
				{
					obj.SendMessage("OnPlayerMoved", SendMessageOptions.DontRequireReceiver);
				}
			}
			
			
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("ReceivePlayerActedEvent"))
			{
				obj.SendMessage("OnPlayerActed", SendMessageOptions.DontRequireReceiver);
			}
		}
		
		mGoalTile = null;
	}
}
