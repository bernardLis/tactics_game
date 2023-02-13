using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimerElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "timer-element__";
    const string _ussMain = _ussClassName + "main";

    const string _ussLineMaskWrapper = _ussClassName + "line-mask-wrapper";
    const string _ussLine = _ussClassName + "line";
    const string _ussLineMask = _ussClassName + "line-mask";

    const string _ussWrapper = _ussClassName + "wrapper";

    const string _ussLabelWrapper = _ussClassName + "label-wrapper";
    const string _ussLabel = _ussClassName + "label";
    const string _ussSecondsLeftLabel = _ussClassName + "seconds-left-label";

    GameManager _gameManager;

    VisualElement _line;
    VisualElement _lineMask;
    Label _label;
    Label _secondsLeftLabel;


    int _totalTicks;
    int _ticksLeft;
    bool _isLooping;

    IVisualElementScheduledItem _timer;

    public event Action OnLoopFinished;
    public event Action OnTimerFinished;
    public TimerElement(float timeLeft, float totalTime, bool isLooping, string text)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnTimerStateChanged += OnTimerStateChanged;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TimerElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        _line = new();
        Add(_line);
        _line.AddToClassList(_ussLine);

        VisualElement lineMaskWrapper = new();
        Add(lineMaskWrapper);
        lineMaskWrapper.AddToClassList(_ussLineMaskWrapper);

        _lineMask = new();
        lineMaskWrapper.Add(_lineMask);
        _lineMask.AddToClassList(_ussLineMask);

        VisualElement wrapper = new();
        Add(wrapper);
        wrapper.AddToClassList(_ussWrapper);

        VisualElement labelWrapper = new();
        Add(labelWrapper);
        labelWrapper.AddToClassList(_ussLabelWrapper);

        _label = new(text);
        labelWrapper.Add(_label);
        _label.AddToClassList(_ussCommonTextPrimary);
        _label.AddToClassList(_ussLabel);

        _secondsLeftLabel = new();
        labelWrapper.Add(_secondsLeftLabel);
        _secondsLeftLabel.AddToClassList(_ussCommonTextPrimary);
        _secondsLeftLabel.AddToClassList(_ussSecondsLeftLabel);

        _ticksLeft = Mathf.RoundToInt(timeLeft * 10);
        _totalTicks = Mathf.RoundToInt(totalTime * 10);
        _isLooping = isLooping;

        float w = 100 - (float)_ticksLeft / (float)_totalTicks * 100;
        _lineMask.style.width = Length.Percent(w);

        _timer = schedule.Execute(UpdateTimer).Every(100);
    }

    public void UpdateTimerValues(float timeLeft, float totalTime)
    {
        _ticksLeft = Mathf.RoundToInt(timeLeft * 10);
        _totalTicks = Mathf.RoundToInt(totalTime * 10);
    }

    public float GetTimeLeft() { return _ticksLeft * 0.1f; }

    public void UpdateLabel(string txt) { _label.text = txt; }

    void OnTimerStateChanged(bool isOn)
    {
        if (_timer == null)
            return;

        if (isOn)
            _timer.Resume();
        else
            _timer.Pause();
    }

    void UpdateTimer()
    {
        float w = 100 - (float)_ticksLeft / (float)_totalTicks * 100;
        _lineMask.style.width = Length.Percent(w);
        string timeLeft = (_ticksLeft * 0.1f).ToString("F1");
        _secondsLeftLabel.text = timeLeft;

        _ticksLeft--;
        if (_ticksLeft == -1)
        {
            if (_isLooping)
                FinishLoop();
            else
                FinishTimer();
        }
    }

    void FinishLoop()
    {
        _ticksLeft = _totalTicks;
        OnLoopFinished?.Invoke();
    }

    void FinishTimer()
    {
        _timer.Pause();
        OnTimerFinished?.Invoke();
    }
}
