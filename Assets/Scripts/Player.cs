using UnityEngine;
using System.Collections;

public class Player : MovableActor {
	
	// Input Variables
	private bool mLockBtnInput;
	private bool mLockMouseInput;
	private bool mInteractionBlocked;
	public bool InteractionBlocked {get {return mInteractionBlocked;} set {mInteractionBlocked = value;}}
	
	// Camera Stuff
	public Camera mPlayerCamera;
	
	// Singleton
	private static Player mInstance;
	public static Player Instance { get { return mInstance;}}
	
	// Inventory
	private ItemInventory mItemHeld;
	public ItemInventory ItemHeld {get {return mItemHeld;} set {mItemHeld = value;}}
	
	// Player Stats
	// - ToDo -
	
	public override void InitializeActor ()
	{
		mInstance = this;
		DontDestroyOnLoad(gameObject);
		
		mLockBtnInput = false;
		mLockMouseInput = false;
		
		MaxHealth = 20;
		Type = ActorType.Player;
		
		base.InitializeActor ();
		
		WorldPos = transform;
		mPosOffsetY = 6;
		mPosOffsetX = mPosOffsetZ = -7;
		
		mElementalDamage = 2;
	}
	
	public void PlayerInitOnLevelLoad()
	{
		// place player on starting tile
		PlaceOnTile(World.Instance.mStartingTile, World.Instance.mStartingDirection);
		//mPlayerCamera.transform.localPosition = TilePlacedOn.transform.localPosition + new Vector3(0, mPosOffsetY, -mPosOffsetZ);

		LevelTile tile = TilePlacedOn;
		GameMainHUDScreen.Instance.AddMapTile((int)tile.transform.position.x / 20, (int)tile.transform.position.z / 20, tile.WallNorthActive, tile.WallEastActive, tile.WallSouthActive, tile.WallWestActive);  
		
		UpdateMap();	
	}
	
	public override void UpdateActor ()
	{
		base.UpdateActor ();
		
		if(Dead)
			return;
		
		// if no transition is supposed to happen, check for input
		if(!InTransition && !InteractionBlocked)
			FetchInput();
	}
	
	public override void ReceiveDamage (int elementalDmg, int magicalDmg, LookingDirection direction)
	{
		base.ReceiveDamage (elementalDmg, magicalDmg, direction);
		
		// get direction opposite of attacked direction and show on screen where dmg came from
		int facingDmgComesFrom = (4 + (int)World.GetOppositeDirection(direction) - (int)Facing) % 4;
		GameMainHUDScreen.Instance.SetDamageMarkerAlpha(facingDmgComesFrom , 1.0f);
		// calc new health bar fill
		GameMainHUDScreen.Instance.UpdateHealthBar(MaxHealth, Health);
		
	}
	
	public override void DealDamage (MovableActor targetActor, LookingDirection direction)
	{
		GameMainHUDScreen.Instance.StartAttackAnimation();
		base.DealDamage (targetActor, direction);
	}
	
	public override void Die()
	{
		// do nothing
		GameMainHUDScreen.Instance.AddHistoryText("You haven been killed");
		Dead = true;
		
		return;
	}
	
	private void FetchInput()
	{
		// Move Forward
		if(PlayerControl.INPUT_FORWARD && !mLockBtnInput)
		{
			// move forward
			move(true);
			
			mLockBtnInput = true;
		}
		else if(PlayerControl.INPUT_BACKWARD && !mLockBtnInput)
		{
			// move backward
			move(false);
			
			mLockBtnInput = true;
		}
		else if(PlayerControl.INPUT_LEFT && !mLockBtnInput)
		{
			// turn left
			turn(false);
			
			mLockBtnInput = true;
		}
		else if(PlayerControl.INPUT_RIGHT && !mLockBtnInput)
		{
			// turn right
			turn(true);
			
			mLockBtnInput = true;
		}
		else
		{
			// release lock
			mLockBtnInput = false;
		}
		
		// Fetch Mouse Input
		if(PlayerControl.INPUT_LMBCLICKED && !mLockMouseInput)
		{
			Ray rayMousePos = mPlayerCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayHit;
			if ( Physics.Raycast(rayMousePos, out rayHit) )
			{
				// get a usable component
				MonoBehaviour[] components =  rayHit.transform.GetComponents<MonoBehaviour>();
				Usable tempUsable = null;
				foreach(MonoBehaviour component in components)
				{
					tempUsable = component as Usable;
					if(tempUsable != null)
						break;
				}
				
				if(tempUsable != null)
					tempUsable.OnUse();
			}  
			
			//mLockMouseInput = true;
		}
		else
		{
			mLockMouseInput = false;
		}
	}
	
	public void SendPlayerActed()
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("ReceivePlayerActedEvent"))
		{
			obj.SendMessage("OnPlayerActed", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private float VectorDist(Vector3 vect)
	{
		return Mathf.Abs(vect.x + vect.y + vect.z);
	}
	
}
