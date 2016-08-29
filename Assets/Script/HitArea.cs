using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class HitArea : MonoBehaviour
{
	public void Damage(AttackArea.AttackInfo attackInfo)
	{
		transform.root.SendMessage ("Damage", attackInfo);
	}
}
