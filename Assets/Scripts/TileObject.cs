using UnityEngine;
using System.Collections;

public enum TilePositions
{
	Middle = 0,
	NorthWest = 5,
	North = 1,
	NorthEast = 6,
	West = 4,
	East = 2,
	SouthWest = 7,
	South = 3,
	SouthEast = 8,
	RandomPos
}
public class TileObject : MonoBehaviour {
	
	private TilePositions mPos;
	public TilePositions Pos { get{return mPos;}}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
