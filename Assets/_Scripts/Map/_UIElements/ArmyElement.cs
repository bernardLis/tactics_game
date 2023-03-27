using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmyElement : ElementWithTooltip
{
    const string _ussClassName = "army__";
    const string _ussMain = _ussClassName + "main";
    const string _ussCount = _ussClassName + "count";

    GameManager _gameManager;

    Label _armyCountLabel;

    public bool IsLocked { get; private set; }

    public ArmyGroup ArmyGroup;

    public ArmyElement(ArmyGroup armyGroup, bool isLocked = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmyElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        ArmyGroup = armyGroup;
        armyGroup.OnCountChanged += OnCountChanged;

        IsLocked = isLocked;

        AddToClassList(_ussMain);
        style.backgroundImage = new StyleBackground(armyGroup.ArmyEntity.Icon);

        _armyCountLabel = new($"{armyGroup.EntityCount}");
        _armyCountLabel.AddToClassList(_ussCount);
        Add(_armyCountLabel);
    }

    void OnCountChanged(int total) { _armyCountLabel.text = $"{total}"; }

    protected override void DisplayTooltip()
    {
        VisualElement tooltip = new();
        tooltip.Add(new Label(("bla bla")));

        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
