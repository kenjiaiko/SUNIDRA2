using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.IO;

public class PlayerController : MonoBehaviour
{
    public float moveStartDistance = 10f;
    public float moveForce = 150f;
    public float maxSpeed = 100f;
    public float targetPointX;
    bool facingRight = true;

	private Rigidbody2D rigi;

	Animator myAnimator;

	GameObject Enemy;

	GameObject username;
	public static Text username_txt;
	GameObject stamina;
	public static Text stamina_txt;
	GameObject attack;
	public static Text attack_txt;
	GameObject hp;
	public static Text hp_txt;
	GameObject coins;
	public static Text coins_txt;
	GameObject stone;
	public static Text stone_txt;
	GameObject message;
	Text message_txt;

	System.DateTime UNIX_EPOCH;
	private int now_time;

	private static string player_name;
	private static string GameCookie;

	private static string _key = "abcdefhijlmnoprstvwxyz0123456789";
	private static string nw_key = TitleSceneCtrl.get_crypto_key ();
	private static string GameCtrlURL = "https://cedec.seccon.jp/2016/game";

	private static string _stamina;
	private static string _attack;
	private static string _hp;
	private static string _coins;
	private static string _stone;

	public static int game_flag = 0;
	public static int game_score = 0;
	public static int where_from = 0; // if 1, from TitleScene

	private bool Enemy_flag = false;

	public static void set_where_from(int x)
	{
		where_from = x;
	}

	public static int get_gameflag()
	{
		return game_flag;
	}

	public static void set_gameflag(int v, int score)
	{
		game_flag = v;
		game_score = score;
	}

	public static string get_player_status()
	{
		return _hp + "," + _attack;
	}

	/*
	string s2e(string s)
	{
		return RJ256(
			System.Text.Encoding.ASCII.GetBytes (_key),
			System.Text.Encoding.ASCII.GetBytes (_key),
			s, true);
	}
	*/

	string e2s(string e)
	{
		byte[] r_table = {
			71, 5,108, 253, 33, 45, 11, 99,
			9, 87, 32, 122, 90, 49, 77, 22
		};
		byte[] r_table2 = {
			71, 5,108, 213, 33, 45, 21, 99,
			9, 87, 32, 192, 90, 59, 27, 22,
			71, 5,108, 223, 33, 45, 41, 12,
			9, 87, 32, 182, 10, 99, 58, 32
		};

		e = RJ256 (r_table2, r_table2, e, false);
		
		byte[] tmp_key2 = Convert.FromBase64String (e);
		for (int i=0; i < tmp_key2.Length; i++) {
			tmp_key2[i] ^= r_table[i % r_table.Length];
		}
		
		e = System.Text.Encoding.ASCII.GetString (tmp_key2);
		byte[] tmp_key = Convert.FromBase64String (e);
		for (int i=0; i < tmp_key.Length; i++) {
			tmp_key[i] = (byte)((tmp_key[i] ^ 0x4F) & 0xFF);
		}
		e = System.Text.Encoding.ASCII.GetString (tmp_key);

		return RJ256(
			System.Text.Encoding.ASCII.GetBytes (_key),
			System.Text.Encoding.ASCII.GetBytes (_key),
			e, false);
	}

	void backup_status()
	{
		_stamina = stamina_txt.text;
		_attack = attack_txt.text;
		_hp = hp_txt.text;
		_coins = coins_txt.text;
		_stone = stone_txt.text;
	}

	void recver_status()
	{
		stamina_txt.text = _stamina;
		attack_txt.text = _attack;
		hp_txt.text = _hp;
		coins_txt.text = _coins;
		stone_txt.text = _stone;
	}


    void Start()
    {
		//nw_key = "xQSVIZ5JTSozF4TUNwd3I+vvmQTMmohiQt3zuYDKLRw0C1AUYL6Kj5gRikjQYbYhQhm7ayH7x1E6wL7hOnHuNmt2z/FhZlLEw7vfcSiov9kvoiUgNNUpSD1DIBLLLcSo";
		if (nw_key == "") {
			Application.Quit ();
		}

        Vector3 screen_point = Camera.main.WorldToScreenPoint(transform.position);
        targetPointX = screen_point.x;

		rigi = GetComponent<Rigidbody2D>();
		myAnimator = GetComponent<Animator>();

		Enemy = GameObject.Find ("Enemy");

		// for displaying status of player
		username = GameObject.Find ("name");
		username_txt = username.GetComponent<Text> ();

		stamina = GameObject.Find ("stamina");
		stamina_txt = stamina.GetComponent<Text> ();
		attack = GameObject.Find ("attack");
		attack_txt = attack.GetComponent<Text> ();
		hp = GameObject.Find ("hp");
		hp_txt = hp.GetComponent<Text> ();
		coins = GameObject.Find ("coins");
		coins_txt = coins.GetComponent<Text> ();
		stone = GameObject.Find ("stone");
		stone_txt = stone.GetComponent<Text> ();

		message = GameObject.Find ("message");
		message_txt = message.GetComponent<Text> ();

		// for time
		UNIX_EPOCH = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
		now_time   = GetUnixTime ();

		string data = "";

		player_name = TitleSceneCtrl.get_playername ();
		int stamina_from_file = PlayerPrefs.GetInt (player_name + "_stamina", 999);

		int stamina_update_time = PlayerPrefs.GetInt (player_name + "_stamina_update_time", 0);
		if (stamina_from_file != 0) {
			stamina_from_file += (now_time - stamina_update_time);
			if (999 < stamina_from_file) {
				stamina_from_file = 999;
			}
		}
		stamina_txt.text = ": " + stamina_from_file.ToString ();

		//PlayerPrefs.DeleteKey (player_name + "_game_flag");
		if (where_from == 1) {
			// from TitleScene
			GameCookie = TitleSceneCtrl.get_gamecookie ();
			game_flag = PlayerPrefs.GetInt (player_name + "_game_flag", 0);
			data = now_time.ToString () + "," + now_time.ToString () + "," + "0,0,0,0,0,0";

			GameObject x;
			if (0 < game_flag) {
				x = GameObject.Find ("Fire");
				x.SetActive (false);
			}
			if (1 < game_flag) {
				x = GameObject.Find ("Fire2");
				x.SetActive (false);
			}
			if (2 < game_flag) {
				float gf_f = 3 / (float)game_flag;
				GameObject bd = GameObject.Find ("body");
				SpriteRenderer sr = bd.GetComponent<SpriteRenderer> ();
				sr.color = new Color (gf_f, gf_f, gf_f, 1f);
				
				bd = GameObject.Find ("wingL");
				sr = bd.GetComponent<SpriteRenderer> ();
				sr.color = new Color (gf_f, gf_f, gf_f, 1f);
				
				bd = GameObject.Find ("wingR");
				sr = bd.GetComponent<SpriteRenderer> ();
				sr.color = new Color (gf_f, gf_f, gf_f, 1f);
			}
			where_from = 0;
		} else {
			// from GameScene
			// GameScene set game_flag
			if (game_flag == 1 || game_flag == -1) {
				recver_status ();
				if (game_flag == 1) {
					GameObject x = GameObject.Find ("Fire");
					x.SetActive (false);
				} else {
					game_flag = 0;
				}
				PlayerPrefs.SetInt (player_name + "_game_flag", game_flag);
				message_txt.text = "you got " + game_score.ToString () + " coins.";

				int coins_num = Int32.Parse (coins_txt.text.Substring (2));
				coins_num += game_score;
				coins_txt.text = ": " + coins_num.ToString ();

				//dummy, t, status, stamina, sttack, HP, coins, stone
				data = now_time.ToString () + "," + now_time.ToString () + "," + 
					"5,0,0,0," + game_score.ToString () + ",0";
			}
			if (game_flag == 2 || game_flag == -2) {
				recver_status ();
				GameObject x = GameObject.Find ("Fire");
				x.SetActive (false);
				if (game_flag == 2) {
					x = GameObject.Find ("Fire2");
					x.SetActive (false);
				} else {
					game_flag = 1;
				}
				PlayerPrefs.SetInt (player_name + "_game_flag", game_flag);
				message_txt.text = "you got " + game_score.ToString () + " coins.";
				
				int coins_num = Int32.Parse (coins_txt.text.Substring (2));
				coins_num += game_score;
				coins_txt.text = ": " + coins_num.ToString ();

				//dummy, t, status, stamina, sttack, HP, coins, stone
				data = now_time.ToString () + "," + now_time.ToString () + "," + 
					"5,0,0,0," + game_score.ToString () + ",0";

			}
			if (game_flag < -2 || 2 < game_flag) {
				recver_status ();

				GameObject x = GameObject.Find ("Fire");
				x.SetActive (false);
				x = GameObject.Find ("Fire2");
				x.SetActive (false);

				if (game_flag < -2) {
					game_flag = 2;
				} else {
					float gf_f = 3 / (float)game_flag;
					if (gf_f < 0.01) {
						Application.LoadLevel ("ScoreScene");
					}
					GameObject bd = GameObject.Find ("body");
					SpriteRenderer sr = bd.GetComponent<SpriteRenderer> ();
					sr.color = new Color (gf_f, gf_f, gf_f, 1f);

					bd = GameObject.Find ("wingL");
					sr = bd.GetComponent<SpriteRenderer> ();
					sr.color = new Color (gf_f, gf_f, gf_f, 1f);

					bd = GameObject.Find ("wingR");
					sr = bd.GetComponent<SpriteRenderer> ();
					sr.color = new Color (gf_f, gf_f, gf_f, 1f);
				}
				PlayerPrefs.SetInt (player_name + "_game_flag", game_flag);
				message_txt.text = "you got " + game_score.ToString () + " coins.";
				
				int coins_num = Int32.Parse (coins_txt.text.Substring (2));
				coins_num += game_score;
				coins_txt.text = ": " + coins_num.ToString ();

				//dummy, t, status, stamina, sttack, HP, coins, stone
				data = now_time.ToString () + "," + now_time.ToString () + "," + 
					"5,0,0,0," + game_score.ToString () + ",0";
			}
		}
		GetServData (data);
	}

    void Update()
    {
		username_txt.text = player_name;

		if (game_flag >= 2) {
			Vector3 g = Enemy.transform.position;
			if (Enemy_flag == false) {
				g.y -= (float)0.01;
			} else {
				g.y += (float)0.01;
			}
			if (g.y < -1.5) {
				Enemy_flag = true;
			}
			if (g.y > 0.75) {
				Enemy_flag = false;
			}
			Enemy.transform.position = g;
		}

		int next_time = GetUnixTime ();
		if (0 < (next_time - now_time)) {
			int stamina_num = Int32.Parse (stamina_txt.text.Substring (2));
			if (stamina_num < 999) {
				stamina_num += (next_time - now_time);
				stamina_txt.text = ": " + stamina_num.ToString ();
			}
			if (999 < stamina_num) {
				stamina_num = 999;
				stamina_txt.text = ": 999";
			}
			now_time = next_time;
			// save to File
			PlayerPrefs.SetInt (player_name + "_stamina", stamina_num);
			PlayerPrefs.SetInt (player_name + "_stamina_update_time", now_time);
		}

        if (!Input.GetMouseButtonDown(0))
            return;

		if (myAnimator.GetBool ("Damage") == true) {
			myAnimator.SetBool("Damage", false);
			return;
		}
		message_txt.text = "";

		// touch img
		Collider2D aCollider2d = Physics2D.OverlapPoint(Input.mousePosition);
		if (aCollider2d) {
			AudioSource AS = message.GetComponent<AudioSource>();
			AS.Play();

			GameObject obj = aCollider2d.transform.gameObject;
			if (obj.name == "stamina_Image") {
				int stamina_num = Int32.Parse(stamina_txt.text.Substring (2));
				int coins_num = Int32.Parse(coins_txt.text.Substring (2));
				if (1000 <= coins_num) {
					if (999 <= stamina_num) {
						message_txt.text = "you don't need get stamina.";
						return;
					}

					coins_num  -= 1000;
					stamina_num = 999;

					PlayerPrefs.SetInt (player_name + "_stamina", stamina_num);
					PlayerPrefs.SetInt (player_name + "_stamina_update_time", GetUnixTime());

					coins_txt.text = ": " + coins_num.ToString ();
					stamina_txt.text = ": " + stamina_num.ToString ();
					message_txt.text = "you got stamina / -1000 coins.";

					//dummy, t, status, stamina, sttack, HP, coins, stone
					PutServData(
						now_time.ToString () + "," + 
						now_time.ToString () + "," +
						"7,0,0,0,0,0");

				} else {
					message_txt.text = "you don't have 1000 coins.";
				}
				return;
			}
			if (obj.name == "attack_Image") {
				int attack_num = Int32.Parse(attack_txt.text.Substring (2));
				int stone_num = Int32.Parse(stone_txt.text.Substring (2));
				if (1 <= stone_num) {
					stone_num  -= 1;
					attack_num += 1;
					attack_txt.text = ": " + attack_num.ToString ();
					stone_txt.text = ": " + stone_num.ToString ();
					message_txt.text = "you got +1 attack / -1 stone.";

					//dummy, t, status, stamina, sttack, HP, coins, stone
					PutServData(
						now_time.ToString () + "," + 
						now_time.ToString () + "," +
						"2,0,0,0,0,0");

				} else {
					message_txt.text = "you don't have a stone.";
				}
				return;
			}
			if (obj.name == "hp_Image") {
				int hp_num = Int32.Parse(hp_txt.text.Substring (2));
				int stone_num = Int32.Parse(stone_txt.text.Substring (2));
				if (1 <= stone_num) {
					stone_num -= 1;
					hp_num    += 10;
					hp_txt.text = ": " + hp_num.ToString ();
					stone_txt.text = ": " + stone_num.ToString ();
					message_txt.text = "you got +10 HP / -1 stone.";

					//dummy, t, status, stamina, sttack, HP, coins, stone
					PutServData(
						now_time.ToString () + "," + 
						now_time.ToString () + "," +
						"1,0,0,0,0,0");

				} else {
					message_txt.text = "you don't have a stone.";
				}
				return;
			}
			if (obj.name == "coins_Image") {
				int coins_num = Int32.Parse(coins_txt.text.Substring (2));
				int stone_num = Int32.Parse(stone_txt.text.Substring (2));
				if (1 <= stone_num) {
					stone_num -= 1;
					coins_num += 1000;
					coins_txt.text = ": " + coins_num.ToString ();
					stone_txt.text = ": " + stone_num.ToString ();
					message_txt.text = "you got +1000 coins / -1 stone.";

					//dummy, t, status, stamina, sttack, HP, coins, stone
					PutServData(
						now_time.ToString () + "," + 
						now_time.ToString () + "," +
						"3,0,0,0,0,0");

				} else {
					message_txt.text = "you don't have a stone.";
				}
				return;
			}
			if (obj.name == "stone_Image") {
				message_txt.text = "you can't increase a stone.";
			}
			return;
		}

        targetPointX = Input.mousePosition.x;
    }

    void FixedUpdate()
    {
        Vector3 screen_point = Camera.main.WorldToScreenPoint(transform.position);

        if (Mathf.Abs (targetPointX - screen_point.x) <= moveStartDistance) {
			targetPointX = screen_point.x;
			myAnimator.SetBool("Run", false);
			return;
		} else {
			myAnimator.SetBool("Run", true);
		}

        float horizontal = Mathf.Sign(targetPointX - screen_point.x);
		rigi.AddForce(Vector2.right * horizontal * moveForce);

		if (Mathf.Abs (rigi.velocity.x) > maxSpeed) {
			rigi.velocity = new Vector2 (Mathf.Sign (rigi.velocity.x) * maxSpeed, rigi.velocity.y);
		}

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 local_scale = transform.localScale;
            local_scale.x *= -1;
            transform.localScale = local_scale;
        }
    }
    
	int GetUnixTime()
	{
		System.DateTime targetTime  = System.DateTime.Now.ToUniversalTime ();
		System.TimeSpan elapsedTime = targetTime - UNIX_EPOCH;
		double x = elapsedTime.TotalSeconds;
		// Debug.Log (x.ToString());
		return System.Convert.ToInt32 (x);
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Fire")
        {
            //Animator myAnimator = GetComponent<Animator>();
			myAnimator.SetBool("Damage", true);
			int stamina_num = Int32.Parse(stamina_txt.text.Substring (2));
			if (stamina_num < 500) {
				message_txt.text = "you don't have 500 stamina.";
			} else {
				stamina_num -= 500;
				stamina_txt.text = ": " + stamina_num.ToString ();

				PlayerPrefs.SetInt (player_name + "_stamina", stamina_num);
				PlayerPrefs.SetInt (player_name + "_stamina_update_time", GetUnixTime());

				// goto map
				backup_status();
				AudioSource AS = collision.gameObject.GetComponent<AudioSource> ();
				AS.Play ();

				message.SetActive(false);

				//dummy, t, status, stamina, sttack, HP, coins, stone
				CallHTTP(
					now_time.ToString () + "," + 
					now_time.ToString () + "," +
					"4,500,0,0,0,0");
			}
        }
		if (collision.gameObject.tag == "BigFire")
		{
			//Animator myAnimator = GetComponent<Animator>();
			myAnimator.SetBool("Damage", true);
			int stamina_num = Int32.Parse(stamina_txt.text.Substring (2));
			if (stamina_num < 750) {
				message_txt.text = "you don't have 750 stamina.";
			} else {
				stamina_num -= 750;
				stamina_txt.text = ": " + stamina_num.ToString ();

				PlayerPrefs.SetInt (player_name + "_stamina", stamina_num);
				PlayerPrefs.SetInt (player_name + "_stamina_update_time", GetUnixTime());

				// goto map
				backup_status();
				AudioSource AS = collision.gameObject.GetComponent<AudioSource> ();
				AS.Play ();

				message.SetActive(false);

				//dummy, t, status, stamina, sttack, HP, coins, stone
				CallHTTP(
					now_time.ToString () + "," + 
					now_time.ToString () + "," +
					"4,750,0,0,0,0");
			}
		}
		if (collision.gameObject.tag == "Dragon")
		{
			//Animator myAnimator = GetComponent<Animator>();
			myAnimator.SetBool("Damage", true);
			int stamina_num = Int32.Parse(stamina_txt.text.Substring (2));
			if (stamina_num < 999) {
				message_txt.text = "you don't have 999 stamina.";
			} else {
				stamina_num -= 999;
				stamina_txt.text = ": " + stamina_num.ToString ();

				PlayerPrefs.SetInt (player_name + "_stamina", stamina_num);
				PlayerPrefs.SetInt (player_name + "_stamina_update_time", GetUnixTime());

				backup_status();
				AudioSource AS = collision.gameObject.GetComponent<AudioSource> ();
				AS.Play ();

				message.SetActive(false);

				//dummy, t, status, stamina, sttack, HP, coins, stone
				CallHTTP(
					now_time.ToString () + "," + 
					now_time.ToString () + "," +
					"4,999,0,0,0,0");
			}
		}
	}

	void  PutServData(string data)
	{
		//dummy, t, status, stamina, sttack, HP, coins, stone
		Dictionary<string,string> postfmt = new Dictionary<string,string> ();
		postfmt ["session"] = GameCookie;
		//Debug.Log ("PutServData:" + GameCookie);
		postfmt ["data"]    = RJ256(
			System.Text.Encoding.ASCII.GetBytes (e2s(nw_key)),
			System.Text.Encoding.ASCII.GetBytes (GameCookie.Substring(0, 32)),
			data, true);
		POST3(GameCtrlURL, postfmt);
	}

	void GetServData(string data)
	{
		//dummy, t, status, stamina, sttack, HP, coins, stone
		Dictionary<string,string> postfmt = new Dictionary<string,string> ();
		postfmt ["session"] = GameCookie;
		//Debug.Log ("GetServData:" + GameCookie);
		postfmt ["data"]    = RJ256(
			System.Text.Encoding.ASCII.GetBytes (e2s(nw_key)),
			System.Text.Encoding.ASCII.GetBytes (GameCookie.Substring(0, 32)),
			data, true);
		POST(GameCtrlURL, postfmt);
	}

	void CallHTTP(string data)
	{
		Dictionary<string,string> postfmt = new Dictionary<string,string> ();
		postfmt ["session"] = GameCookie;
		//Debug.Log ("CallHTTP:" + GameCookie);
		postfmt ["data"] = RJ256(
			System.Text.Encoding.ASCII.GetBytes (e2s(nw_key)),
			System.Text.Encoding.ASCII.GetBytes (GameCookie.Substring(0, 32)),
			data, true);
		POST2(GameCtrlURL, postfmt);
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

			int fin = 0;
			for (int i=0; i < PlainData.Length; i++) {
				if (PlainData[i] == 0) {
					fin = i;
					break;
				}
			}

			// get dec data
			if (fin != 0) {
				r = System.Text.Encoding.UTF8.GetString(PlainData.Take(fin).ToArray());
			} else {
				r = System.Text.Encoding.UTF8.GetString(PlainData);
			}
		}
		
		return r;
	}

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
			if (www.text.Substring(0, 6) == "failed") {
				//Debug.Log (www.text);
				Application.LoadLevel("TitleScene");
			} else {
				//dummy, t, status, stamina, sttack, HP, coins, stone
				string x = RJ256(System.Text.Encoding.ASCII.GetBytes (
					e2s(nw_key)),
				    System.Text.Encoding.ASCII.GetBytes (GameCookie.Substring(0, 32)),
				    www.text, false);
				string [] st = x.Split(',');

				GameCookie       = st[1];
				//stamina_txt.text = ": " + st[3];
				attack_txt.text  = ": " + st[4];
				hp_txt.text      = ": " + st[5];
				coins_txt.text   = ": " + st[6];
				stone_txt.text   = ": " + st[7];
			}
		} else {
			Application.LoadLevel("TitleScene");
		}
	}

	WWW POST2(string url, Dictionary<string,string> post)
	{
		WWWForm form = new WWWForm();
		foreach(KeyValuePair<string,string> post_arg in post) {
			form.AddField(post_arg.Key, post_arg.Value);
		}
		WWW www = new WWW(url, form);
		StartCoroutine(WaitForRequest2(www));
		return www;
	}
	
	private IEnumerator WaitForRequest2(WWW www)
	{
		yield return www;
		if (www.error == null) {
			if (www.text.Substring(0, 6) == "failed") {
				//Debug.Log (www.text);
				Application.LoadLevel("TitleScene");
			} else {
				//Debug.Log (www.text);
				if(www.text.Substring(0, 7) == "succeed") {
					//Debug.Log ("WaitForRequest2:" + www.text.Substring(7));
					GameCookie = www.text.Substring(7);
					// goto map
					Application.LoadLevel("GameScene");
				} else {
					Application.LoadLevel("TitleScene");
				}
			}
		} else {
			//Debug.Log (www.error);
			Application.LoadLevel("TitleScene");
		}
	}

	WWW POST3(string url, Dictionary<string,string> post)
	{
		WWWForm form = new WWWForm();
		foreach(KeyValuePair<string,string> post_arg in post) {
			form.AddField(post_arg.Key, post_arg.Value);
		}
		WWW www = new WWW(url, form);
		StartCoroutine(WaitForRequest3(www));
		return www;
	}
	
	private IEnumerator WaitForRequest3(WWW www)
	{
		yield return www;
		if (www.error == null) {
			if (www.text.Substring(0, 6) == "failed") {
				//Debug.Log (www.text);
				Application.LoadLevel("TitleScene");
			} else {
				//Debug.Log (www.text);
				if(www.text.Substring(0, 7) == "succeed") {
					GameCookie = www.text.Substring(7);
				} else {
					Application.LoadLevel("TitleScene");
				}
			}
		} else {
			//Debug.Log (www.error);
			Application.LoadLevel("TitleScene");
		}
	}

}