using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Lis
{
    public class QuestElement : VisualElement
    {
        Quest _quest;
        readonly Label _countLabel;

        public QuestElement(Quest q)
        {
            _quest = q;
            style.flexDirection = FlexDirection.Row;
            style.alignContent = Align.Center;
            style.alignItems = Align.Center;

            Label icon = new();
            icon.style.width = 40;
            icon.style.height = 40;
            icon.style.backgroundImage = new StyleBackground(q.GetIcon());
            Add(icon);

            _countLabel = new Label();
            _countLabel.style.fontSize = 20;
            Add(_countLabel);
            UpdateCountLabel();

            q.OnQuestUpdated += UpdateCountLabel;
            q.OnQuestCompleted += DestroySelf;
        }

        void UpdateCountLabel()
        {
            _countLabel.text = $"{_quest.TotalAmount - _quest.CurrentAmount}/{_quest.TotalAmount}";
        }

        void DestroySelf()
        {
            _quest.OnQuestUpdated -= UpdateCountLabel;
            _quest.OnQuestCompleted -= DestroySelf;
            RemoveFromHierarchy();
        }
    }
}