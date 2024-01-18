using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class StarRankElement : ElementWithTooltip
    {
        List<VisualElement> _stars = new();
        int _maxRank;
        public int Rank;
        VisualElement _tooltipElement;

        const string _ussClassName = "star-rank-element__";
        const string _ussStar = _ussClassName + "star";
        const string _ussStarGold = _ussClassName + "star-gold";
        const string _ussStarGray = _ussClassName + "star-gray";

        const string _ussEffect = _ussClassName + "effect";

        int _currentStarIndex;

        IVisualElementScheduledItem _rankUpdateScheduler;

        public StarRankElement(int rank, float scale = 1f, VisualElement tooltip = null, int maxRank = 5)
        {
            var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StarRankElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            style.flexDirection = FlexDirection.Row;
            transform.scale = Vector3.one * scale;

            _maxRank = maxRank;
            Rank = rank;

            for (int i = 0; i < maxRank; i++)
            {
                VisualElement star = new();
                star.AddToClassList(_ussStar);
                star.AddToClassList(_ussStarGray);
                Add(star);
                _stars.Add(star);
            }
            SetRank(rank);

            if (tooltip != null)
                _tooltipElement = tooltip;
        }

        public void SetRank(int rank)
        {
            if (_rankUpdateScheduler != null)
            {
                _rankUpdateScheduler.Pause();
                _currentStarIndex = 0;
            }
            _rankUpdateScheduler = schedule.Execute(UpdateStar).Every(100);

            if (rank < 0)
                return;
            if (rank > _maxRank)
                return;
            Rank = rank;
        }

        void UpdateStar()
        {
            VisualElement star = _stars[_currentStarIndex];
            star.AddToClassList(_ussStarGray);
            star.RemoveFromClassList(_ussStarGold);
            star.AddToClassList(_ussEffect);
            if (_currentStarIndex < Rank)
            {
                star.RemoveFromClassList(_ussStarGray);
                star.AddToClassList(_ussStarGold);
            }

            if (_currentStarIndex > 0)
                _stars[_currentStarIndex - 1].RemoveFromClassList(_ussEffect);
            _currentStarIndex++;

            if (_currentStarIndex == _stars.Count)
            {
                _rankUpdateScheduler.Pause();
                _rankUpdateScheduler = schedule.Execute(ResetRankUpdate).StartingIn(100);
            }
        }

        void ResetRankUpdate()
        {
            _stars[_stars.Count - 1].RemoveFromClassList(_ussEffect);
            _currentStarIndex = 0;
        }

        protected override void DisplayTooltip()
        {
            if (_tooltipElement != null)
            {
                _tooltip = new(this, _tooltipElement);
            }
            else
            {
                Label l = new Label("Rank tooltip missing.");
                l.style.whiteSpace = WhiteSpace.Normal;
                _tooltip = new(this, l);
            }

            base.DisplayTooltip();
        }

    }
}
