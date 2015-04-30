using UnityEngine;
using System.Collections;

public class InputScreen : MonoBehaviour {
	public bool mPressedLeft;
	public bool mPressedRight;
	public bool mPressedForward;
	public bool mMouseClicked;
	public bool mPressedBackward;
	
	public UIAnchor mAnchorLeft;
	public UIAnchor mAnchorRight;
	
	private static InputScreen _Instance;
	public static InputScreen Instance {get {return _Instance;} }
	
	void Awake()
	{
		_Instance = this;
		
		mPressedLeft = false;
		mPressedRight = false;
		mPressedForward = false;
		mPressedBackward = false;
		mMouseClicked = false;
		
		UICamera.fallThrough = this.gameObject;
		
#if UNITY_STANDALONE
		mAnchorLeft.relativeOffset =  new Vector2(-1,-1);
		mAnchorRight.relativeOffset =  new Vector2(-1,-1);
#endif
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	void OnClick()
	{
		//Didnt hit any NGUI Collision Boxes, so handle game-screen mouse-clicking
		mMouseClicked = true;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	private void OnBtnLeftHit()
	{
		mPressedLeft = !mPressedLeft;
	}
	
	private void OnBtnRightHit()
	{
		mPressedRight = !mPressedRight;
	}
	
	private void OnBtnFwdHit()
	{
		mPressedForward = !mPressedForward;
	}
	
	private void OnBtnBwdHit()
	{
		mPressedBackward = !mPressedBackward;
	}
}
