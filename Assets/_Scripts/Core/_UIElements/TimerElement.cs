using System;
using Lis.Arena.Fight;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class TimerElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        protected const string UssClassName = "timer-element__";

        const string _ussLabelWrapper = UssClassName + "label-wrapper";
        const string _ussLabel = UssClassName + "label";
        const string _ussSecondsLeftLabel = UssClassName + "seconds-left-label";
        readonly FightManager _fightManager;

        readonly bool _isLooping;
        readonly string _text;

        readonly IVisualElementScheduledItem _timer;
        Label _label;
        Label _secondsLeftLabel;

        protected VisualElement LabelWrapper;
        protected int TicksLeft;

        protected int TotalTicks;

        protected TimerElement(float timeLeft, float totalTime, bool isLooping, string text)
        {
            GameManager gameManager = GameManager.Instance;
            _fightManager = FightManager.Instance;
            StyleSheet ss = gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TimerElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _text = text;

            TicksLeft = Mathf.RoundToInt(timeLeft * 10);
            TotalTicks = Mathf.RoundToInt(totalTime * 10);
            _isLooping = isLooping;

            _timer = schedule.Execute(UpdateTimer).Every(100);
            if (!_fightManager.IsTimerOn)
                _timer.Pause();

            _fightManager.OnGamePaused += Pause;
            _fightManager.OnGameResumed += Resume;

            AddLabelWrapper();
        }

        public event Action OnLoopFinished;
        public event Action OnTimerFinished;

        protected VisualElement AddLabelWrapper()
        {
            LabelWrapper = new();
            Add(LabelWrapper);
            LabelWrapper.AddToClassList(_ussLabelWrapper);

            _label = new(_text);
            LabelWrapper.Add(_label);
            _label.AddToClassList(_ussCommonTextPrimary);
            _label.AddToClassList(_ussLabel);

            _secondsLeftLabel = new();
            LabelWrapper.Add(_secondsLeftLabel);
            _secondsLeftLabel.AddToClassList(_ussCommonTextPrimary);
            _secondsLeftLabel.AddToClassList(_ussSecondsLeftLabel);

            return LabelWrapper;
        }

        public void UpdateTimerValues(float timeLeft, float totalTime)
        {
            TicksLeft = Mathf.RoundToInt(timeLeft * 10);
            TotalTicks = Mathf.RoundToInt(totalTime * 10);
        }

        public float GetTimeLeft()
        {
            return TicksLeft * 0.1f;
        }

        public void UpdateLabel(string txt)
        {
            _label.text = txt;
        }

        public void SetTimerFontSize(int size)
        {
            _secondsLeftLabel.style.fontSize = size;
        }

        void Pause()
        {
            _timer.Pause();
        }

        public void Resume()
        {
            _timer.Resume();
        }

        protected virtual void UpdateTimer()
        {
            string timeLeft = (TicksLeft * 0.1f).ToString("F1");

            if (_secondsLeftLabel != null)
                _secondsLeftLabel.text = timeLeft;

            TicksLeft--;
            if (TicksLeft <= -1)
            {
                if (_isLooping)
                    FinishLoop();
                else
                    FinishTimer();
            }
        }

        void FinishLoop()
        {
            TicksLeft = TotalTicks;
            OnLoopFinished?.Invoke();
        }

        void FinishTimer()
        {
            _timer.Pause();
            OnTimerFinished?.Invoke();
        }
    }
}