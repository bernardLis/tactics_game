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

        AddArmyGroups();

        AddMostKillsEntity();
        AddMostEntitiesAbility();
        AddMostDamageAbility();
    }

    void AddArmyGroups()
    {
        for (int i = 0; i < _gameManager.PlayerHero.Army.Count; i++)
        {
            // HERE: make a visual element out of this
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            ArmyGroup ag = _gameManager.PlayerHero.Army[i];
            // HERE: testing
            ag.KillCount = Random.Range(0, 10);

            container.Add(new ArmyGroupElement(ag));

            VisualElement killCounterContainer = new();
            container.style.alignItems = Align.Center;

            ChangingValueElement killCounter = new();
            killCounter.Initialize(ag.OldKillCount, 24);
            killCounterContainer.Add(new Label($"# of kills:"));
            killCounterContainer.Add(killCounter);

            container.Add(killCounterContainer);
            container.Add(new Label($"Evolves at:  {ag.NumberOfKillsToEvolve()}"));

            // HERE: should be some kind of evolution show
            container.schedule.Execute(() =>
            {
                Debug.Log($"execute {Time.time}");
                killCounter.ChangeAmount(ag.KillCount);
                if (ag.ShouldEvolve())
                    container.Add(new MyButton("Evolve", _ussCommonMenuButton, () =>
                    {
                        SetEnabled(false);
                        ag.Evolve();
                    }));
            }).StartingIn(1000 + 500 * i);

            Add(container);

            container.style.opacity = 0;
            DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f).SetDelay(0.5f * i);

        }
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

        DOTween.To(x => l.style.opacity = x, 0, 1, 0.5f).SetDelay(0.5f)
        .OnComplete(() =>
        {
            if (_abilityLogs.Count == 0) OnFinished?.Invoke();
        });
    }

    void AddMostEntitiesAbility()
    {
        if (_abilityLogs.Count == 0) return;

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
