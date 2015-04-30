using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Usable : MonoBehaviour {
	public bool mUsedByItem;
	public bool mDestroyOnUse;
	public string mItemToUse;
	
	public bool mTriggerSelf;
	
	public TriggerType mTriggerType;
	public int mTrigValue;
	public bool mTriggerState;
	private Triggerable mTarget;
	
	void Awake()
	{
		mTarget = this.gameObject.GetComponent<Triggerable>();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual void OnUse()
	{
		if(!this.enabled)
			return;
		
		if(mUsedByItem)
		{
			if(Player.Instance.ItemHeld == null)
			{
				GameMainHUDScreen.Instance.AddHistoryText("An item is needed for this action.");
				return;
			}
			if(!mItemToUse.Equals(Player.Instance.ItemHeld.mItemName))
			{
				GameMainHUDScreen.Instance.AddHistoryText("Carrying wrong item for this action.");
				return;
			}
			
			if(mDestroyOnUse)
				Player.Instance.ItemHeld = null;
		}
		
		if(mTriggerSelf)
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
		
		foreach(ToTrigger trigObj in gameObject.GetComponents<ToTrigger>())
		{
			trigObj.Trigger();
		}
	}
	
}
