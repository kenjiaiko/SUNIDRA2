using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class EnemyGeneratorCtrl : MonoBehaviour
{
	public GameObject enemyPrefab;
	public GameObject[] existEnemys;

	public int maxEnemy = 2;

	void Start()
	{
		existEnemys = new GameObject[maxEnemy];
		StartCoroutine(Exec());
	}

	IEnumerator Exec()
	{
		while (true)
		{
			Generate();
			yield return new WaitForSeconds(3.0f);
		}
	}

	// generate enemys if it doesn't exist enemys.
	void Generate()
	{
		for (int enemyCount = 0; enemyCount < existEnemys.Length; ++ enemyCount)
		{
			if (existEnemys[enemyCount] == null)
			{
				existEnemys[enemyCount] = Instantiate(
					enemyPrefab, transform.position, transform.rotation) as GameObject;
				return;
			}
		}
	}
}
