using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineTimerElement : TimerElement
{
    const string _ussLineMain = _ussClassName + "line-main";
    const string _ussWrapper = _ussClassName + "wrapper";
    const string _ussLineMaskWrapper = _ussClassName + "line-mask-wrapper";
    const string _ussLine = _ussClassName + "line";
    const string _ussLineMask = _ussClassName + "line-mask";


    VisualElement _wrapper;

    VisualElement _line;
    VisualElement _lineMask;

    VisualElement _lineMaskWrapper;

    public LineTimerElement(float timeLeft, float totalTime, bool isLooping, string text)
        : base(timeLeft, totalTime, isLooping, text)
    {

        AddToClassList(_ussLineMain);

        _line = new();
        Add(_line);
        _line.AddToClassList(_ussLine);

        _lineMaskWrapper = new();
        Add(_lineMaskWrapper);
        _lineMaskWrapper.AddToClassList(_ussLineMaskWrapper);

        _lineMask = new();
        _lineMaskWrapper.Add(_lineMask);
        _lineMask.AddToClassList(_ussLineMask);

        float w = 100 - (float)_ticksLeft / (float)_totalTicks * 100;
        _lineMask.style.width = Length.Percent(w);

        _wrapper = new();
        Add(_wrapper);
        _wrapper.AddToClassList(_ussWrapper);

        Add(GetLabelWrapper());
    }

    public void SetStyles(string wrapperClass, string lineClass, string lineMaskWrapperClass, string lineMaskClass)
    {
        _wrapper.ClearClassList();
        _wrapper.AddToClassList(wrapperClass);

        _line.ClearClassList();
        _line.AddToClassList(lineClass);

        _lineMaskWrapper.ClearClassList();
        _lineMaskWrapper.AddToClassList(lineMaskWrapperClass);

        _lineMask.ClearClassList();
        _lineMask.AddToClassList(lineMaskClass);
    }

    public void UpdateLineStyle(string lineClass)
    {
        _line.ClearClassList();
        _line.AddToClassList(lineClass);
    }

    protected override void UpdateTimer()
    {
        base.UpdateTimer();

        float w = 100 - (float)_ticksLeft / (float)_totalTicks * 100;
        w = Mathf.Clamp(w, 0, 100);
        _lineMask.style.width = Length.Percent(w);
    }

}

