using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleInfoManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;

    VisualElement _root;
    VisualElement _infoPanel;
    TroopsLimitElement _troopsLimitElement;

    GoldElement _goldElement;
    SpiceElement _spiceElement;
    Label _tilesUntilBossLabel;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

        _root = _battleManager.Root;
        _infoPanel = _root.Q<VisualElement>("infoPanel");
        _tilesUntilBossLabel = _root.Q<Label>("tilesUntilBoss");

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

        AddGoldElement();

        AddTroopsLimitElement();
        UpdateTroopsLimitElement();

        HandleTilesUntilBoss();
        // HERE: spice
        // AddSpiceElement();

        ResolveBattleModifiers();
    }

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

        _battleManager.OnPlayerCreatureAdded += (c) => UpdateTroopsLimitElement();
        _battleManager.OnPlayerEntityDeath += (c) => UpdateTroopsLimitElement();
    }

    void UpdateTroopsLimitElement()
    {
        int count = Mathf.Clamp(_battleManager.PlayerCreatures.Count - 1, 0, 9999);
        _troopsLimitElement.UpdateCountContainer($"{count}", Color.white);
    }

    void HandleTilesUntilBoss()
    {
        _battleAreaManager.OnTilePurchased += UpdateTilesUntilBossLabel;
        UpdateTilesUntilBossLabel();
    }

    void UpdateTilesUntilBossLabel()
    {
        _tilesUntilBossLabel.text = $"{_battleAreaManager.PurchasedTiles.Count} / {_battleManager.CurrentBattle.TilesUntilBoss}";
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
