using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
	Camera cam;

	// UI
	UIDocument UIDocument;
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

	VisualElement logContainer;
	Label logText;

	Queue<string> logQueue;
	string currentLog;
	bool showLogIsRunning;


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
		UIDocument = GetComponent<UIDocument>();
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

		// log
		logContainer = root.Q<VisualElement>("logContainer");
		logText = root.Q<Label>("logText");

		logQueue = new Queue<string>();
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

	public void HideAllUIPanels()
	{
		UIDocument.rootVisualElement.Q<VisualElement>("inventoryContainer").style.display = DisplayStyle.None;
		UIDocument.rootVisualElement.Q<VisualElement>("questUI").style.display = DisplayStyle.None;
		UIDocument.rootVisualElement.Q<VisualElement>("conversationContainer").style.display = DisplayStyle.None;
		UIDocument.rootVisualElement.Q<VisualElement>("tooltipUI").style.display = DisplayStyle.None;
		logContainer.style.display = DisplayStyle.None;
	}

	public void DisplayLogText(string newText)
	{
		// add log text to the queue
		logQueue.Enqueue(newText);

		// make sure only one ui panel is active
		if (!showLogIsRunning)
			StartCoroutine(ShowLogText());
	}

	IEnumerator ShowLogText()
	{
		showLogIsRunning = true;

		// only one can be visible.
		HideAllUIPanels();
		logContainer.style.display = DisplayStyle.Flex;

		while (logQueue.Count > 0)
		{
			currentLog = logQueue.Dequeue();
			logText.text = currentLog;
			yield return new WaitForSeconds(2f);
		}

		showLogIsRunning = false;
		logContainer.style.display = DisplayStyle.None;
		yield break;
	}
}
