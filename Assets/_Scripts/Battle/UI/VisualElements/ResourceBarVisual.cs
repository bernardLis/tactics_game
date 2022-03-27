using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceBarVisual : VisualElement
{
    VisualElement _missing;
    VisualElement _interactionResult;
    Label _text;

    public ResourceBarVisual(Color color)
    {
        _missing = new();
        _interactionResult = new();
        _text = new();

        style.backgroundColor = color;

        AddToClassList("resourceBar");
        _missing.AddToClassList("barMissingAmount");
        _interactionResult.AddToClassList("barInteractionResult");
        _text.AddToClassList("barText");

        Add(_missing);
        Add(_interactionResult);
        Add(_text);
    }

    public void SetText(string newText)
    {
        _text.text = newText;
    }


}
