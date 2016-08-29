using UnityEngine;
using System.Collections;
using System;

public class CharacterStatus : MonoBehaviour
{
	public int HP    = 100;
	public int MaxHP = 100;
	public int Power = 10;

	// Any Character have a last attack target(character).
	public GameObject lastAttackTarget = null;

	public string characterName = "Player";

	public bool attacking  = false;
	public bool died       = false;
	public bool powerBoost = false;

	float powerBoostTime = 0.0f;

	// effect handle
	ParticleSystem powerUpEffect;
	
	public void GetItem(DropItem.ItemKind itemKind)
	{
		switch (itemKind)
		{
		case DropItem.ItemKind.Attack:
			// character power up 
			powerBoostTime = 30.0f;
			powerUpEffect.Play ();
			break;
		
		case DropItem.ItemKind.Heal:
			// character is recovered guy's HP of (MaxHP / 2)
			HP = Mathf.Min(HP + MaxHP / 2, MaxHP);
			break;
		}
	}
	
	void Start()
	{
		if (gameObject.tag == "Player")
		{
			string s = PlayerController.get_player_status ();
			string[] v = s.Split(',');
			HP    = Int32.Parse(v[0].Substring (2));
			MaxHP = Int32.Parse(v[0].Substring (2));
			Power = Int32.Parse(v[1].Substring (2));

			powerUpEffect = transform.Find("PowerUpEffect").GetComponent<ParticleSystem>();
		}
	}
	
	void Update()
	{
		if (gameObject.tag != "Player")
			return;

		powerBoost = false;
		if (powerBoostTime > 0.0f)
		{
			powerBoost = true;
			powerBoostTime = Mathf.Max(powerBoostTime - Time.deltaTime, 0.0f);
		}
		else
		{
			powerUpEffect.Stop();
		}
	}
	
}
