using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

public class TitleSceneCtrl : MonoBehaviour {
    
    public Texture2D bgTexture;
	public static string playername = "";
	public static string systemcode = "";
	public static string GameCookie = "";
	public static string crypto_key = "";

	private static string GameCtrlURL = "https://cedec.seccon.jp/2016/sign-in";
	private static string GameCtrlURL2= "https://cedec.seccon.jp/2016/";

	private string pwd = "";

	private string user_err = "";
	private string pass_err = "";
	
	public static string get_playername()
	{
		return playername;
	}

	public static string get_systemcode()
	{
		return systemcode;
	}

	public static string get_gamecookie()
	{
		return GameCookie;
	}

	public static string get_crypto_key()
	{
		return crypto_key;
	}

	void Start()
	{
		if (Application.platform == RuntimePlatform.Android) {
			System.IO.FileStream fs = new System.IO.FileStream (
				Application.dataPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			byte[] bs = new byte[fs.Length];
			fs.Read (bs, 0, bs.Length);
			fs.Close ();
			SHA1 sha = new SHA1CryptoServiceProvider ();
			byte[] hashBytes = sha.ComputeHash (bs);
			systemcode = System.Convert.ToBase64String (hashBytes);
		}

		Dictionary<string,string> postfmt = new Dictionary<string,string> ();
		postfmt ["code"] = systemcode;
		POST2(GameCtrlURL2, postfmt);

		playername = PlayerPrefs.GetString ("USER", "");
		pwd = PlayerPrefs.GetString ("PASS", "");
	}

	void Update()
	{
	}

	void OnGUI()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / 854.0f, Screen.height / 480.0f, 1.0f));

        GUI.DrawTexture(new Rect(0.0f, 0.0f, 854.0f, 480.0f), bgTexture);

        if (GUI.Button(new Rect(327, 290, 200, 54), "Start", buttonStyle))
        {
			user_err = "";
			pass_err = "";

			if (playername.Length < 4) {
				user_err = " (err: length)";
			} else {
				if (pwd.Length < 4) {
					pass_err = " (err: length)";
				} else {
					Dictionary<string,string> postfmt = new Dictionary<string,string> ();
					postfmt ["user"] = playername;
					postfmt ["pass"] = pwd;
					POST(GameCtrlURL, postfmt);
				}
			}
        }

		GUIStyle style = new GUIStyle();
		style.fontSize = 16;
		style.normal.textColor = Color.gray;

		GUI.Label(new Rect(327, 150, 200, 30), "Username"+user_err, style);
		playername = GUI.TextField(new Rect(327, 170, 200, 30), playername, 20);

		GUI.Label(new Rect(327, 210, 200, 30), "Password"+pass_err, style);
		pwd = GUI.PasswordField(new Rect(327, 230, 200, 30), pwd, '*', 20);
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
			if (www.text == "failed") {
				user_err = " (err: failed login)";
				pass_err = " (err: failed login)";
			} else {
				GameCookie = www.text;
				PlayerController.set_where_from(1);

				PlayerPrefs.SetString ("USER", playername);
				PlayerPrefs.SetString ("PASS", pwd);

				if (crypto_key != "") {
					Application.LoadLevel("2dScene");
				}
			}
		} else {
			user_err = " (err: network unreachable)";
			pass_err = " (err: network unreachable)";
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
			crypto_key = new string(www.text.Reverse().ToArray());
		}
	}
}
