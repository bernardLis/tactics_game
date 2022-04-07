using UnityEngine;
using UnityEngine.UIElements;

public class StatVisual : VisualElement
{
    public Label Icon;
    public Label Value;

    public StatVisual(Sprite icon, int value)
    {
        BaseStatVisual(icon);

        Value.text = value.ToString();
    }

    public StatVisual(Sprite icon, Stat stat)
    {
        BaseStatVisual(icon);

        Value.text = stat.GetValue().ToString();

        Value.style.color = Color.white;
        if (stat.GetValue() > stat.BaseValue)
            Value.style.color = Color.green;
        if (stat.GetValue() < stat.BaseValue)
            Value.style.color = Color.red;
    }

    void BaseStatVisual(Sprite icon)
    {
        style.flexDirection = FlexDirection.Row;

        Icon = new();
        Icon.AddToClassList("statIcon");
        Icon.style.backgroundImage = icon.texture;
        Add(Icon);

        Value = new();
        Value.AddToClassList("statValue");
        Add(Value);

        // TODO: Tooltip study
        Icon.RegisterCallback<MouseEnterEvent>((evt) => MouseEnters());
    }

    void MouseEnters()
    {
        Debug.Log($"mouse over {Value}");
    }

}
