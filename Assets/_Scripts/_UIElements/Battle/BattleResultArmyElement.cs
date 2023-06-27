using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using Random = UnityEngine.Random;

public class BattleResultArmyElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonButtonBasic = "common__button-basic";

    const string _ussClassName = "battle-result-army__";
    const string _ussMain = _ussClassName + "main";
    const string _ussArmyGroupContainer = _ussClassName + "army-group-container";
    const string _ussArmyStatsContainer = _ussClassName + "army-stats-container";
    const string _ussGivePickupsButton = _ussClassName + "give-pickups-button";

    GameManager _gameManager;
    AudioManager _audioManager;
    BattleManager _battleManager;
    BattleLogManager _battleLogManager;

    List<BattleLogAbility> _abilityLogs = new();

    VisualElement _pickupsContainer;

    ScrollView _armyGroupContainer;
    List<CreatureCardExp> _creatureExpElements = new();

    VisualElement _logRecordsContainer;
    public event Action OnFinished;
    public BattleResultArmyElement(VisualElement content)
    {
        _gameManager = GameManager.Instance;
        _audioManager = _gameManager.GetComponent<AudioManager>();
        _battleManager = BattleManager.Instance;
        _battleLogManager = _battleManager.GetComponent<BattleLogManager>();

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleResultArmyStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        foreach (BattleLog item in _battleLogManager.Logs)
            if (item is BattleLogAbility abilityLog)
                _abilityLogs.Add(abilityLog);

        _pickupsContainer = new();
        Add(_pickupsContainer);

        _armyGroupContainer = new();
        _armyGroupContainer.AddToClassList(_ussArmyGroupContainer);
        Add(_armyGroupContainer);
        _gameManager.PlayerHero.OnCreatureAdded += AddArmyToContainer;

        _logRecordsContainer = new();
        Add(_logRecordsContainer);

        ShowArmyStats();
        // wait for army stats to show
        this.schedule.Execute(AddMostKillsEntity).StartingIn(500 + 500 * _gameManager.PlayerHero.Army.Count);
    }

    void AddArmyToContainer(Creature creature)
    {
        VisualElement container = new();
        container.AddToClassList(_ussArmyStatsContainer);
        _armyGroupContainer.Add(container);

        CreatureIcon icon = new(creature);
        container.Add(icon);
    }
    void ShowArmyStats()
    {
        for (int i = 0; i < _gameManager.PlayerHero.Army.Count; i++)
        {
            _audioManager.PlayUIDelayed("Placing Paper", 0.5f * i);

            VisualElement container = new();
            container.AddToClassList(_ussArmyStatsContainer);
            _armyGroupContainer.Add(container);

            Creature c = _gameManager.PlayerHero.Army[i];
            CreatureCardExp creatureElement = new(c);
            container.Add(creatureElement);
            _creatureExpElements.Add(creatureElement);

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

    public void RefreshArmy()
    {
        _armyGroupContainer.Clear();
        ShowArmyStats();
    }


    public void MoveAway()
    {
        _logRecordsContainer.style.display = DisplayStyle.None;
        _pickupsContainer.style.display = DisplayStyle.None;

        foreach (CreatureCardExp el in _creatureExpElements)
            el.FoldSelf();

        style.position = Position.Absolute;
        _armyGroupContainer.style.position = Position.Absolute;
        parent.Add(_armyGroupContainer);

        _armyGroupContainer.style.left = worldBound.xMin;

        _audioManager.PlayUIDelayed("Paper Flying", 0.5f);
        DOTween.To(x => _armyGroupContainer.style.left = x, worldBound.xMin, 40, 0.5f)
            .SetDelay(0.5f)
            .SetEase(Ease.InOutFlash)
            .OnComplete(() =>
            {
                _audioManager.PlayUI("Paper Flying");
                DOTween.To(x => _armyGroupContainer.style.bottom = x, _armyGroupContainer.layout.y, 20, 0.5f);
            });
    }
}
