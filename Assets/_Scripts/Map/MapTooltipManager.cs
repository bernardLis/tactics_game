using DG.Tweening;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Map
{
    public class MapTooltipManager : Singleton<MapTooltipManager>
    {
        VisualElement _mapTooltipContainer;

        string _tooltipHideId = "tooltipHide";

        void Start()
        {
            _mapTooltipContainer =
                GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("mapTooltipContainer");
        }


        public void DisplayTooltip(string tooltipText)
        {
            if (_mapTooltipContainer == null) return;

            _mapTooltipContainer.style.display = DisplayStyle.Flex;
            _mapTooltipContainer.Clear();
            _mapTooltipContainer.Add(new Label(tooltipText));
            DOTween.Kill(_tooltipHideId);

            DOTween.To(x => _mapTooltipContainer.style.opacity = x,
                _mapTooltipContainer.style.opacity.value, 1, 0.5f);
        }

        public void HideTooltip()
        {
            if (_mapTooltipContainer == null) return;

            DOTween.To(x => _mapTooltipContainer.style.opacity = x,
                    _mapTooltipContainer.style.opacity.value, 0, 0.5f)
                .SetId(_tooltipHideId)
                .OnComplete(() => { _mapTooltipContainer.style.display = DisplayStyle.None; });
        }
    }
}