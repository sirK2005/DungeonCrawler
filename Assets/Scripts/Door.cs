using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Door : MonoBehaviour {
	public GameObject mSide1;
	public GameObject mSide2;
	public AttachedToWall wallAttachObject;
	private LevelWall mWallAttachedTo;
	public bool mOpen;
	
	public string mMatOpenName;
	public string mMatClosedName;
	[SerializeField]
	private Material mMatOpen;
	[SerializeField]
	private Material mMatClosed;
	
	// Use this for initialization
	void Start () 
	{
		wallAttachObject = gameObject.GetComponent<AttachedToWall>();
		mWallAttachedTo = wallAttachObject.mWallAttachedTo;
	
		if(mWallAttachedTo)
			mWallAttachedTo.SetPassabble(mOpen);
		
		if(mMatOpen == null)
		{
			SetMaterials("Wall_Stone_Door");
		}
		
		GetComponent<Triggerable>().mReceiverState = mOpen;
	}

	/*public void RotateWallAttachment(LevelTile tile)
	{
		mWallAttachedTo = wallAttachObject.Rotate(tile);
	}*/
	
	public void SetMaterials(string doorTextureToLoad)
	{
		mMatOpenName = doorTextureToLoad + "_Open";
		mMatClosedName = doorTextureToLoad + "_Closed";
		mMatOpen = Resources.Load("Materials/Walls/" + mMatOpenName) as Material;
		mMatClosed = Resources.Load("Materials/Walls/" + mMatClosedName) as Material;
		
		Debug.Log(mMatOpen);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Open()
	{
		Material loadedMat;
		loadedMat = mMatOpen;
		mSide1.gameObject.renderer.material = loadedMat;
		mSide2.gameObject.renderer.material = loadedMat;
		if(mWallAttachedTo)
			mWallAttachedTo.SetPassabble(true);
		mOpen = true;
	}
	
	public void Close()
	{
		Material loadedMat;
		loadedMat = mMatClosed;
		mSide1.gameObject.renderer.material = loadedMat;
		mSide2.gameObject.renderer.material = loadedMat;
		if(mWallAttachedTo)
			mWallAttachedTo.SetPassabble(false);
		mOpen = false;
	}
}
