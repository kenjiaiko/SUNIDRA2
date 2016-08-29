using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
	Vector2 slideStartPosition;
	Vector2 prevPosition;
	Vector2 delta = Vector2.zero;

	bool moved = false;
	
	void Update()
	{
		if (Input.GetButtonDown("Fire1"))
			slideStartPosition = GetCursorPosition();
		
		// slide checker (if user slide 10% of screen width)
		if (Input.GetButton("Fire1"))
		{
			if (Vector2.Distance(slideStartPosition,GetCursorPosition()) >= (Screen.width * 0.1f))
				moved = true;
		}

		// finish to slide
		if (!Input.GetButtonUp("Fire1") && !Input.GetButton("Fire1"))
			moved = false;

		// get length of slide distance
		if (moved)
			delta = GetCursorPosition() - prevPosition;
		else
			delta = Vector2.zero;
		
		// update the cursor
		prevPosition = GetCursorPosition();
	}
	
	// is clicked?
	public bool Clicked()
	{
		if (!moved && Input.GetButtonUp("Fire1"))
			return true;
		else
			return false;
	}	
	
	// 
	public Vector2 GetDeltaPosition()
	{
		return delta;
	}
	
	public bool Moved()
	{
		return moved;
	}
	
	public Vector2 GetCursorPosition()
	{
		return Input.mousePosition;
	}
}
