using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimationType
{
	PingPong,
	Loop,
	Single,
}

public class Animated : MonoBehaviour {
	
	public int mAnimationSteps;
	public float mAnimationSpeed;
	public AnimationType mAniType;
	public bool mStartAnimated;
	
	private float mAniPingPongDirec;
	private float mCurTimeAnimated;
	private bool mAnimate;
	public bool Running {get {return mAnimate;}}
	
	public List<List<Material>> mMeshAniMaterials;
	public List<GameObject> mMeshObjectList;
	
	private FourSided mFourSidedAttached;
	
	public string mObjectName;
	Hashtable mSideTable;
	
	public GameObject mTexGoal;
	
	void Awake()
	{
	}
	
	// Use this for initialization
	void Start () {
		mAnimate = true;
		mCurTimeAnimated = 0;
		mAniPingPongDirec = 1;
		
		AnimationInit();
		
		if(mStartAnimated)
			StartAnimation();
		else
			StopAnimation();
	}
	
	public void AnimationInit()
	{
		mFourSidedAttached = GetComponent<FourSided>();
		
		mSideTable = new Hashtable();
		mSideTable.Add(0, "Front");
		mSideTable.Add(1, "Back");
		mSideTable.Add(2, "Left");
		mSideTable.Add(3, "Right");
		
		// get all meshes of object
		mMeshAniMaterials = new List<List<Material>>();
		if(mFourSidedAttached)
		{
			for(int sideCnt = 0; sideCnt < 4; sideCnt++)
			{
				List<Material> matList = new List<Material>();
				for(int count = 0; count < mAnimationSteps; count++)
				{
					string goalMatName = "Materials/" + mObjectName + "_" + (string)mSideTable[sideCnt] + "_" + count;
					Material mat = Resources.Load(goalMatName) as Material;
					if(mat == null)
						Debug.LogWarning(goalMatName + " could not be loaded!");
					matList.Add(mat);
				}
				mMeshAniMaterials.Add(matList);
			}
		}
		else
		{
			List<Material> matList = new List<Material>();
			for(int count = 0; count < mAnimationSteps; count++)
			{
				string goalMatName = "Materials/" + mObjectName + "_" + count;
				Material mat = Resources.Load(goalMatName) as Material;
				if(mat == null)
					Debug.LogWarning(goalMatName + " could not be loaded!");
				matList.Add(mat);
			}
			mMeshAniMaterials.Add(matList);
		}
		
		
		mAniPingPongDirec = 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Animate();
	}
	
	public void StartAnimation()
	{
		mAnimate = true;
		mCurTimeAnimated = 0;
	}
	
	public void StopAnimation()
	{
		mAnimate = false;
	}
	
	public void SetAnimationType(AnimationType type)
	{
		mAniType = type;
	}
	
	public void Animate()
	{
		if(mAnimate)
		{
			// update Time
			mCurTimeAnimated += (Time.deltaTime / mAnimationSpeed) * mAniPingPongDirec;
			if((mCurTimeAnimated > mAnimationSteps) || (mCurTimeAnimated < 0))
			{
				if(mCurTimeAnimated < 0)
					mCurTimeAnimated = 1;
				if(mCurTimeAnimated > mAnimationSteps)
					mCurTimeAnimated = mCurTimeAnimated - 1;
				
				switch(mAniType)
				{
				case AnimationType.Loop:
					mCurTimeAnimated = 0;
					break;
					
				case AnimationType.PingPong:
					mAniPingPongDirec *= -1;
					break;
					
				case AnimationType.Single:
					mCurTimeAnimated = mAnimationSteps-1; // set to last animation frame
					StopAnimation();
					break;
				}
			}
			
			//go through all meshes and update animation
			if(mFourSidedAttached)
			{
				mTexGoal.renderer.sharedMaterial = mMeshAniMaterials[(int)mFourSidedAttached.mSideShown][(int) mCurTimeAnimated];
			}
			else
			{
				mTexGoal.renderer.sharedMaterial = mMeshAniMaterials[0][(int) mCurTimeAnimated];
			}
		}
	}
}
