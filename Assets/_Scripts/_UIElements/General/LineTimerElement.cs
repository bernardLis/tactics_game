using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineTimerElement : TimerElement
{
    const string _ussLineMain = _ussClassName + "line-main";
    const string _ussWrapper = _ussClassName + "wrapper";
    const string _ussLine = _ussClassName + "line";

    VisualElement _wrapper;

    VisualElement _line;

    public LineTimerElement(float timeLeft, float totalTime, bool isLooping, string text)
        : base(timeLeft, totalTime, isLooping, text)
    {

        AddToClassList(_ussLineMain);

        _line = new();
        Add(_line);
        _line.AddToClassList(_ussLine);

        float w = (float)_ticksLeft / (float)_totalTicks * 100;
        _line.style.width = Length.Percent(w);

        _wrapper = new();
        Add(_wrapper);
        _wrapper.AddToClassList(_ussWrapper);

        Add(GetLabelWrapper());
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

        float w = (float)_ticksLeft / (float)_totalTicks * 100;

        w = Mathf.Clamp(w, 0, 100);
        _line.style.width = Length.Percent(w);
    }

}

