using UnityEngine;
using System.Collections;

public class LoadLevelChecker : MonoBehaviour {
	
	private static bool mTested;
	private static bool mAlreadyExists;
	
	void Awake()
	{
		if(mAlreadyExists)
			Destroy(gameObject);
		
		DontDestroyOnLoad(gameObject);
		
		mAlreadyExists = true;
		
		if(mTested)
			return;
		
		mTested = true;
		
		LevelTile tempPlayer;
		
		try
		{
			tempPlayer = Player.Instance.TilePlacedOn;
		}
		catch
		{
			
			//player could not be found, so we didnt load from MenueScreen, so do that manually
			if(Application.loadedLevelName.Equals("TechDemoLevel"))
			{
				Application.LoadLevel("Init");
				Application.LoadLevel("TechDemoLevel");
			}
			if(Application.loadedLevelName.Equals("GenericLevel"))
			{
				Application.LoadLevel("Init");
				Application.LoadLevel("GenericLevel");
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
}
