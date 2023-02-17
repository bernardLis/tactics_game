using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimerElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    protected const string _ussClassName = "timer-element__";


    const string _ussLabelWrapper = _ussClassName + "label-wrapper";
    const string _ussLabel = _ussClassName + "label";
    const string _ussSecondsLeftLabel = _ussClassName + "seconds-left-label";

    GameManager _gameManager;

    protected VisualElement _labelWrapper;
    Label _label;
    Label _secondsLeftLabel;
    string _text;

    protected int _totalTicks;
    protected int _ticksLeft;
    protected bool _isLooping;

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

        _text = text;

        _ticksLeft = Mathf.RoundToInt(timeLeft * 10);
        _totalTicks = Mathf.RoundToInt(totalTime * 10);
        _isLooping = isLooping;

        if (_gameManager.IsTimerOn)
            _timer = schedule.Execute(UpdateTimer).Every(100);
    }

    protected VisualElement GetLabelWrapper()
    {
        _labelWrapper = new();
        Add(_labelWrapper);
        _labelWrapper.AddToClassList(_ussLabelWrapper);

        _label = new(_text);
        _labelWrapper.Add(_label);
        _label.AddToClassList(_ussCommonTextPrimary);
        _label.AddToClassList(_ussLabel);

        _secondsLeftLabel = new();
        _labelWrapper.Add(_secondsLeftLabel);
        _secondsLeftLabel.AddToClassList(_ussCommonTextPrimary);
        _secondsLeftLabel.AddToClassList(_ussSecondsLeftLabel);

        return _labelWrapper;
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

    protected virtual void UpdateTimer()
    {
        string timeLeft = (_ticksLeft * 0.1f).ToString("F1");

        if (_secondsLeftLabel != null)
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
