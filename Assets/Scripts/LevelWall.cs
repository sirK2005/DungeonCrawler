using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LevelWall : MonoBehaviour {
	
	[SerializeField]
	private LevelWall mSharedWall;
	public LevelWall SharedWall { get {return mSharedWall;} set {mSharedWall = value;} }
	
	private bool _isActive;
	public bool IsActive { get { return _isActive;} set {_isActive = value;} }
	[SerializeField]
	private bool _isPassable;
	public bool IsPassable { get { return _isPassable;} set {_isPassable = value;}}
	[SerializeField]
	private bool _IsDoor;
	public bool IsDoor{ get {return _IsDoor;} set {_IsDoor = value;}}
	[SerializeField]
	private int mCurrentMaterial;
	public int CurrentMaterial {get {return mCurrentMaterial;} set {mCurrentMaterial = value;} }
	
	
	void Awake()
	{
		_isActive = gameObject.activeSelf;
	}
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	/// <summary>
	/// Sets the texture of this wall to a specified texture.
	/// </summary>
	/// <param name='tex'>
	/// Given Wall-Texture
	/// </param>
	public void SetTexture(int tex)
	{
		Material loadedMat;
		
		mCurrentMaterial = tex;
		
		loadedMat = World.Instance.mMaterialList[tex];
		
		gameObject.renderer.material = loadedMat;
		if(mSharedWall != null)
		{
			mSharedWall.renderer.material = loadedMat;
			mSharedWall.CurrentMaterial = tex;
		}
	}
	
	public void SetActivation(bool act)
	{
		if(act)
		{
			gameObject.SetActive(true);
			_isActive = true;
			if(mSharedWall != null)
			{
				mSharedWall.gameObject.SetActive(true);
				mSharedWall._isActive = true;
			}
			SetPassabble(false);
		}
		else
		{
			gameObject.SetActive(false);
			_isActive = false;
			if(mSharedWall != null)
			{
				mSharedWall.gameObject.SetActive(false);
				mSharedWall._isActive = false;
			}
			SetPassabble(true);
		}
	}
	
	public void SetPassabble(bool pas)
	{
		if(pas)
		{
			_isPassable = true;
			if(mSharedWall != null)
			{
				mSharedWall.IsPassable = true;
			}
		}
		else
		{
			_isPassable = false;
			if(mSharedWall != null)
			{
				mSharedWall.IsPassable = false;
			}
		}
	}
	
	
}
