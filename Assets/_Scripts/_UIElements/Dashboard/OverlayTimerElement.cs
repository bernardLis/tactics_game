using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlayTimerElement : TimerElement
{
    const string _ussOverlayMain = _ussClassName + "overlay-main";
    const string _ussOverlayMask = _ussClassName + "overlay-mask";

    VisualElement _overlayMask;

    public OverlayTimerElement(float timeLeft, float totalTime, bool isLooping, string text)
        : base(timeLeft, totalTime, isLooping, text)
    {
        AddToClassList(_ussOverlayMain);

        _overlayMask = new();
        _overlayMask.AddToClassList(_ussOverlayMask);

        Add(_overlayMask);
        Add(GetLabelWrapper());
    }

    public void SetStyles(string mainStyle, string overlayMaskStyle, string labelWrapperStyles)
    {
        ClearClassList();
        AddToClassList(mainStyle);

        _overlayMask.ClearClassList();
        _overlayMask.AddToClassList(overlayMaskStyle);

        _labelWrapper.ClearClassList();
        _labelWrapper.AddToClassList(labelWrapperStyles);
    }

    protected override void UpdateTimer()
    {
        base.UpdateTimer();

        float h = (float)_ticksLeft / (float)_totalTicks * 100;
        _overlayMask.style.height = Length.Percent(h);
    }

}

