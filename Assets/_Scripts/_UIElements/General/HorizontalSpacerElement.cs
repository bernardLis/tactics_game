using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class HorizontalSpacerElement : VisualElement
{
    const string _ussCommonSpacer = "common__horizontal-spacer";

    public HorizontalSpacerElement()
    {
        AddToClassList(_ussCommonSpacer);
    }
}
