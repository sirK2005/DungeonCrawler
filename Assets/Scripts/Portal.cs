using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	
	public LevelTile mGoalTile;
	public LookingDirection mPortDirection;
	public bool mUsePlayerLookingDirection;
	
	private void PortPlayer()
	{
		if(!mUsePlayerLookingDirection)
			Player.Instance.PlaceOnTile(mGoalTile, mPortDirection);
		else
			Player.Instance.PlaceOnTile(mGoalTile, Player.Instance.Facing);
			
	}
}
