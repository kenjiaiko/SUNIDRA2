using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour
{
	public enum ItemKind
	{
		Attack,
		Heal,
	};
	public ItemKind kind;

	// sound handle
	public AudioClip itemSeClip;
	
	void OnTriggerEnter(Collider other)
	{
		// is player get a item?
		if(other.tag == "Player"){
			CharacterStatus aStatus = other.GetComponent<CharacterStatus>();
			aStatus.GetItem(kind);

			// delete the item
			Destroy(gameObject);
			AudioSource.PlayClipAtPoint(itemSeClip, transform.position);
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		Vector3 velocity = Random.insideUnitSphere * 2.0f + Vector3.up * 8.0f;
		GetComponent<Rigidbody>().velocity = velocity;
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
