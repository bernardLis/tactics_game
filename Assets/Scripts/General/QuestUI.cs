using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;

public class QuestUI : MonoBehaviour
{
	public UIDocument UIDocument;
	VisualElement questUI;
	VisualElement activeQuestsElement;
	VisualElement completedQuestsElement;
	VisualElement failedQuestsElement;

	public InputMaster controls;

	GameObject player;

	QuestManager questManager;


	void Awake()
	{
		var root = UIDocument.rootVisualElement;
		questUI = root.Q<VisualElement>("questUI");
		activeQuestsElement = root.Q<VisualElement>("activeQuests");
		completedQuestsElement = root.Q<VisualElement>("completedQuests");
		failedQuestsElement = root.Q<VisualElement>("failedQuests");

		controls = new InputMaster();
		controls.FMPlayer.EnableQuestUI.performed += ctx => EnableQuestUI();

		controls.UI.Test.performed += ctx => Test();
		controls.UI.DisableQuestUI.performed += ctx => DisableQuestUI();

		player = GameObject.FindGameObjectWithTag("Player");

		questManager = GameManager.instance.GetComponent<QuestManager>();
	}

	void OnEnable()
	{
		controls.FMPlayer.Enable();
	}

	void OnDisable()
	{
		controls.FMPlayer.Disable();
	}

	void EnableQuestUI()
	{
		PopulateQuestUI();
		questUI.style.display = DisplayStyle.Flex;
		// TODO: only controls.FMPlayer.Disable() does not disable player controlls
		controls.FMPlayer.Disable();
		PauseGame();

		controls.UI.Enable();
	}

	void DisableQuestUI()
	{
		questUI.style.display = DisplayStyle.None;

		controls.FMPlayer.Enable();
		ResumeGame();

		controls.UI.Disable();
	}

	void PopulateQuestUI()
	{
		List<Quest> activeQuests = questManager.ReturnActiveQuests();
		foreach (Quest quest in activeQuests)
		{
			Label questName = new Label(quest.qName);
			print(questName.GetClasses().FirstOrDefault());

			activeQuestsElement.Add(questName);
		}

		List<Quest> completedQuests = questManager.ReturnCompletedQuests();
		foreach (Quest quest in completedQuests)
		{
			Label questName = new Label(quest.qName);

			completedQuestsElement.Add(questName);
		}

		List<Quest> failedQuests = questManager.ReturnFailedQuests();
		foreach (Quest quest in failedQuests)
		{
			Label questName = new Label(quest.qName);
			failedQuestsElement.Add(questName);
		}
	}

	void Test()
	{
		print("testtest");
	}

	void PauseGame()
	{
		player.SetActive(false);
		Time.timeScale = 0;
	}

	void ResumeGame()
	{
		player.SetActive(true);
		Time.timeScale = 1;
	}

}
