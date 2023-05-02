using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmyGroupElement : ElementWithTooltip
{
    const string _ussClassName = "army-group__";
    const string _ussMain = _ussClassName + "main";
    const string _ussCount = _ussClassName + "count";

    GameManager _gameManager;

    Label _armyCountLabel;

    public bool IsLocked { get; private set; }

    public ArmyGroup ArmyGroup;

    public ArmyGroupElement(ArmyGroup armyGroup, bool isLocked = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmyGroupElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        ArmyGroup = armyGroup;
        armyGroup.OnCountChanged += OnCountChanged;

        IsLocked = isLocked;

        AddToClassList(_ussMain);

        EntityIcon icon = new(armyGroup.ArmyEntity);
        Add(icon);

        _armyCountLabel = new($"{armyGroup.EntityCount}");
        _armyCountLabel.AddToClassList(_ussCount);
        Add(_armyCountLabel);
    }

    void OnCountChanged(int listPos, int total) { _armyCountLabel.text = $"{total}"; }

    protected override void DisplayTooltip()
    {
        EntityElement tooltip = new(ArmyGroup.ArmyEntity);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
