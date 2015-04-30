using UnityEngine;
using System.Collections;

public class MainMenueScreen : MonoBehaviour {
	
	private static MainMenueScreen mInstance;
	public static MainMenueScreen Instance {get {return mInstance;}}
	
	void Awake()
	{
		mInstance = this;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	private void OnStartGenericLevelBtnHit()
	{
		Player.Instance.Health = Player.Instance.MaxHealth;
		Player.Instance.Dead = false;
		
		Application.LoadLevel("GenericLevel");
	}

	private void OnStartTechDemolBtnHit()
	{
		Player.Instance.Health = Player.Instance.MaxHealth;
		Player.Instance.Dead = false;
		
		Application.LoadLevel("TechDemoLevel");
	}
}
