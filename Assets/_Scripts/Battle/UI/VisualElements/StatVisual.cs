using UnityEngine;
using UnityEngine.UIElements;

public class StatVisual : VisualElement
{
    public Label Icon;
    public Label Value;

    public StatVisual(Sprite icon, int value)
    {
        style.flexDirection = FlexDirection.Row;

        Icon = new();
        Icon.AddToClassList("statIcon");
        Icon.style.backgroundImage = icon.texture;

        Value = new();
        Value.AddToClassList("statValue");
        Value.text = value.ToString();

        Add(Icon);
        Add(Value);
    }

}
