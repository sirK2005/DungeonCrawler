using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public static bool INPUT_LEFT;
	public static bool INPUT_RIGHT;
	public static bool INPUT_FORWARD;
	public static bool INPUT_BACKWARD;
	
	public static bool INPUT_LMBCLICKED;
	public static bool INPUT_MOVINGBLOCKED;
	
	void Update()
	{
	
		if(INPUT_MOVINGBLOCKED)
		{
			
			InputScreen.Instance.mPressedLeft = false;
			InputScreen.Instance.mPressedRight = false;
			InputScreen.Instance.mPressedForward = false;
			InputScreen.Instance.mPressedBackward = false;		
		}
		else
		{
			INPUT_LEFT = Input.GetKey(KeyCode.A) || InputScreen.Instance.mPressedLeft;
			INPUT_RIGHT = Input.GetKey(KeyCode.D) || InputScreen.Instance.mPressedRight;
			INPUT_FORWARD = Input.GetKey(KeyCode.W) || InputScreen.Instance.mPressedForward;
			INPUT_BACKWARD = Input.GetKey(KeyCode.S) || InputScreen.Instance.mPressedBackward;
		}
		
		INPUT_LMBCLICKED = InputScreen.Instance.mMouseClicked;
		InputScreen.Instance.mMouseClicked = false;
		
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(Application.loadedLevel != 1)
				Application.LoadLevel("MainMenue");
			else
				Application.Quit();
		}
		
		//Debug.Log(INPUT_FORWARD);
	}
	
	public static void SetForward(bool val)
	{
		INPUT_FORWARD = val;
	}
	
	public static void SetLeft(bool val)
	{
		INPUT_LEFT = val;
	}
	
	public static void SetRight(bool val)
	{
		INPUT_RIGHT = val;
	}
}
