using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleInfoManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleSpire _battleSpire;

    Spire _spire;

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
        _battleSpire = BattleSpire.Instance;

        _root = _battleManager.Root;
        _infoPanel = _root.Q<VisualElement>("infoPanel");
        _livesCountLabel = _root.Q<Label>("livesCount");

        _battleManager.OnBattleInitialized += ResolveInfoPanel;
    }

    void ResolveInfoPanel()
    {
        _infoPanel.style.opacity = 0f;
        _infoPanel.style.display = DisplayStyle.Flex;
        DOTween.To(x => _infoPanel.style.opacity = x, 0, 1, 0.5f).SetDelay(0.5f);

        if (_gameManager == null) _gameManager = GameManager.Instance;
        _spire = _gameManager.CurrentBattle.Spire;

        ResolveLivesLabel();
        UpdateLivesLabel();

        AddTroopsLimitElement();
        UpdateTroopsLimitElement();

        AddGoldElement();
        AddSpiceElement();

        ResolveBattleModifiers();
    }

    void ResolveLivesLabel()
    {
        _livesCountLabel.style.display = DisplayStyle.Flex;
        _spire.StoreyLives.CurrentLives.OnValueChanged += (v) => UpdateLivesLabel();
    }

    void UpdateLivesLabel()
    {
        _livesCountLabel.text = $"Lives: {_spire.StoreyLives.CurrentLives.Value}";
        Helpers.DisplayTextOnElement(_root, _livesCountLabel, "-1", Color.red);
    }

    void AddTroopsLimitElement()
    {
        VisualElement TroopsLimitContainer = _battleManager.Root.Q<VisualElement>("troopsLimitContainer");
        TroopsLimitContainer.style.display = DisplayStyle.Flex;

        _troopsLimitElement = new("");
        TroopsLimitContainer.Add(_troopsLimitElement);

        _gameManager.PlayerHero.OnCreatureAdded += (c) => UpdateTroopsLimitElement();
        _gameManager.PlayerHero.OnCreatureRemoved += (c) => UpdateTroopsLimitElement();

        _spire.StoreyTroops.MaxTroopsTree.CurrentValue.OnValueChanged += (v) => UpdateTroopsLimitElement();
    }

    void UpdateTroopsLimitElement()
    {
        _troopsLimitElement.UpdateCountContainer($"{_gameManager.PlayerHero.CreatureArmy.Count} / {_spire.StoreyTroops.MaxTroopsTree.CurrentValue.Value}", Color.white);
    }

    void AddGoldElement()
    {
        _goldElement = new(_gameManager.Gold);
        _gameManager.OnGoldChanged += OnGoldChanged;
        _infoPanel.Add(_goldElement);
    }

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
