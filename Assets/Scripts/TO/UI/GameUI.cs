using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
	Camera cam;

	// UI
	public UIDocument UIDocument;
	VisualElement tileInfo;
	Label tileTextLabel;

	VisualElement characterInfo;
	Label nameLabel;
	Label currentHealthLabel;
	Label currentManaLabel;
	Label moveRangeLabel;
	Label armorLabel;
	Label strengthLabel;
	Label intelligenceLabel;

	VisualElement rushUI;

	VisualElement logContainer;
	Label logText;

	#region Singleton
	public static GameUI instance;
	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of UIDocument found");
			return;
		}
		instance = this;

		#endregion

		// TODO: Supposedly, this is an expensive call
		cam = Camera.main;

		// getting ui elements
		var root = UIDocument.rootVisualElement;
		tileInfo = root.Q<VisualElement>("tileInfo");

		tileTextLabel = root.Q<Label>("tileText");

		characterInfo = root.Q<VisualElement>("characterInfo");

		nameLabel = root.Q<Label>("nameLabel");
		currentHealthLabel = root.Q<Label>("currentHealthLabel");
		currentManaLabel = root.Q<Label>("currentManaLabel");
		moveRangeLabel = root.Q<Label>("moveRangeLabel");
		armorLabel = root.Q<Label>("armorLabel");
		strengthLabel = root.Q<Label>("strengthLabel");
		intelligenceLabel = root.Q<Label>("intelligenceLabel");

		rushUI = root.Q<VisualElement>("rushUI");

		// log
		logContainer = root.Q<VisualElement>("logContainer");
		logText = root.Q<Label>("logText");
	}


	public void UpdateTileInfoUI(string tileText)
	{
		tileTextLabel.text = tileText;
	}

	public void UpdateCharacterInfoUI(int currentHealth,
		int maxHealth, int currentMana, int maxMana)
	{
		currentHealthLabel.text = currentHealth + "/" + maxHealth;
		currentManaLabel.text = currentMana + "/" + maxMana;
	}

	public void ShowTileInfoUI()
	{
		//show UI;
		tileInfo.style.display = DisplayStyle.Flex;
	}
	public void HideTileInfoUI()
	{
		// hide UI;
		tileInfo.style.display = DisplayStyle.None;
	}
	public void ShowCharacterInfoUI()
	{
		//show UI;
		characterInfo.style.display = DisplayStyle.Flex;
	}
	public void HideCharacterInfoUI()
	{
		// hide UI;
		characterInfo.style.display = DisplayStyle.None;
	}

	public void DrawRushUI(Vector3 pos, Vector2 rushVector, float percentToLimit)
	{
		rushUI.style.display = DisplayStyle.Flex;

		// showing over the limit color
		rushUI.style.backgroundColor = Color.Lerp(Color.white, Color.red, percentToLimit);

		// TODO: this UI sucks.
		// direction
		Vector2 dir = rushVector.normalized;
		float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg - 90;
		rushUI.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		// position
		Vector3 posAdjusted = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);

		if (angle >= -135 && angle < -45)
		{
			posAdjusted = new Vector3(pos.x, pos.y + 0.5f, 0f);
		}
		else if (angle >= -45 && angle < 45)
		{
			posAdjusted = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
		}
		else if ((angle >= 45 && angle < 91) || angle < -225)
		{
			posAdjusted = new Vector3(pos.x, pos.y, 0f);
		}
		else if (angle >= -225 && angle < -135)
		{
			posAdjusted = new Vector3(pos.x - 0.5f, pos.y, 0f);
		}


		Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(rushUI.panel, posAdjusted, cam);
		rushUI.transform.position = newPosition;

		// length
		float len = (Mathf.Abs(rushVector.x) + Mathf.Abs(rushVector.y)) * 50; // TODO: this is incorrect
		rushUI.style.width = len;
	}

	public void HideRushUI()
	{
		rushUI.style.display = DisplayStyle.None;
	}

	public void DisplayLogText(string newText)
	{
		logText.text = newText;
		logContainer.style.display = DisplayStyle.Flex;

		Invoke("HideLogText", 2);
	}

	void HideLogText()
	{
		logContainer.style.display = DisplayStyle.None;
	}
}
