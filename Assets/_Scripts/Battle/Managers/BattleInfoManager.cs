using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleInfoManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleBase _battleBase;

    Base _base;

    VisualElement _root;
    VisualElement _infoPanel;
    TroopsLimitElement _troopsLimitElement;
    Label _livesCountLabel;
    GoldElement _goldElement;
    SpiceElement _spiceElement;

    public void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleBase = BattleBase.Instance;

        _base = _gameManager.SelectedBattle.Base;

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
        Debug.Log($"_base {_base}");
        Debug.Log($"_base.LivesUpgrade {_base.LivesUpgrade}");
        Debug.Log($"_base.LivesUpgrade.CurrentLives {_base.LivesUpgrade.CurrentLives}");
        Debug.Log($"_base.LivesUpgrade.CurrentLives.Value {_base.LivesUpgrade.CurrentLives.Value}");

        _base.LivesUpgrade.CurrentLives.OnValueChanged += (v) => UpdateLivesLabel();
    }

    void UpdateLivesLabel()
    {
        _livesCountLabel.text = $"Lives: {_base.LivesUpgrade.CurrentLives.Value}";
    }

    void AddTroopsLimitElement()
    {
        VisualElement TroopsLimitContainer = _battleManager.Root.Q<VisualElement>("troopsLimitContainer");
        TroopsLimitContainer.style.display = DisplayStyle.Flex;

        _troopsLimitElement = new("");
        TroopsLimitContainer.Add(_troopsLimitElement);

        _gameManager.PlayerHero.OnCreatureAdded += (c) => UpdateTroopsLimitElement();
        _base.TroopsUpgrade.CurrentLimit.OnValueChanged += (v) => UpdateTroopsLimitElement();
    }

    void UpdateTroopsLimitElement()
    {
        _troopsLimitElement.UpdateCountContainer($"{_gameManager.PlayerHero.CreatureArmy.Count} / {_base.TroopsUpgrade.CurrentLimit.Value}", Color.white);
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
        if (_gameManager.SelectedBattle.BattleModifiers == null) return;

        foreach (BattleModifier b in _gameManager.SelectedBattle.BattleModifiers)
            _infoPanel.Add(new BattleModifierElement(b, true));
    }
}
