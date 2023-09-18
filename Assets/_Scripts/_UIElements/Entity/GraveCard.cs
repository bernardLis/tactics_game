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
    BattleManager _battleManager;

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

        _battleManager = BattleManager.Instance;

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
        _resurrectButton = new(_creature.Level.Value * 200, buttonText: "Resurrect");
        _resurrectButton.OnPurchased += Resurrect;

        UpdateResurrectButton();
        _gameManager.PlayerHero.OnCreatureAdded += (c) => UpdateResurrectButton();
        _gameManager.PlayerHero.OnCreatureRemoved += (c) => UpdateResurrectButton();
        // HERE: troops BattleSpire.Instance.Spire.StoreyTroops.MaxTroopsTree.CurrentValue.OnValueChanged += (v) => UpdateResurrectButton();

        _rightPanel.Add(_resurrectButton);
    }

    void UpdateResurrectButton()
    {
        _resurrectButton.UnblockPurchase();
        if (_battleManager.IsPlayerTeamFull()) _resurrectButton.BlockPurchase("Your team is full.");
    }

    void Resurrect()
    {
        _battleManager.PlayerHero.AddCreature(_creature);
        OnResurrected?.Invoke();
    }
}
