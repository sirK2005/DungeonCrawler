using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
	
	public float mCurTime;
	public float mStartTime;
	public float mTriggerTime;
	public bool mLoop;
	public bool mRunning;
	public bool mStartRunning;
	
	void Awake()
	{
		gameObject.GetComponentInChildren<MeshFilter>().gameObject.SetActive(false);
	}
	
	void Start()
	{
		if(mStartRunning)
			StartTimer();
	}
	
	public void StartTimer()
	{
		mRunning = true;
		mCurTime = mStartTime;
	}
	
	public void StopTimer()
	{
		mRunning = false;
	}
	
	void Update()
	{
		if(!mRunning)
			return;
		
		
		mCurTime += Time.deltaTime;
	
		if(mCurTime >= mTriggerTime){
			foreach(ToTrigger trigObj in GetComponents<ToTrigger>())
				trigObj.Trigger();
			
			if(!mLoop)
			{
				mRunning = false;
			}
			else
			{
				mCurTime = 0;
			}
		}
	}

}
