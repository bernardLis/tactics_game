using UnityEngine;
using UnityEngine.UIElements;

public class StatElement : ElementWithTooltip
{
    public Label Icon;
    public Label Value;

    Stat _stat;

    string _tooltipText;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "stat-element";

    const string _ussIcon = _ussClassName + "__icon";
    const string _ussValue = _ussClassName + "__value";

    // when there are no Stats => stats won't be interacted with
    public StatElement(Sprite icon, Stat stat) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StatElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _stat = stat;
        BaseStatVisual(icon);
        _stat.OnValueChanged += UpdateValue;
    }

    void BaseStatVisual(Sprite icon)
    {
        style.flexDirection = FlexDirection.Row;

        Icon = new();
        Icon.AddToClassList(_ussIcon);
        Icon.style.backgroundImage = new StyleBackground(icon);
        Add(Icon);

        Value = new();
        Value.AddToClassList(_ussValue);
        Value.AddToClassList(_ussCommonTextPrimary);
        Value.text = _stat.GetValue().ToString();
        Add(Value);

        _tooltipText = _stat.StatType.ToString();
    }

    public void UpdateValue(int value) { Value.text = value.ToString(); }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(_tooltipText));
        base.DisplayTooltip();
    }
}
