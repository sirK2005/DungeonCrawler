using UnityEngine;
using System.Collections;

public class AttachedToWall : MonoBehaviour {
	
	public LevelWall mWallAttachedTo;
	public LookingDirection mFacing;

	// Rotates object facing and wall attached to
	public LevelWall Rotate(LevelTile tile)
	{
		if(mFacing == LookingDirection.North)
		{
			// object is placed at wallSouth
			mFacing = LookingDirection.East;
			mWallAttachedTo = tile.WallWest;
		}
		else if(mFacing == LookingDirection.East)
		{
			// object is placed at wallWest
			mFacing = LookingDirection.South;
			mWallAttachedTo = tile.WallNorth;
		}
		else if(mFacing == LookingDirection.South)
		{
			// object is placed at wallSouth
			mFacing = LookingDirection.West;
			mWallAttachedTo = tile.WallEast;
		}
		else if(mFacing == LookingDirection.West)
		{
			// object is placed at wallSouth
			mFacing = LookingDirection.North;
			mWallAttachedTo = tile.WallSouth;
		}

		return mWallAttachedTo;
	}
	
}
