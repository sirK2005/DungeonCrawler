using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LevelTileSnapComponent : MonoBehaviour {
	
	public int snapGridSize = 20;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 curPos = transform.localPosition;
		
		Vector3 snappedPos = new Vector3();
		
		snappedPos.x = (int)curPos.x ;
		snappedPos.y = curPos.y;
		snappedPos.z = (int)curPos.z;
		
		snappedPos.x = Mathf.RoundToInt(snappedPos.x / snapGridSize) * snapGridSize;
		snappedPos.z = Mathf.RoundToInt(snappedPos.z / snapGridSize) * snapGridSize;
		
		transform.localPosition = snappedPos;
	}
}
