using UnityEngine.UIElements;

namespace Lis.Core
{
    public class OverlayTimerElement : TimerElement
    {
        const string _ussOverlayMain = UssClassName + "overlay-main";
        const string _ussOverlayMask = UssClassName + "overlay-mask";

        readonly VisualElement _overlayMask;

        public OverlayTimerElement(float timeLeft, float totalTime, bool isLooping, string text)
            : base(timeLeft, totalTime, isLooping, text)
        {
            AddToClassList(_ussOverlayMain);

            _overlayMask = new();
            _overlayMask.AddToClassList(_ussOverlayMask);

            Insert(0, _overlayMask);
        }

        public void SetStyles(string mainStyle, string overlayMaskStyle, string labelWrapperStyles)
        {
            if (!string.IsNullOrEmpty(mainStyle))
            {
                ClearClassList();
                AddToClassList(mainStyle);
            }

            if (!string.IsNullOrEmpty(overlayMaskStyle))
            {
                _overlayMask.ClearClassList();
                _overlayMask.AddToClassList(overlayMaskStyle);
            }

            if (!string.IsNullOrEmpty(labelWrapperStyles))
            {
                LabelWrapper.ClearClassList();
                LabelWrapper.AddToClassList(labelWrapperStyles);
            }
        }

        protected override void UpdateTimer()
        {
            base.UpdateTimer();

            float h = TicksLeft / (float)TotalTicks * 100;
            _overlayMask.style.height = Length.Percent(h);
        }
    }
}