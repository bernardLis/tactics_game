using Lis.Core.Utilities;
using UnityEngine.UIElements;
using DG.Tweening;


namespace Lis
{
    public class MapTooltipManager : Singleton<MapTooltipManager>
    {
        VisualElement _mapTooltipContainer;

        void Start()
        {
            _mapTooltipContainer =
                GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("mapTooltipContainer");
        }


        public void DisplayTooltip(string tooltipText)
        {
            _mapTooltipContainer.style.display = DisplayStyle.Flex;
            _mapTooltipContainer.Clear();
            _mapTooltipContainer.Add(new Label(tooltipText));

            DOTween.To(x => _mapTooltipContainer.style.opacity = x,
                _mapTooltipContainer.style.opacity.value, 1, 0.5f);
        }

        public void HideTooltip()
        {
            DOTween.To(x => _mapTooltipContainer.style.opacity = x,
                    _mapTooltipContainer.style.opacity.value, 0, 0.5f)
                .OnComplete(() => { _mapTooltipContainer.style.display = DisplayStyle.None; });
        }
    }
}