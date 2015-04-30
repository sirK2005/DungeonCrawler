using UnityEngine;
using System.Collections;

public class Enemy : MovableActor {
	
	public bool mRoaming;
	
	public Animated mIdleAnimation;
	public Animated mAttackAnimation;
	public Animated mDieAnimation;
	
	private bool mAttacking;
	
	private LookingDirection mAttackDirection;
	
	// Use this for initialization
	void Start () {
	
	}
	
	private void OnPlayerActed()
	{
		EnemyUpdate();
	}
	
	public override void InitializeActor ()
	{
		MaxHealth = 10;
		Type = ActorType.Enemy; 
		
		base.InitializeActor ();
		
		TilePlacedOn = mStartingTile;
		PlaceOnTile(TilePlacedOn, Facing);
	}
	
	public override void UpdateActor ()
	{
		base.UpdateActor ();
		
		if(!mIdleAnimation.Running){
			if(mAttacking && !mAttackAnimation.Running)
			{
				mIdleAnimation.StartAnimation();
				base.DealDamage (Player.Instance, mAttackDirection);
				Player.Instance.InteractionBlocked = false;
				mAttacking = false;
			}
				
			
			if(Dead && !mDieAnimation.Running){
				base.Die();
			}
		}
		
	}
	
	public override void AdjustRotation (LookingDirection direc)
	{
		// do nothing
		return;
	}
	
	public override void UpdateMap ()
	{
		// do nothing;
		return;
	}
	
	public override void Die ()
	{
		mIdleAnimation.StopAnimation();
		mDieAnimation.StartAnimation();
		Dead = true;
	}
	
	public override void DealDamage (MovableActor targetActor, LookingDirection direction)
	{
		Player.Instance.InteractionBlocked = true;
		mAttacking = true;
		mAttackDirection = direction;
		AttackVisualistion();
	}
	
	public override void AttackVisualistion ()
	{
		mIdleAnimation.StopAnimation();
		mAttackAnimation.StartAnimation();
	}
	
	private void AttackedByPlayer()
	{
		if(Dead)
			return;
		
		Player.Instance.DealDamage(this, Player.Instance.Facing);
		Player.Instance.SendPlayerActed();
	}
	
	/// <summary>
	/// Controls enemy behaviour. Is Called after Player movement 
	/// </summary>
	private void EnemyUpdate()
	{
		if(Dead)
			return;
		
		LookingDirection attackDirection = LookingDirection.North;
		
		//Check if player is standing next to enemy
		bool playerClose = false;
		
		if(TilePlacedOn.NeighborNorth)
			if(TilePlacedOn.WallNorthPassable)
				if(TilePlacedOn.NeighborNorth.ActorOnTile)
					if(TilePlacedOn.NeighborNorth.ActorOnTile.Type == ActorType.Player)
					{
						playerClose = true;
						attackDirection = (LookingDirection) 0;
					}
		if(TilePlacedOn.NeighborEast)
			if(TilePlacedOn.WallEastPassable)
				if(TilePlacedOn.NeighborEast.ActorOnTile)
					if(TilePlacedOn.NeighborEast.ActorOnTile.Type == ActorType.Player)
						{
						playerClose = true;
						attackDirection = (LookingDirection) 1;
						}
		if(TilePlacedOn.NeighborSouth)
			if(TilePlacedOn.WallSouthPassable)
				if(TilePlacedOn.NeighborSouth.ActorOnTile)
					if(TilePlacedOn.NeighborSouth.ActorOnTile.Type == ActorType.Player)
						{
						playerClose = true;
						attackDirection = (LookingDirection) 2;
						}
		if(TilePlacedOn.NeighborWest)
			if(TilePlacedOn.WallWestPassable)
				if(TilePlacedOn.NeighborWest.ActorOnTile)
					if(TilePlacedOn.NeighborWest.ActorOnTile.Type == ActorType.Player)
						{
						playerClose = true;
						attackDirection = (LookingDirection) 3;
						}
		
		if(playerClose)
		{
			DealDamage(Player.Instance, attackDirection);
			return;
		}
		
		//Pick Random Direction;
		int rndDirec = Random.Range(0,4);
		Facing = (LookingDirection) rndDirec;
		
		// and move in that direction
		if(mRoaming)
			move(true);
	}
}
