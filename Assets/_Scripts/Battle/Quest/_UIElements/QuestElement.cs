using DG.Tweening;
using Lis.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Quest
{
    public class QuestElement : ElementWithTooltip
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        readonly Quest _quest;
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
            icon.style.backgroundImage = new(q.GetIcon());
            Add(icon);

            _countLabel = new();
            _countLabel.AddToClassList(_ussCommonTextPrimary);
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

            parent.Insert(0, this);
            style.alignSelf = Align.Center;

            DOTween.To(x => transform.scale = x * Vector3.one, 1, 1.5f, 1f)
                .SetEase(Ease.OutBack)
                .OnComplete(RemoveFromHierarchy);
        }

        protected override void DisplayTooltip()
        {
            _tooltip = new(this, new Label(_quest.GetDescription()));
            base.DisplayTooltip();
        }
    }
}