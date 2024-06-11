using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class LineTimerElement : TimerElement
    {
        private const string _ussLineMain = UssClassName + "line-main";
        private const string _ussLineWrapper = UssClassName + "line-wrapper";
        private const string _ussLine = UssClassName + "line";

        private readonly VisualElement _line;

        private readonly VisualElement _wrapper;

        public LineTimerElement(float timeLeft, float totalTime, bool isLooping, string text)
            : base(timeLeft, totalTime, isLooping, text)
        {
            AddToClassList(_ussLineMain);

            _line = new();
            Add(_line);
            _line.AddToClassList(_ussLine);

            float w = TicksLeft / (float)TotalTicks * 100;
            _line.style.width = Length.Percent(w);

            _wrapper = new();
            Add(_wrapper);
            _wrapper.AddToClassList(_ussLineWrapper);


            LabelWrapper.style.position = Position.Absolute;
            // Add(AddLabelWrapper());
        }

        public void HideLabel()
        {
            LabelWrapper.style.display = DisplayStyle.None;
        }

        public void SetStyles(string wrapperClass, string lineClass)
        {
            _wrapper.ClearClassList();
            _wrapper.AddToClassList(wrapperClass);

            _line.ClearClassList();
            _line.AddToClassList(lineClass);
        }

        public void UpdateLineStyle(string lineClass)
        {
            _line.ClearClassList();
            _line.AddToClassList(lineClass);
        }

        protected override void UpdateTimer()
        {
            base.UpdateTimer();

            float w = TicksLeft / (float)TotalTicks * 100;

            w = Mathf.Clamp(w, 0, 100);
            _line.style.width = Length.Percent(w);
        }
    }
}