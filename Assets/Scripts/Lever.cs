using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour {

	public GameObject mSide1;
	private LevelWall mWallAttachedTo;
	public bool mState;
	
	public string mMatOpen;
	public string mMatClosed;
	
	
	// Use this for initialization
	void Start () 
	{
		AttachedToWall obj = gameObject.GetComponent<AttachedToWall>();
		mWallAttachedTo = obj.mWallAttachedTo;
		
		GetComponent<Triggerable>().mReceiverState = mState;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SwitchOn()
	{
		Material loadedMat;
		loadedMat = Resources.Load("Materials/" + mMatOpen) as Material;
		mSide1.gameObject.renderer.material = loadedMat;
		mState = true;
	}
	
	public void SwitchOff()
	{
		Material loadedMat;
		loadedMat = Resources.Load("Materials/" + mMatClosed) as Material;
		mSide1.gameObject.renderer.material = loadedMat;
		mState = false;
	}
}
