using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmyElement : ElementWithTooltip
{
    const string _ussClassName = "army";
    const string _ussMain = _ussClassName + "__main";

    GameManager _gameManager;

    Label _armyCountLabel;

    public ArmyGroup ArmyGroup;

    public ArmyElement(ArmyGroup armyGroup)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmyElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        ArmyGroup = armyGroup;
        armyGroup.OnCountChanged += OnCountChanged;

        style.backgroundImage = new StyleBackground(armyGroup.ArmyEntity.Icon);
        style.width = 70;
        style.height = 70;

        _armyCountLabel = new($"{armyGroup.EntityCount}");
        _armyCountLabel.style.color = Color.white;
        _armyCountLabel.style.fontSize = 36;
        _armyCountLabel.style.position = Position.Absolute;
        Add(_armyCountLabel);
    }

    void OnCountChanged(int total)
    {
        _armyCountLabel.text = $"{total}";
    }


    protected override void DisplayTooltip()
    {
        VisualElement tooltip = new();
        tooltip.Add(new Label(("bla bla")));

        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
