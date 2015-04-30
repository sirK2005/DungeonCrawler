using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LogicType
{
	Less,
	More,
	Equal
}

public class LogicalUnit : MonoBehaviour {
	
	public int mValueHeld;
	public int mValueCompared;
	public LogicType mLogicType;
	
	void Awake()
	{
		gameObject.GetComponentInChildren<MeshFilter>().gameObject.SetActive(false);
	}
	
	private void AddValue(int TrigValue)
	{
		mValueHeld += TrigValue;
		TestLogic();
	}
	
	private void TakeValue(int TrigValue)
	{
		mValueHeld -= TrigValue;
		TestLogic();
	}
	
	public void TestLogic()
	{
		if(mLogicType == LogicType.Equal)
		{
			if(mValueHeld == mValueCompared)
				foreach(ToTrigger trigObj in gameObject.GetComponents<ToTrigger>())
					trigObj.Trigger();
		}
		
		if(mLogicType == LogicType.Less)
		{
			if(mValueHeld < mValueCompared)
				foreach(ToTrigger trigObj in gameObject.GetComponents<ToTrigger>())
					trigObj.Trigger();
		}
		
		if(mLogicType == LogicType.More)
		{
			if(mValueHeld > mValueCompared)
				foreach(ToTrigger trigObj in gameObject.GetComponents<ToTrigger>())
					trigObj.Trigger();
		}
	}
}
