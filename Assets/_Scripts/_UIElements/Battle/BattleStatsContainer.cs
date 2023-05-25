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


    const string _ussClassName = "battle-stats-container__";
    const string _ussMain = _ussClassName + "main";
    const string _ussArmyGroupContainer = _ussClassName + "army-group-container";


    const string _ussArmyStatsContainer = _ussClassName + "army-stats-container";

    GameManager _gameManager;
    BattleManager _battleManager;
    BattleLogManager _battleLogManager;

    List<BattleLogAbility> _abilityLogs = new();

    ScrollView _armyGroupContainer;
    List<VisualElement> _armyStatContainers = new();

    VisualElement _logRecordsContainer;

    VisualElement _content;

    public event Action OnFinished;
    public BattleStatsContainer(VisualElement content)
    {
        _content = content; // HERE: different name

        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleLogManager = _battleManager.GetComponent<BattleLogManager>();

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleStatsContainer);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        foreach (BattleLog item in _battleLogManager.Logs)
            if (item is BattleLogAbility abilityLog)
                _abilityLogs.Add(abilityLog);

        _armyGroupContainer = new();
        _armyGroupContainer.AddToClassList(_ussArmyGroupContainer);
        Add(_armyGroupContainer);
        _gameManager.PlayerHero.OnArmyAdded += AddArmyToContainer;

        _logRecordsContainer = new();
        Add(_logRecordsContainer);

        ShowArmyStats();
        // wait for army stats to show
        this.schedule.Execute(AddMostKillsEntity).StartingIn(500 + 500 * _gameManager.PlayerHero.Army.Count);
    }

    void AddArmyToContainer(ArmyGroup armyGroup)
    {
        VisualElement container = new();
        container.AddToClassList(_ussArmyStatsContainer);
        _armyGroupContainer.Add(container);

        ArmyGroup ag = armyGroup;
        ArmyGroupElement armyGroupElement = new(ag);
        armyGroupElement.EntityIcon.PlayAnimationAlways();
        container.Add(armyGroupElement);
    }

    void ShowArmyStats()
    {
        for (int i = 0; i < _gameManager.PlayerHero.Army.Count; i++)
        {
            VisualElement container = new();
            container.AddToClassList(_ussArmyStatsContainer);
            _armyGroupContainer.Add(container);

            ArmyGroup ag = _gameManager.PlayerHero.Army[i];
            ArmyGroupElement armyGroupElement = new(ag);
            armyGroupElement.EntityIcon.PlayAnimationAlways();
            container.Add(armyGroupElement);

            VisualElement statsContainer = new();
            _armyStatContainers.Add(statsContainer);
            container.Add(statsContainer);

            Label kills = new($"Kills: {ag.TotalKillCount - ag.OldKillCount}");
            Label damageDealt = new($"Damage Dealt: {ag.TotalDamageDealt - ag.OldDamageDealt}");
            Label damageTaken = new($"Damage Taken: {ag.TotalDamageTaken - ag.OldDamageTaken}");
            statsContainer.Add(kills);
            statsContainer.Add(damageDealt);
            statsContainer.Add(damageTaken);

            container.style.opacity = 0;
            DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f)
                    .SetDelay(0.5f * i);
        }
    }

    void AddMostKillsEntity()
    {
        List<BattleEntity> entities = new(_battleManager.PlayerEntities);
        if (entities.Count == 0)
        {
            OnFinished?.Invoke();
            return;
        }

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
        _logRecordsContainer.Add(l);

        l.style.opacity = 0;
        DOTween.To(x => l.style.opacity = x, 0, 1, 0.5f)
                .OnComplete(() => AddMostEntitiesAbility());
    }

    void AddMostEntitiesAbility()
    {
        if (_abilityLogs.Count == 0)
        {
            OnFinished?.Invoke();
            return;
        }

        List<BattleLogAbility> copy = new(_abilityLogs.OrderByDescending(a => a.NumberOfAffectedEntities).ToList());
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;

        container.Add(new Label("Ability That Affected Most Entities: "));
        container.Add(new AbilityIcon(copy[0].Ability));
        container.Add(new Label($"# affected entities: {copy[0].NumberOfAffectedEntities}"));
        _logRecordsContainer.Add(container);

        container.style.opacity = 0;
        DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f)
                .OnComplete(() => AddMostDamageAbility());
    }

    void AddMostDamageAbility()
    {
        if (_abilityLogs.Count == 0)
        {
            OnFinished?.Invoke();
            return;
        }

        List<BattleLogAbility> copy = new(_abilityLogs.OrderByDescending(a => a.DamageDealt));
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;

        container.Add(new Label("Ability That Dealt Most Damage: "));
        container.Add(new AbilityIcon(copy[0].Ability));
        container.Add(new Label($"# damage dealt: {copy[0].DamageDealt}"));
        _logRecordsContainer.Add(container);

        container.style.opacity = 0;
        DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f)
            .OnComplete(() => OnFinished?.Invoke());
    }

    public void MoveAway()
    {
        DOTween.To(x => _logRecordsContainer.style.opacity = x, 1, 0, 0.5f)
                .OnComplete(() => _logRecordsContainer.style.display = DisplayStyle.None);
        foreach (VisualElement el in _armyStatContainers)
        {
            DOTween.To(x => el.style.opacity = x, 1, 0, 0.5f)
                    .OnComplete(() => el.style.display = DisplayStyle.None);
        }
        style.position = Position.Absolute;
        _armyGroupContainer.style.position = Position.Absolute;
        parent.Add(_armyGroupContainer);

        _armyGroupContainer.style.left = worldBound.xMin;
        DOTween.To(x => _armyGroupContainer.style.left = x, worldBound.xMin, 40, 1f)
            .SetDelay(0.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                DOTween.To(x => _armyGroupContainer.style.bottom = x, _armyGroupContainer.layout.y, 20, 1f);
            });
    }
}
