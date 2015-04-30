using UnityEngine;
using System.Collections;

public class Plate : MonoBehaviour {
	private Vector3 mPlatePos;
	public bool mTriggerOnLeave;
	private bool mSteppedOn;
	public bool mTexHidden;

	// Use this for initialization
	void Start () {
		mPlatePos = transform.position;
		if(mTexHidden)
			gameObject.GetComponentInChildren<MeshFilter>().gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		//TestForCollision();
	}
	
	private void OnPlayerMoved()
	{
		TestForCollision();
	}
	
	private void TestForCollision()
	{
		if(collider.bounds.Contains(Player.Instance.mPlayerCamera.transform.position + new Vector3(0,-Player.Instance.mPosOffsetY, 0)))
		{
			mSteppedOn = true;
			foreach(ToTrigger trigObj in gameObject.GetComponents<ToTrigger>())
				trigObj.Trigger();
		}
		else
		{
			if(mSteppedOn)
			{
				mSteppedOn = false;
				if(mTriggerOnLeave)
				{
					foreach(ToTrigger trigObj in gameObject.GetComponents<ToTrigger>())
						trigObj.Trigger();
				}
				
			}
		}
	}
}
