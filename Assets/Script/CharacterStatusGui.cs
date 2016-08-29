using UnityEngine;
using System.Collections;

public class CharacterStatusGui : MonoBehaviour
{
	CharacterStatus playerStatus;

	// character name GUI
	Rect nameRect = new Rect(0f, 0f, 120f, 15f);
	public GUIStyle nameLabelStyle;

	// life bar GUI
	public Texture backLifeBarTexture;
	public Texture frontLifeBarTexture;
    
	float frontLifeBarOffsetX = 2f;
	float lifeBarTextureWidth = 128f;

	Rect playerLifeBarRect = new Rect(0f, 0f, 128f, 10f);
	Rect enemyLifeBarRect  = new Rect(0f, 0f, 128f, 10f);

	Color playerFrontLifeBarColor = Color.green;
	Color enemyFrontLifeBarColor  = Color.red;

	// camera component
	Camera ca;

	void Start()
	{
		GameObject c = GameObject.Find("Main Camera");
		ca = c.GetComponent<Camera> ();
	}

	// Draw player's status
	void DrawPlayerStatus()
	{
		DrawCharacterStatus(playerStatus, playerLifeBarRect, playerFrontLifeBarColor);
	}

	//  Draw a enemy(player's last attack target) status
	void DrawEnemyStatus()
	{
		if (playerStatus.lastAttackTarget != null)
		{
			CharacterStatus target_status = 
				playerStatus.lastAttackTarget.GetComponent<CharacterStatus>();
			DrawCharacterStatus(target_status, enemyLifeBarRect, enemyFrontLifeBarColor);
		}
	}

    void DrawCharacterStatus(CharacterStatus status, Rect bar_rect, Color front_color)
    {
		Vector3 a = status.transform.position;
		a.y += 2;
		Vector3 pos = ca.WorldToScreenPoint (a);

		float x = pos.x;
		float y = ca.pixelHeight - pos.y;

		if (status.characterName == TitleSceneCtrl.get_playername())
			x -= 140;
		else
			x += 20;

        // draw name
        GUI.Label(
            new Rect(x, y, nameRect.width, nameRect.height),
			status.characterName,
            nameLabelStyle);

		// get the life of the character.
		float life_value = (float)status.HP / status.MaxHP;

		// draw life bar
		if(backLifeBarTexture != null)
		{
			y += nameRect.height;
			GUI.DrawTexture(new Rect(
				x, 
				y, 
				bar_rect.width, 
				bar_rect.height), backLifeBarTexture);
		}
		if(frontLifeBarTexture != null)
		{
			float resize_front_bar_offset_x = 
				frontLifeBarOffsetX * bar_rect.width / lifeBarTextureWidth;
			float front_bar_width = bar_rect.width - resize_front_bar_offset_x * 2;

			var gui_color = GUI.color;
			GUI.color = front_color;

			GUI.DrawTexture(new Rect(
				x + resize_front_bar_offset_x, 
				y, 
				front_bar_width * life_value, 
				bar_rect.height), frontLifeBarTexture);

			GUI.color = gui_color;
		}
	}

	void Awake()
	{
		PlayerCtrl player_ctrl = GameObject.FindObjectOfType(typeof(PlayerCtrl)) as PlayerCtrl;
		playerStatus = player_ctrl.GetComponent<CharacterStatus>();
	}

	void OnGUI()
	{
		DrawPlayerStatus();
		DrawEnemyStatus();
	}
}
