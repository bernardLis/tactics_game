using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OpponentGroupMarkerElement : ElementWithTooltip
{
    GameManager _gameManager;

    const string _ussClassName = "opponent-group-marker__";
    const string _ussMain = _ussClassName + "main";

    EnemyWave _opponentGroup;
    public OpponentGroupMarkerElement(EnemyWave opponentGroup)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.OpponentGroupMarkerStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _opponentGroup = opponentGroup;
        AddToClassList(_ussMain);

        style.backgroundImage = new StyleBackground(_opponentGroup.Icon);
        style.unityBackgroundImageTintColor = _opponentGroup.Element.Color.Color;
    }

    protected override void DisplayTooltip()
    {
        OpponentGroupCard tooltip = new(_opponentGroup);

        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
