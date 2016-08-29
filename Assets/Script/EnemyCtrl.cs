using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class EnemyCtrl : MonoBehaviour 
{
	CharacterStatus status;
	CharaAnimation charaAnimation;
	CharacterMove characterMove;
	Transform attackTarget;
	public GameObject hitEffect;

	GameRuleCtrl gameRuleCtrl;
	
	// standby time is 2
	public float waitBaseTime = 2.0f;
	float waitTime;

	public float walkRange = 5.0f;

	public Vector3 basePosition;
	public GameObject[] dropItemPrefab;

	enum State {
		Walking,
		Chasing,
		Attacking,
		Died,
	};
	
	State state = State.Walking;
	State nextState = State.Walking;

	public AudioClip deathSeClip;
	AudioSource deathSeAudio;
	
	
	// Use this for initialization
	void Start ()
	{
		status         = GetComponent<CharacterStatus>();
		charaAnimation = GetComponent<CharaAnimation>();
		characterMove  = GetComponent<CharacterMove>();
		gameRuleCtrl   = FindObjectOfType<GameRuleCtrl>();

		basePosition = transform.position;
		waitTime = waitBaseTime;
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (state)
		{
		case State.Walking:
			Walking();
			break;
		case State.Chasing:
			Chasing();
			break;
		case State.Attacking:
			Attacking();
			break;
		}
		
		if (state != nextState)
		{
			state = nextState;
			switch (state)
			{
			case State.Walking:
				WalkStart();
				break;
			case State.Chasing:
				ChaseStart();
				break;
			case State.Attacking:
				AttackStart();
				break;
			case State.Died:
				Died();
				break;
			}
		}
	}

	void ChangeState(State nextState)
	{
		this.nextState = nextState;
	}
	
	void WalkStart()
	{
		StateStartCommon();
	}
	
	void Walking()
	{
		if (waitTime > 0.0f)
		{
			waitTime -= Time.deltaTime;

			if (waitTime <= 0.0f)
			{
				// set next destination
				Vector2 randomValue = Random.insideUnitCircle * walkRange;
				Vector3 destinationPosition = 
					basePosition + new Vector3(randomValue.x, 0.0f, randomValue.y);
				SendMessage("SetDestination", destinationPosition);
			}
		}
		else
		{
			if (characterMove.Arrived())
			{
				waitTime = Random.Range(waitBaseTime, waitBaseTime * 2.0f);
			}

			if (attackTarget)
			{
				ChangeState(State.Chasing);
			}
		}
	}

	void ChaseStart()
	{
		StateStartCommon();
	}

	void Chasing()
	{
		SendMessage("SetDestination", attackTarget.position);
		if (Vector3.Distance( attackTarget.position, transform.position ) <= 2.0f)
		{
			ChangeState(State.Attacking);
		}
	}

	void AttackStart()
	{
		StateStartCommon();
		status.attacking = true;
		
		Vector3 targetDirection = (attackTarget.position - transform.position).normalized;
		SendMessage("SetDirection", targetDirection);
		SendMessage("StopMove");
	}
	
	void Attacking()
	{
		if (charaAnimation.IsAttacked())
			ChangeState(State.Walking);
		waitTime = Random.Range(waitBaseTime, waitBaseTime * 2.0f);
		attackTarget = null;
	}
	
	void dropItem()
	{
		if (dropItemPrefab.Length == 0)
		{
			return;
		}

		int dropRate = Random.Range (0, 100);

		if (attackTarget == null)
		{
			// drop a item in 8% if player killed a enemy
			if (dropRate < 8)
			{
				GameObject dropItem = dropItemPrefab [Random.Range (0, dropItemPrefab.Length)];
				Instantiate (dropItem, transform.position, Quaternion.identity);
			}
			return;
		}

		GameObject go = attackTarget.root.gameObject;
		CharacterStatus player = go.GetComponent<CharacterStatus> ();

		if (player.HP < 20) {
			GameObject dropItem = dropItemPrefab [0]; //GREEN
			Instantiate (dropItem, transform.position, Quaternion.identity);
			return;
		}

		if (player.HP < 75 && dropRate < 50) {
			GameObject dropItem = dropItemPrefab [Random.Range (0, dropItemPrefab.Length)];
			Instantiate (dropItem, transform.position, Quaternion.identity);
			return;
		}

		if (dropRate < 10) {
			GameObject dropItem = dropItemPrefab [Random.Range (0, dropItemPrefab.Length)];
			Instantiate (dropItem, transform.position, Quaternion.identity);
			return;
		}
	}
	
	void Died()
	{
		status.died = true;

		dropItem();
		Destroy(gameObject);

		if (gameObject.tag == "Boss")
		{
			gameRuleCtrl.GameClear();
		}
		
		AudioSource.PlayClipAtPoint(deathSeClip, transform.position);
	}
	
	public void Damage(AttackArea.AttackInfo attackInfo)
	{
		GameObject effect = Instantiate(
			hitEffect, transform.position, Quaternion.identity) as GameObject;
		effect.transform.localPosition = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
		Destroy(effect, 0.3f);
		
		status.HP -= attackInfo.attackPower;

		if (status.HP <= 0)
		{
			status.HP = 0;
			ChangeState(State.Died);
		}
	}

	void StateStartCommon()
	{
		status.attacking = false;
		status.died      = false;
	}

	public void SetAttackTarget(Transform target)
	{
		attackTarget = target;
	}
}
