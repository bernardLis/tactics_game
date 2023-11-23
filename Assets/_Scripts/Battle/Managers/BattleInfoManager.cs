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
    TroopsLimitElement _troopsCounter;

    GoldElement _goldElement;
    Label _tilesUntilBossLabel;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

        _root = _battleManager.Root;
        _infoPanel = _root.Q<VisualElement>("infoPanel");
        _tilesUntilBossLabel = _root.Q<Label>("tilesUntilBoss");

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
    }

    void AddGoldElement()
    {
        _goldElement = new(_gameManager.Gold);
        _gameManager.OnGoldChanged += OnGoldChanged;
        _infoPanel.Add(_goldElement);
    }

    void AddTroopsLimitElement()
    {
        _troopsCounter = new("");
        _infoPanel.Add(_troopsCounter);

        _battleManager.OnPlayerCreatureAdded += (c) => UpdateTroopsLimitElement();
        _battleManager.OnPlayerEntityDeath += (c) => UpdateTroopsLimitElement();
    }

    void UpdateTroopsLimitElement()
    {
        int count = Mathf.Clamp(_battleManager.PlayerCreatures.Count - 1, 0, 9999);
        _troopsCounter.UpdateCountContainer($"{count}", Color.white);
    }

    void HandleTilesUntilBoss()
    {
        _battleAreaManager.OnTilePurchased += UpdateTilesUntilBossLabel;
        UpdateTilesUntilBossLabel();
    }

    void UpdateTilesUntilBossLabel()
    {
        _tilesUntilBossLabel.text = $"{_battleAreaManager.PurchasedTiles.Count - 1} / {_battleManager.CurrentBattle.TilesUntilBoss}";
    }

    void OnGoldChanged(int newValue)
    {
        int change = newValue - _goldElement.Amount;
        Helpers.DisplayTextOnElement(_root, _goldElement, "" + change, Color.yellow);
        _goldElement.ChangeAmount(newValue);
    }
}
