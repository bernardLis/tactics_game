using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleInfoManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    VisualElement _root;
    VisualElement _infoPanel;
    TroopsLimitElement _troopsLimitElement;
    Label _livesCountLabel;
    GoldElement _goldElement;
    SpiceElement _spiceElement;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _root = _battleManager.Root;
        _infoPanel = _root.Q<VisualElement>("infoPanel");
        _livesCountLabel = _root.Q<Label>("livesCount");

        if (BattleIntroManager.Instance != null)
            BattleIntroManager.Instance.OnIntroFinished += ResolveInfoPanel;
        else
            _battleManager.OnBattleInitialized += ResolveInfoPanel;
    }

    void ResolveInfoPanel()
    {
        _infoPanel.style.opacity = 0f;
        _infoPanel.style.display = DisplayStyle.Flex;
        DOTween.To(x => _infoPanel.style.opacity = x, 0, 1, 0.5f).SetDelay(3f);

        AddLivesElement();
        // UpdateLivesLabel();

        AddGoldElement();

        AddTroopsLimitElement();
        // UpdateTroopsLimitElement();

        // HERE: spice
        // AddSpiceElement();

        ResolveBattleModifiers();

        AddActivePortalsElement();
    }

    void AddActivePortalsElement()
    {
        VisualElement container = new();
        container.style.height = 45;
        container.style.minWidth = 140;
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;

        _infoPanel.Add(container);

        foreach (BattleOpponentPortal portal in BattleWaveManager.Instance.OpponentPortals)
        {
            VisualElement icon = new();
            icon.style.width = 30;
            icon.style.height = 30;
            icon.style.marginLeft = 5;

            portal.OnPortalOpened += (p) =>
            {
                icon.style.backgroundImage = new StyleBackground(p.Element.Icon);
                container.Add(icon);
            };

            portal.OnPortalClosed += (p) =>
            {
                container.Remove(icon);
            };
        }

    }

    void AddLivesElement()
    {
        _infoPanel.Add(new BattleLivesElement());
    }

    // void ResolveLivesLabel()
    // {
    //     _livesCountLabel.style.display = DisplayStyle.Flex;
    //     _spire.StoreyLives.MaxLivesTree.CurrentValue.OnValueChanged += (v) => UpdateLivesLabel();
    //     _spire.StoreyLives.CurrentLives.OnValueChanged += (v) => UpdateLivesLabel();
    // }

    // void UpdateLivesLabel()
    // {
    //     _livesCountLabel.text = $"Lives: {_spire.StoreyLives.CurrentLives.Value}";
    //     Helpers.DisplayTextOnElement(_root, _livesCountLabel, $"{_spire.StoreyLives.CurrentLives.PreviousValue - _spire.StoreyLives.CurrentLives.Value}", Color.red);
    // }

    void AddGoldElement()
    {
        _goldElement = new(_gameManager.Gold);
        _gameManager.OnGoldChanged += OnGoldChanged;
        _infoPanel.Add(_goldElement);
    }

    void AddTroopsLimitElement()
    {
        _troopsLimitElement = new("");
        _infoPanel.Add(_troopsLimitElement);

        // _gameManager.PlayerHero.OnCreatureAdded += (c) => UpdateTroopsLimitElement();
        // _gameManager.PlayerHero.OnCreatureRemoved += (c) => UpdateTroopsLimitElement();

        // StoreyTroops.MaxTroopsTree.CurrentValue.OnValueChanged += (v) => UpdateTroopsLimitElement();
    }

    // void UpdateTroopsLimitElement()
    // {
    //     _troopsLimitElement.UpdateCountContainer($"{_gameManager.PlayerHero.CreatureArmy.Count} / {_spire.StoreyTroops.MaxTroopsTree.CurrentValue.Value}", Color.white);
    // }


    void AddSpiceElement()
    {
        _spiceElement = new(_gameManager.Spice);
        _gameManager.OnSpiceChanged += OnSpiceChanged;
        _infoPanel.Add(_spiceElement);
    }

    void OnGoldChanged(int newValue)
    {
        int change = newValue - _goldElement.Amount;
        Helpers.DisplayTextOnElement(_root, _goldElement, "" + change, Color.yellow);
        _goldElement.ChangeAmount(newValue);
    }

    void OnSpiceChanged(int newValue)
    {
        int change = newValue - _spiceElement.Amount;
        Helpers.DisplayTextOnElement(_root, _spiceElement, "" + change, Color.red);
        _spiceElement.ChangeAmount(newValue);
    }

    void ResolveBattleModifiers()
    {
        if (_gameManager.CurrentBattle.BattleModifiers == null) return;

        foreach (BattleModifier b in _gameManager.CurrentBattle.BattleModifiers)
            _infoPanel.Add(new BattleModifierElement(b, true));
    }
}
