using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class AttackAreaActivator : MonoBehaviour
{
	Collider[] attackAreaColliders;
	
	public AudioClip attackSeClip;
	AudioSource attackSeAudio;
	
	void Start ()
	{
		AttackArea[] attackAreas = GetComponentsInChildren<AttackArea>();
		attackAreaColliders = new Collider[attackAreas.Length];
		
		for (int attackAreaCnt = 0; attackAreaCnt < attackAreas.Length; attackAreaCnt++) {
			attackAreaColliders[attackAreaCnt] = attackAreas[attackAreaCnt].GetComponent<Collider>();
			attackAreaColliders[attackAreaCnt].enabled = false;
		}
		
		attackSeAudio = gameObject.AddComponent<AudioSource>();
		attackSeAudio.clip = attackSeClip;
		attackSeAudio.loop = false;
	}
	
	public void StartAttackHit()
	{
		foreach (Collider attackAreaCollider in attackAreaColliders)
			attackAreaCollider.enabled = true;
		attackSeAudio.Play();
	}

	public void EndAttackHit()
	{
		foreach (Collider attackAreaCollider in attackAreaColliders)
			attackAreaCollider.enabled = false;
	}
}
