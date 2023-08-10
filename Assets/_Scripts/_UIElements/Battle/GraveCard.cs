using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GraveCard : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "grave-card__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    VisualElement _leftPanel;
    VisualElement _rightPanel;

    PurchaseButton _resurrectButton;

    Creature _creature;

    public event Action OnResurrected;
    public GraveCard(Creature creature)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.GraveCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _creature = creature;

        _leftPanel = new();
        _rightPanel = new();
        Add(_leftPanel);
        Add(_rightPanel);

        PopulateLeftPanel();
        PopulateRightPanel();
    }

    void PopulateLeftPanel()
    {
        EntityIcon icon = new(_creature);
        _leftPanel.Add(icon);
    }

    void PopulateRightPanel()
    {
        _resurrectButton = new(_creature.Level * 200, buttonText: "Resurrect", callback: Resurrect);
        _rightPanel.Add(_resurrectButton);
    }

    void Resurrect()
    {
        OnResurrected?.Invoke();
    }
}
