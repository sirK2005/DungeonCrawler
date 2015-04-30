using UnityEngine;
using System.Collections;

public class ScreenActivator : MonoBehaviour {
	
	void Awake()
	{
		DontDestroyOnLoad(this);
	}
	
	void OnLevelWasLoaded(int level)
	{
		if(level == 0)
		{
			GameMainHUDScreen.Instance.gameObject.SetActive(false);
			InputScreen.Instance.gameObject.SetActive(false);
			MainMenueScreen.Instance.gameObject.SetActive(false);
		}
		
		if(level == 1)
		{
			GameMainHUDScreen.Instance.gameObject.SetActive(false);
			InputScreen.Instance.gameObject.SetActive(false);
			MainMenueScreen.Instance.gameObject.SetActive(true);
		}
		
		if(level > 1)
		{
			GameMainHUDScreen.Instance.gameObject.SetActive(true);
			InputScreen.Instance.gameObject.SetActive(true);
			MainMenueScreen.Instance.gameObject.SetActive(false);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
