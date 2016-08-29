using UnityEngine;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class CharaAnimation : MonoBehaviour
{
	Animator animator;
	CharacterStatus status;
	Vector3 prePosition;

	bool isDown   = false;
	bool attacked = false;
	
	public bool IsAttacked()
	{
		return attacked;
	}
	
	public void StartAttackHit()
	{
		//Debug.Log ("StartAttackHit");
	}
	
	public void EndAttackHit()
	{
		//Debug.Log ("EndAttackHit");
	}
	
	public void EndAttack()
	{
		attacked = true;
	}
	
	void Start ()
	{
		animator = GetComponent<Animator>();
		status = GetComponent<CharacterStatus>();
		prePosition = transform.position;
	}
	
	void Update ()
	{
		if (Time.time < 1f)
		{
			animator.SetFloat ("Speed", 0f);
		}
		else
		{
			Vector3 delta_position = transform.position - prePosition;
			animator.SetFloat ("Speed", delta_position.magnitude / Time.deltaTime);
		}
		
		if(attacked && !status.attacking)
		{
			attacked = false;
		}
		animator.SetBool("Attacking", (!attacked && status.attacking));
		
		if(!isDown && status.died)
		{
			isDown = true;
			animator.SetTrigger("Down");
		}
		
		prePosition = transform.position;
	}
}