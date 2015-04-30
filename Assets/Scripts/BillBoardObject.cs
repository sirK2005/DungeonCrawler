using UnityEngine;
using System.Collections;

public class BillBoardObject : MonoBehaviour {
	
	public Transform playerCamera;
	
	void Awake()
	{
		
	}

	// Use this for initialization
	void Start () 
	{
		Init();
	}
	
	public void Init()
	{
		playerCamera = Player.Instance.mPlayerCamera.transform;
	}
	
	public virtual void UpdatePos()
	{
		transform.rotation = playerCamera.rotation;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdatePos();
	}
}
