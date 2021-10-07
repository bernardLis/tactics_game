using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
public class QuestSlot : VisualElement
{
	public Label title;
	public Quest quest;

	QuestUI questUI;

	public QuestSlot()
	{
		AddToClassList("questSlot");

		// create a new label element and add it to the root
		title = new Label();
		Add(title);
		title.AddToClassList("questTitleLabel");

		// on click
		RegisterCallback<PointerDownEvent>(OnPointerDown);
	}

	public void HoldQuest(Quest _quest)
	{
		questUI = GameUI.instance.GetComponent<QuestUI>();

		quest = _quest;
		title.text = quest.qName;
	}

	public void DropQuest()
	{
		if (quest != null)
			quest = null;
		if (title.text != null)
			title.text = null;
	}

	void OnPointerDown(PointerDownEvent evt)
	{
		// || item == null
		if (evt.button != 0)
			return;

		Select();
	}

	public void Select()
	{
		Debug.Log("clicked on " + quest.qName);

		this.style.backgroundColor = Color.magenta;

		if (questUI != null)
		{
			questUI.OnQuestClick(quest);
			questUI.selectedQuestSlot = this;
		}


	}

	public void Unselect()
	{
		this.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
	}
}
