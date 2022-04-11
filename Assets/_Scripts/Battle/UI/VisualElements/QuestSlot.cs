using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
public class QuestSlot : VisualElement
{
    public Label Title;
    public Quest Quest;

    QuestUI _questUI;

    public QuestSlot()
    {
        AddToClassList("questSlot");

        // create a new label element and add it to the root
        Title = new Label();
        Add(Title);
        Title.AddToClassList("questTitleLabel");

        // on click
        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    public void HoldQuest(Quest _quest)
    {
        _questUI = GameUI.Instance.GetComponent<QuestUI>();

        Quest = _quest;
        Title.text = Quest.Title;
    }

    public void DropQuest()
    {
        if (Quest != null)
            Quest = null;
        if (Title.text != null)
            Title.text = null;
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
        if (_questUI != null)
        {
            _questUI.UnselectCurrent();
            _questUI.OnQuestClick(Quest);
            _questUI.SelectedQuestSlot = this;
        }

        this.AddToClassList("selectedQuestSlot");
    }

    public void Unselect()
    {
        this.RemoveFromClassList("selectedQuestSlot");
    }
}
