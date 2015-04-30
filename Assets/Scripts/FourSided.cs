using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Sides
{
	Front = 0,
	Back = 1,
	Left = 2,
	Right = 3,
}

public class FourSided : MonoBehaviour {
	
	public LookingDirection mFacingDirection;
	public string mTexBaseName;
	public List<Material> mMaterialList;
	public GameObject mTexGoal;
	public Sides mSideShown;
	
	private Vector3 mOrigPos;
	
	private List<string> mFacingOrder; //returns string that represents side to show according to player looking direction
	
	void Awake()
	{
		Material tempMat;
		mMaterialList = new List<Material>();
		tempMat = Resources.Load("Materials/" + mTexBaseName + "_Front") as Material;
		mMaterialList.Add(tempMat);
		tempMat = Resources.Load("Materials/" + mTexBaseName + "_Back") as Material;
		mMaterialList.Add(tempMat);
		tempMat = Resources.Load("Materials/" + mTexBaseName + "_Left") as Material;
		mMaterialList.Add(tempMat);
		tempMat = Resources.Load("Materials/" + mTexBaseName + "_Right") as Material;
		mMaterialList.Add(tempMat);
		
		mFacingOrder = new List<string>();
		
		mOrigPos = mTexGoal.transform.localPosition;
		
		mFacingOrder.Add("Back");
		mFacingOrder.Add("Left");
		mFacingOrder.Add("Front");
		mFacingOrder.Add("Right");
	}
	
	public void SetFacing(LookingDirection direc)
	{
		mFacingDirection = direc;
	}
	
	// Use this for initialization
	void Start () {
		AttachedToWall obj = GetComponent<AttachedToWall>();
		if(obj)
			mFacingDirection = obj.mFacing;
		
		// adjust Facing Order in accordance to own facing
		for(int count = 0; count < (int)mFacingDirection; count++)
		{
			mFacingOrder.Insert(0, mFacingOrder[3]);
			mFacingOrder.RemoveAt(4);
		}
	}
	
	private void ActivateSide(string side)
	{
		switch(side)
		{
			case "Left":
				mTexGoal.renderer.sharedMaterial = mMaterialList[(int)Sides.Left];
				mTexGoal.transform.localPosition = mOrigPos + new Vector3(-5,0,0);
				mSideShown = Sides.Left;
			break;
			
			case "Right":
				mTexGoal.renderer.sharedMaterial = mMaterialList[(int)Sides.Right];
				mTexGoal.transform.localPosition = mOrigPos + new Vector3(5,0,0);
				mSideShown = Sides.Right;
			break;
			
			case "Front":
				mTexGoal.renderer.sharedMaterial = mMaterialList[(int)Sides.Front];
				mTexGoal.transform.localPosition = mOrigPos;
				mSideShown = Sides.Front;
			break;
			
			case "Back":
				mTexGoal.renderer.sharedMaterial = mMaterialList[(int)Sides.Back];
				mTexGoal.transform.localPosition = mOrigPos + new Vector3(0,0,2);
				mSideShown = Sides.Back;
			break;
		}
	}
	
	void Update()
	{
		// change tex according to player facing
		float rotY = Player.Instance.mPlayerCamera.transform.eulerAngles.y;
		if((rotY >= 0) && (rotY < 45))
			ActivateSide(mFacingOrder[0]);
		if((rotY >= 315) && (rotY < 360))
			ActivateSide(mFacingOrder[0]);
		if((rotY >= 45) && (rotY < 135))
			ActivateSide(mFacingOrder[1]);
		if((rotY >= 135) && (rotY < 225))
			ActivateSide(mFacingOrder[2]);
		if((rotY >= 225) && (rotY < 315))
			ActivateSide(mFacingOrder[3]);
		
	}
}
