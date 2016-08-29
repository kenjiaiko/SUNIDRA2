using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.IO;

public class GameRuleCtrl : MonoBehaviour {
	// game time (0-300)
	public float timeRemaining = 5.0f * 60.0f;
	public static int timeRemaining_forScoreScene = -1;

	public static string GameCookie = "";

	// game over flag
	public bool gameOver = false;
	// game clear flag
	public bool gameClear = false;

	// 
	public float sceneChangeTime = 6.0f;
	
	public AudioClip clearSeClip;
	AudioSource clearSeAudio;

	public int NetworkFlag = 0; // 0=OFF,1=ON
	public int timeRemainingInt_Back = -1;
	private string GameCtrlURL = "https://cedec2015.seccon.jp/cedec2015/GameCtrl/";
	private string httpkey1  = "abcdefghijklmnopqrstuvwxyz012345";
	private string httpkey2  = "6789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private string apkhash   = "";

	GameObject player = null;
	CharacterStatus player_cs = null;

	GameObject dragon = null;
	CharacterStatus dragon_cs = null;

	GameObject gameGUI = null;
	CharacterStatusGui characterStatusGUI_cs = null;

	public static int get_score()
	{
		return timeRemaining_forScoreScene;
	}

	public static string get_gamecookie()
	{
		return GameCookie;
	}

	void Start()
	{
		// init audio
		clearSeAudio = gameObject.AddComponent<AudioSource>();
		clearSeAudio.loop = false;
		clearSeAudio.clip = clearSeClip;

		// for getting status of player
		player = GameObject.Find ("Player");
		player_cs = player.GetComponent<CharacterStatus> ();

		/*
		// for getting status of dragon
		dragon = GameObject.Find ("Dragon");
		dragon_cs = dragon.GetComponent<CharacterStatus> ();
		*/

		// for change gameGUI to connect/noconnect
		gameGUI = GameObject.Find ("GameGUI");
		characterStatusGUI_cs = gameGUI.GetComponent<CharacterStatusGui> ();

		// set player name from title
		player_cs.characterName = TitleSceneCtrl.get_playername ();

		// init score
		timeRemaining_forScoreScene = -1;

		apkhash = TitleSceneCtrl.get_systemcode ();

		if (PlayerController.get_gameflag () == 0) 
		{
			GameObject[] x = GameObject.FindGameObjectsWithTag("Boss");
			for (int i = 0; i < x.Length; i++) {
				if (x[i].name != "Warg") {
					x[i].SetActive(false);
				}
			}
			x = GameObject.FindGameObjectsWithTag("Warg");
			for (int i = 0; i < x.Length; i++) {
				x[i].SetActive(false);
			}
			GameObject y = GameObject.FindGameObjectWithTag("Generator");
			y.SetActive(false);
		}

		if (PlayerController.get_gameflag () == 1) 
		{
			GameObject[] x = GameObject.FindGameObjectsWithTag("Boss");
			for (int i = 0; i < x.Length; i++) {
				if (x[i].name != "Warg") {
					x[i].SetActive(false);
				}
			}
			GameObject y = GameObject.FindGameObjectWithTag("Generator");
			y.SetActive(false);
		}

		if (PlayerController.get_gameflag () >= 2) 
		{
			GameObject[] x = GameObject.FindGameObjectsWithTag("Boss");
			for (int i = 0; i < x.Length; i++) {
				if (x[i].name != "Dragon") {
					x[i].SetActive(false);
				}
			}
		}

		NetworkFlag = 1;
		GameCookie = "";

		//ShareStatus("Start");
	}
	
	void Update()
	{
		if( gameOver || gameClear ){
			sceneChangeTime -= Time.deltaTime;
			if( sceneChangeTime <= 0.0f ){
				int x = PlayerController.get_gameflag();
				if (gameClear == true) {
					PlayerController.set_gameflag(x+1, timeRemaining_forScoreScene);
				}
				if (gameOver == true) {
					PlayerController.set_gameflag(-(x+1), 30);
				}
				Application.LoadLevel("2dScene");
			}
			return;
		}
		
		timeRemaining -= Time.deltaTime;

		if (timeRemaining <= 0.0f ) {
			GameOver();
		}

		int timeRemainingInt = (int)timeRemaining;

		if ((timeRemainingInt % 5) == 0) {
			if (timeRemainingInt != timeRemainingInt_Back) {
				timeRemainingInt_Back = timeRemainingInt;
			}
		}

		if (NetworkFlag == 0) {
			if (characterStatusGUI_cs.nameLabelStyle.normal.textColor != Color.black) {
				characterStatusGUI_cs.nameLabelStyle.normal.textColor = Color.black;
			}
		}
	}
	
	public void GameOver()
	{
		gameOver = true;
		GameCookie = "";
	}
	public void GameClear()
	{
		timeRemaining_forScoreScene = (int)timeRemaining;

		gameClear = true;
		//ShareStatus("GameClear");

		clearSeAudio.Play ();
	}
	
	void DisConnect()
	{
		NetworkFlag = 0;
	}
	/*
	void ShareStatus(string status)
	{
		if (NetworkFlag == 0) {
			GameCookie = "";
			return;
		}

		// player info
		int p_HP = player_cs.HP;
		byte[] p_name = System.Text.Encoding.ASCII.GetBytes (player_cs.characterName);

		// enemy info
		int e_HP = dragon_cs.HP;


		string data = "";

		// --
		data += RandomString(256) + ":" + apkhash + ","; //DUMMY + base64(sha1 of apk)
		data += p_HP.ToString() + ","; //PlayerHP
		data += e_HP.ToString() + ","; //DragonHP
		data += ((int)timeRemaining).ToString() + ","; //Time
		data += Convert.ToBase64String(p_name); //PlayerName
		// --

		string httpkey3 = RandomString (32);
		if (GameCookie == "")
			GameCookie = RandomString (32);

		string k = RJ256 (
			Convert.FromBase64String(GameCookie), 
			System.Text.Encoding.ASCII.GetBytes(httpkey1), httpkey2, true);
		data = RJ256 (
			Convert.FromBase64String(k), 
			System.Text.Encoding.ASCII.GetBytes(httpkey3), data, true);

		Dictionary<string,string> postfmt = new Dictionary<string,string> ();

		postfmt ["s"] = status;
		postfmt ["p"] = data;
		postfmt ["g"] = GameCookie;

		POST(GameCtrlURL, postfmt);
	}
*/
	/*
	public WWW GET(string url)
	{
		WWW www = new WWW (url);
		StartCoroutine (WaitForRequest (www));
		return www;
	}
	*/

	WWW POST(string url, Dictionary<string,string> post)
	{
		WWWForm form = new WWWForm();
		foreach(KeyValuePair<string,string> post_arg in post) {
			form.AddField(post_arg.Key, post_arg.Value);
		}
		WWW www = new WWW(url, form);
		StartCoroutine(WaitForRequest(www));
		return www;
	}

	private IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		if (www.error == null) {
			if (www.text == "cheat") {
				GameCookie = " (Err: Cheater)";
				DisConnect();
			} else if(www.text == "timeout"){
				GameCookie = " (Err: Timeout)";
				DisConnect();
			} else if(www.text == "unknown"){
				GameCookie = " (Err: Unknown)";
				DisConnect();
			} else if(www.text == "timerange"){
				GameCookie = " (Err: Timerange)";
				DisConnect();
			} else if(www.text == "err"){
				GameCookie = " (Err: Error)";
				DisConnect();
			} else if(www.text == "ok"){
				GameCookie = "";
				DisConnect ();
			} else {
				GameCookie = www.text;
			}
		} else {
			GameCookie = " (Offline)";
			DisConnect ();
		}
	}

	string RandomString(int count)
	{
		string group = "";

		group += "0123456789+/";
		group += "abcdefghijklmnopqrstuvwxyz";
		group += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		char[] list = new char[count];
		for (int i=0; i < count; i++) {
			list[i] = group[UnityEngine.Random.Range (0, group.Length)];
		}

		return new string(list);
	}

	string RJ256(byte[] key, byte[] IV, string data, bool flag)
	{
		RijndaelManaged myRijndael = new RijndaelManaged();

		// --
		myRijndael.Padding   = PaddingMode.Zeros;
		myRijndael.Mode      = CipherMode.ECB;
		myRijndael.KeySize   = 256;
		myRijndael.BlockSize = 256;
		// --

		string r;

		if(flag){ //Encrypt

			// data
			byte[] toEncryptData = System.Text.Encoding.UTF8.GetBytes(data);

			// key
			ICryptoTransform h = myRijndael.CreateEncryptor(key, IV);

			// create enc stream
			MemoryStream outp = new MemoryStream();
			CryptoStream inp  = new CryptoStream(outp, h, CryptoStreamMode.Write);

			// streaming
			inp.Write(toEncryptData, 0, toEncryptData.Length);
			inp.FlushFinalBlock();

			// get enc data with base64
			r = Convert.ToBase64String(outp.ToArray());

		}else{ // Decrypt

			// data with base64 decoding
			byte[] toDecryptData = Convert.FromBase64String(data);
			byte[] PlainData = new byte[toDecryptData.Length];

			// key
			ICryptoTransform h = myRijndael.CreateDecryptor(key, IV);

			// create dec stream
			MemoryStream inp  = new MemoryStream(toDecryptData);
			CryptoStream outp = new CryptoStream(inp, h, CryptoStreamMode.Read);

			// streaming
			outp.Read(PlainData, 0, PlainData.Length);

			// get dec data
			r = System.Text.Encoding.UTF8.GetString(PlainData);
		}
		
		return r;
	}

	public void OnApplicationPause (bool pauseStatus)
	{
		if (pauseStatus) {
		} else {
			DisConnect();
			//Debug.Log("onResume");
		}
	}
}
