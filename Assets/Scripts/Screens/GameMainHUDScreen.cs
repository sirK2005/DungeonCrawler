using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMainHUDScreen : MonoBehaviour {
	// GUI Constances
	public static int GUI_WIDTH;
	public static int GUI_HEIGHT;
	
	// UI Objects
	public UILabel mHistoryLabel;
	public UIButton mTextWindowExitBtn;
	public Transform mTextWindowTrans;
	public UILabel mTextWindowText;
	public UISprite mItemHeldSprite;
	public UIPanel mMapPanel;
	public UIAtlas mAtlas;
	public UISprite mMapPlayerSprite;
	public UISlider PlayerHealthbBarSlider;
	
	// damage marker
	public UISprite mDamageMarkerTop;
	public UISprite mDamageMarkerRight;
	public UISprite mDamageMarkerBottom;
	public UISprite mDamageMarkerLeft;
	private float mDmgMarkTopAlpha;
	private float mDmgMarkLeftAlpha;
	private float mDmgMarkBotAlpha;
	private float mDmgMarkRightAlpha;
	
	// Item Sprite Holders
	public UISprite mWeapon;
	
	private float mWeaponAnimationScreenPos;
	
	private List<Vector2> mTilesSearchedList;
	
	// 
	public float mTimeHistoryVisible;
	public float mTimeHistoryVisibleMax;
	public float mHistoryFadeSpd;
	
	private static GameMainHUDScreen mInstance;
	public static GameMainHUDScreen Instance {get {return mInstance;}}
	
	void Awake()
	{
		mInstance = this;
		
		// Get GUI Screen Values
		UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
		DontDestroyOnLoad(mRoot);
		float ratio = (float)mRoot.activeHeight / Screen.height;
		GUI_WIDTH = (int) Mathf.Ceil(Screen.width * ratio);
		GUI_HEIGHT = (int) Mathf.Ceil(Screen.height * ratio);
		
		mTimeHistoryVisible = 0;
		mHistoryFadeSpd = 0.75f;
		
		mTilesSearchedList = new List<Vector2>();
		mMapPlayerSprite = NGUITools.AddSprite(mMapPanel.gameObject, mAtlas, "gui_map_direc");
		mMapPlayerSprite.transform.localScale = new Vector3(12, 12, 0);
		mMapPlayerSprite.transform.localPosition = new Vector3(0, 0, -10);
		
		mDmgMarkTopAlpha = mDmgMarkRightAlpha = mDmgMarkBotAlpha = mDmgMarkLeftAlpha = 1;
		
		PositionWeaponOnScreen(1.2f, 1);
		mWeaponAnimationScreenPos = 1.2f;
		//mWeapon.transform.localPosition =  new Vector3(Screen.width, Screen.height, 0);
		
	}
	
	// Use this for initialization
	void Start () {
		OnTxtWndBtnHit();
	}
	
	public void AddMapTile(int x, int y, bool n, bool e, bool s, bool w)
	{
		
		if(mTilesSearchedList.Contains(new Vector2(x, y)))
			return;
		
		mTilesSearchedList.Add(new Vector2(x, y));
		
		if(n){
			CreateMapSprite(x, y);
		}
		if(e){
			CreateMapSprite(x, y).transform.Rotate(0, 0, -90);
		}
		if(s){
			CreateMapSprite(x, y).transform.Rotate(0, 0, 180);
		}
		if(w){
			CreateMapSprite(x, y).transform.Rotate(0, 0, -270);
		}
		
		AdjustMapPosToPos(x, y);
	}
	
	public void AdjustMapPosToPos(int x, int y)	
	{
		mMapPanel.transform.localPosition = new Vector3(-x * 16, -50 - y * 16, 0);
		mMapPanel.clipRange = new Vector4(x * 16, y * 16, 90, 90);
		mMapPlayerSprite.transform.localPosition = new Vector3(x * 16, y * 16, 0);
	}
	
	public void AdjustMapPlayerDirec(LookingDirection direc){
		
		switch(direc)
		{
			case LookingDirection.North:
			mMapPlayerSprite.transform.eulerAngles = new Vector3(0,0,0);
			break;
			
			case LookingDirection.East:
			mMapPlayerSprite.transform.eulerAngles = new Vector3(0,0,-90);
			break;
			
			case LookingDirection.South:
			mMapPlayerSprite.transform.eulerAngles = new Vector3(0,0,180);
			break;
			
			case LookingDirection.West:
			mMapPlayerSprite.transform.eulerAngles = new Vector3(0,0,90);
			break;
		}
	}
	
	private UISprite CreateMapSprite(int x, int y)
	{
		UISprite sprite = NGUITools.AddSprite(mMapPanel.gameObject, mAtlas, "gui_map_wall");
		sprite.transform.localScale = new Vector3(16, 2, 0);
		sprite.transform.localPosition = new Vector3(x * 16, y * 16, -10);
		return sprite;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float dmgMarkerFadeSpd = 1;
		
		// update damake marker visibility
		Color tempCol;
		if(mDmgMarkTopAlpha > 0)
		{
			mDamageMarkerTop.color -= new Color(0, 0, 0, mDmgMarkTopAlpha);
			mDmgMarkTopAlpha -= Time.deltaTime * dmgMarkerFadeSpd;
			mDamageMarkerTop.color += new Color(0, 0, 0, mDmgMarkTopAlpha);
		}
		if(mDmgMarkRightAlpha > 0)
		{
			mDamageMarkerRight.color -= new Color(0, 0, 0, mDmgMarkRightAlpha);
			mDmgMarkRightAlpha -= Time.deltaTime * dmgMarkerFadeSpd;
			mDamageMarkerRight.color += new Color(0, 0, 0, mDmgMarkRightAlpha);
		}
		if(mDmgMarkBotAlpha > 0)
		{
			mDamageMarkerBottom.color -= new Color(0, 0, 0, mDmgMarkBotAlpha);
			mDmgMarkBotAlpha -= Time.deltaTime * dmgMarkerFadeSpd;
			mDamageMarkerBottom.color += new Color(0, 0, 0, mDmgMarkBotAlpha);
		}
		if(mDmgMarkLeftAlpha > 0)
		{	
			mDamageMarkerLeft.color -= new Color(0, 0, 0, mDmgMarkLeftAlpha);
			mDmgMarkLeftAlpha -= Time.deltaTime * dmgMarkerFadeSpd;
			mDamageMarkerLeft.color += new Color(0, 0, 0, mDmgMarkLeftAlpha);
		}
		
		// weapon movement
		float hitSpd = 2f;
		if(mWeaponAnimationScreenPos < 1.2f)
		{
			mWeaponAnimationScreenPos += Time.deltaTime * hitSpd;
			PositionWeaponOnScreen(mWeaponAnimationScreenPos, 0.5f);
		}
		
		// update history visibility
		if(mTimeHistoryVisible > 0)
		{
			mTimeHistoryVisible -= Time.deltaTime * mHistoryFadeSpd;
			mHistoryLabel.color = new Color(1, 1, 1, mTimeHistoryVisible);
		}
		
		// update item held sprite
		if(Player.Instance.ItemHeld != null)
		{
			mItemHeldSprite.enabled = true;
			mItemHeldSprite.spriteName = Player.Instance.ItemHeld.mItemName;
		}
		else
			mItemHeldSprite.enabled = false;
	}
	
	public void SetDamageMarkerAlpha(int marker, float alpha)
	{
		switch(marker)
		{
		case 0:
			mDmgMarkTopAlpha = alpha;
			break;
			
		case 1:
			mDmgMarkRightAlpha = alpha;
			break;
			
		case 2:
			mDmgMarkBotAlpha = alpha;
			break;
			
		case 3:
			mDmgMarkLeftAlpha = alpha;
			break;
			
		default:
			Debug.LogError("GameScreenUI: Something went wrong setting alpha of Damage Marker");
			break;
		}
	}
	
	public void AddHistoryText(string txt)
	{
		mHistoryLabel.text = txt;
		mHistoryLabel.color = new Color(1, 1, 1, 1);
		mTimeHistoryVisible = 4;
	}
	
	public void ShowTextWindow(string txt)
	{
		mTextWindowText.text = txt;
		mTextWindowTrans.localPosition = new Vector3(0,0,-30);
		PlayerControl.INPUT_MOVINGBLOCKED = true;
	}
	
	private void OnTxtWndBtnHit()
	{
		mTextWindowTrans.localPosition = new Vector3(-Screen.width * 2,0,0);
		PlayerControl.INPUT_MOVINGBLOCKED = false;
	}
	
	public void UpdateHealthBar(int maxHealth, int curHealth)
	{
		float fill;
		
		if(curHealth >= 0)
		{
			fill = (float)curHealth / maxHealth;
		}
		else
			fill = 0;
		
		PlayerHealthbBarSlider.sliderValue = fill;
	}
	
	public void StartAttackAnimation()
	{
		mWeaponAnimationScreenPos = 0;
	}
	
	private void PositionWeaponOnScreen(float x, float y)
	{
		// make relative values to pixel values:
		int pixelPosX = (int) (x * GUI_WIDTH);
		int pixelPosY = (int) (y * GUI_HEIGHT);
		
		mWeapon.transform.localPosition = new Vector3(pixelPosX, pixelPosY, 0);
	}
}
