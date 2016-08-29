using UnityEngine;
using System.Collections;

public class TargetCursor : MonoBehaviour
{
	// character cursol
	public float radius = 1.0f;
	public float angularVelocity = 480.0f;

	public Vector3 destination = new Vector3( 0.0f, 0.5f, 0.0f );
	Vector3 position = new Vector3( 0.0f, 0.5f, 0.0f );
	float angle = 0.0f;
	
	public void SetPosition(Vector3 iPosition)
	{
		destination = iPosition;
		destination.y = 0.5f;
	}
	
	void Start()
	{
		SetPosition(transform.position);
		position = destination;
	}

	// Update is called once per frame
	void Update ()
	{
		position += (destination - position) / 10.0f;
		angle += angularVelocity * Time.deltaTime;
		Vector3 offset = Quaternion.Euler(0.0f, angle, 0.0f) * new Vector3(0.0f, 0.0f, radius);
		transform.localPosition =  position + offset;
	}
}
