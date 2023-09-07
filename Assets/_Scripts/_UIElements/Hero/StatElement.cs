using UnityEngine;
using UnityEngine.UIElements;

public class StatElement : ElementWithTooltip
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "stat-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";
    const string _ussValue = _ussClassName + "value";

    GameManager _gameManager;

    public Label Icon;
    public Label Value;

    Stat _stat;

    string _tooltipText;

    public StatElement(Stat stat) : base()
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StatElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        _stat = stat;
        BaseStatVisual();
        _stat.OnValueChanged += UpdateValue;
    }

    void BaseStatVisual()
    {
        Icon = new();
        Icon.AddToClassList(_ussIcon);
        Icon.style.backgroundImage = new StyleBackground(_stat.Icon);
        Add(Icon);

        Value = new();
        Value.AddToClassList(_ussValue);
        Value.AddToClassList(_ussCommonTextPrimary);
        Value.text = _stat.GetValue().ToString();
        Add(Value);

        string description = _stat.Description;
        if (description.Length == 0)
            description = _stat.StatType.ToString();
        _tooltipText = description;
    }

    public void UpdateValue(int value) { Value.text = value.ToString(); }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(_tooltipText));
        base.DisplayTooltip();
    }
}
