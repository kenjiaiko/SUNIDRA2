using UnityEngine;
using System.Collections;

public class ScoreSceneCtrl : MonoBehaviour {

	// タイトル画面テクスチャ
	public Texture2D bgTexture;

	private string player_name;
	private string to_message = "Thank you for playing!!";

	// Use this for initialization
	void Start () {
		player_name = TitleSceneCtrl.get_playername ();
		PlayerPrefs.GetInt (player_name + "_game_flag", 0);
	}

	void Update()
	{
	}

	void OnGUI()
	{
		// スタイルを準備.
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
		
		// 解像度対応
		GUI.matrix = Matrix4x4.TRS(
			Vector3.zero,
			Quaternion.identity,
			new Vector3(Screen.width / 854.0f, Screen.height / 480.0f, 1.0f));

		// タイトル画面テクスチャ表示
		GUI.DrawTexture(new Rect(0.0f, 0.0f, 854.0f, 480.0f), bgTexture);

		GUIStyle style = new GUIStyle();
		style.fontSize = 24;
		style.normal.textColor = Color.white;
		GUI.Label(new Rect(310, 190, 200, 30), to_message, style);

		if (GUI.Button (new Rect (327, 290, 200, 54), " Quit", buttonStyle)) {
			player_name = TitleSceneCtrl.get_playername ();
			PlayerPrefs.SetInt (player_name + "_game_flag", 0);
			Application.Quit();
		}
	}
}
