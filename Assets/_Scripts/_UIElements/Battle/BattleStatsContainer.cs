using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using Random = UnityEngine.Random;

public class BattleStatsContainer : VisualElement
{
    const string _ussCommonMenuButton = "common__menu-button";

    GameManager _gameManager;
    BattleLogManager _battleLogManager;

    List<BattleLogAbility> _abilityLogs = new();

    public event Action OnFinished;
    public BattleStatsContainer()
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);

        _battleLogManager = BattleManager.Instance.GetComponent<BattleLogManager>();

        foreach (BattleLog item in _battleLogManager.Logs)
            if (item is BattleLogAbility abilityLog)
                _abilityLogs.Add(abilityLog);

        AddMostKillsEntity();

        if (_abilityLogs.Count == 0)
        {
            OnFinished?.Invoke();
            return;
        }
        AddMostEntitiesAbility();
        AddMostDamageAbility();
    }


    void AddMostKillsEntity()
    {
        List<BattleEntity> entities = new(BattleManager.Instance.PlayerEntities);

        BattleEntity entityWithMostKills = entities[0];
        int topKillCount = entities[0].KilledEnemiesCount;
        foreach (BattleEntity e in entities)
        {
            if (e.KilledEnemiesCount > topKillCount)
            {
                topKillCount = e.KilledEnemiesCount;
                entityWithMostKills = e;
            }
        }
        Label l = new($"Entity With Most Kills: {entityWithMostKills.name}, # kills: {topKillCount} ");
        Add(l);
        l.style.opacity = 0;

        DOTween.To(x => l.style.opacity = x, 0, 1, 0.5f).SetDelay(0.5f);
    }

    void AddMostEntitiesAbility()
    {
        List<BattleLogAbility> copy = new(_abilityLogs.OrderByDescending(a => a.NumberOfAffectedEntities).ToList());
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;

        container.Add(new Label("Ability That Affected Most Entities: "));
        container.Add(new AbilityIcon(copy[0].Ability));
        container.Add(new Label($"# affected entities: {copy[0].NumberOfAffectedEntities}"));
        Add(container);

        container.style.opacity = 0;
        DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f).SetDelay(1f);
    }

    void AddMostDamageAbility()
    {
        if (_abilityLogs.Count == 0) return;

        List<BattleLogAbility> copy = new(_abilityLogs.OrderByDescending(a => a.DamageDealt));
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;

        container.Add(new Label("Ability That Dealt Most Damage: "));
        container.Add(new AbilityIcon(copy[0].Ability));
        container.Add(new Label($"# damage dealt: {copy[0].DamageDealt}"));
        Add(container);

        container.style.opacity = 0;
        DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f)
        .SetDelay(1.5f)
        .OnComplete(() => OnFinished?.Invoke());
    }
}
