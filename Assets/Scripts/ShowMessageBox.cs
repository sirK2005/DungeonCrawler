using UnityEngine;
using System.Collections;

public class ShowMessageBox : MonoBehaviour {
	public string mMessage;

	public void Read(int para)
	{
		GameMainHUDScreen.Instance.ShowTextWindow(mMessage);
	}
}
