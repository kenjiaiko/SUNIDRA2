using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class AttackArea : MonoBehaviour
{
	CharacterStatus status;
	
	public AudioClip hitSeClip;
	AudioSource hitSeAudio;
	
	void Start()
	{
		status = transform.root.GetComponent<CharacterStatus>();

		hitSeAudio = gameObject.AddComponent<AudioSource>();
		hitSeAudio.clip = hitSeClip;
		hitSeAudio.loop = false;
	}

	public class AttackInfo
	{
		public int attackPower;
		public Transform attacker;
	}

	AttackInfo GetAttackInfo()
	{			
		AttackInfo attackInfo = new AttackInfo();
		attackInfo.attackPower = status.Power;

		if (status.powerBoost)
			attackInfo.attackPower += 7;
		
		attackInfo.attacker = transform.root;
		return attackInfo;
	}
	
	void OnTriggerEnter(Collider other)
	{
		other.SendMessage("Damage", GetAttackInfo());
		hitSeAudio.Play();
	}

	public void OnAttack()
	{
		GetComponent<Collider>().enabled = true;
	}

	public void OnAttackTermination()
	{
		GetComponent<Collider>().enabled = false;
	}
}
