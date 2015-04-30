using UnityEngine;
using System.Collections;

public class ItemObject : MonoBehaviour {
	
	public string mItemName;
	public LevelTile mTilePlacedOn;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void PickUp()
	{
		Player.Instance.ItemHeld = new ItemInventory(mItemName); 
		mTilePlacedOn.ItemPlaced = null;
		Destroy(this.gameObject);
	}
	
}
