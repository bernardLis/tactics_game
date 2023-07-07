using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleModifierElement : ElementWithTooltip
{
    const string _ussClassName = "battle-modifier__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";

    GameManager _gameManager;

    public BattleModifier BattleModifier { get; private set; }

    GoldElement _goldElement;

    public BattleModifierElement(BattleModifier modifier, bool justIcon = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleModifierStyles);
        if (ss != null)
            styleSheets.Add(ss);

        BattleModifier = modifier;

        Label icon = new();
        icon.AddToClassList(_ussIcon);
        icon.style.backgroundImage = new StyleBackground(BattleModifier.Icon);
        Add(icon);

        if (justIcon) return;

        AddToClassList(_ussMain);
        AddToClassList("common__button-basic");

        _goldElement = new(BattleModifier.Cost);
        Add(_goldElement);
    }

    protected override void DisplayTooltip()
    {
        Label tooltip = new(BattleModifier.Description);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }
}
