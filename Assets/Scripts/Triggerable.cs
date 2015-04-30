using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Triggerable : MonoBehaviour {
	private GameObject mTarget;
	public bool mReceiverState;
	public string mTriggerOnFunc;
	public string mTriggerOffFunc;
	
	void Awake()
	{
		mTarget = gameObject;
	}
	
	public void TriggerOn(int trigValue)
	{
		if(!string.IsNullOrEmpty(mTriggerOnFunc))
			mTarget.SendMessage(mTriggerOnFunc, trigValue, SendMessageOptions.DontRequireReceiver);
		else
			Debug.LogWarning("No TriggerOnFunction has been defined");
	}
	
	public void TriggerOff(int trigValue)
	{
		if(!string.IsNullOrEmpty(mTriggerOffFunc))
			mTarget.SendMessage(mTriggerOffFunc, trigValue, SendMessageOptions.DontRequireReceiver);
		else
			Debug.LogWarning("No TriggerOffFunction has been defined");
		
	}

}
