using UnityEngine;
using System.Collections;

public enum TriggerType
{
	Toggle,
	ContinuousOn,
	ContinuousOff,
	Once
}

public class ToTrigger : MonoBehaviour {
	public int mTrigValue;
	public Triggerable mTarget;
	
	public TriggerType mTriggerType;
	public bool mTriggerState;
	
	void Start()
	{
		if(mTarget)
			mTriggerState = mTarget.mReceiverState;
	}
	
	public void Trigger()
	{
		if(mTriggerType == TriggerType.Toggle)
		{
			if(mTriggerState) // already turned on
			{
				mTarget.TriggerOff(mTrigValue);
				
				mTriggerState = false;
			}
			else
			{
				mTarget.TriggerOn(mTrigValue);
				
				mTriggerState = true;
			}
		}
		
		if(mTriggerType == TriggerType.ContinuousOn)
		{
			mTarget.TriggerOn(mTrigValue);
		}
		
		if(mTriggerType == TriggerType.ContinuousOff)
		{
			mTarget.TriggerOff(mTrigValue);
		}
		
		if(mTriggerType == TriggerType.Once)
		{
			if(!mTriggerState)
			{
				mTarget.TriggerOn(mTrigValue);
				mTriggerState = true;
			}
		}
	}
}
