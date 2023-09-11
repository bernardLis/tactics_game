using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleFinishedScreen : FullScreenElement
{
    const string _ussCommonSpacer = "common__horizontal-spacer";

    const string _ussClassName = "battle-finished__";
    const string _ussMain = _ussClassName + "main";

    BattleManager _battleManager;

    protected VisualElement _mainContainer;

    public BattleFinishedScreen()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleFinishedStyles);
        if (ss != null) styleSheets.Add(ss);

        _battleManager = BattleManager.Instance;

        _mainContainer = new();
        _mainContainer.AddToClassList(_ussMain);
        _content.Add(_mainContainer);

        AddTitle();

        _mainContainer.Add(new BattleStatsElement());
        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonSpacer);
        _mainContainer.Add(spacer);

        DisableNavigation();
    }

    protected virtual void AddTitle()
    {
        // meant to be overwritten
    }
}
