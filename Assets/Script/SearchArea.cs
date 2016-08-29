using UnityEngine;
using System.Collections;

public class SearchArea : MonoBehaviour
{
	EnemyCtrl enemyCtrl;

	void Start()
	{
		enemyCtrl = transform.root.GetComponent<EnemyCtrl>();
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			enemyCtrl.SetAttackTarget(other.transform);
		}
	}
}
