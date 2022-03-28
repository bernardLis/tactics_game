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

    public void DisplayMissingAmount(int total, int current)
    {
        _missing.style.display = DisplayStyle.Flex;
        float missingPerc = ((float)total - (float)current) / (float)total;
        missingPerc = Mathf.Clamp(missingPerc, 0, 1);
        _missing.style.width = Length.Percent(missingPerc * 100);

        SetText($"{current}/{total}");
    }

    public void DisplayInteractionResult(int total, int current, int result)
    {
        DisplayMissingAmount(total, current);
        _interactionResult.style.display = DisplayStyle.Flex;
    }

    public void SetText(string newText)
    {
        _text.text = newText;
    }


}
