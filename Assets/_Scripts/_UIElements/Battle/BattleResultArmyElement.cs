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
    List<VisualElement> _armyStatContainers = new();

    VisualElement _logRecordsContainer;

    int _goldCollected = 0;
    int _spiceCollected = 0;
    List<Item> _itemsCollected = new();
    MyButton _givePickupsButton;
    bool _pickupsGiven;

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

        ShowPickupStats();
        ShowArmyStats();
        // wait for army stats to show
        this.schedule.Execute(AddMostKillsEntity).StartingIn(500 + 500 * _gameManager.PlayerHero.Army.Count);
    }

    void AddArmyToContainer(Creature creature)
    {
        VisualElement container = new();
        container.AddToClassList(_ussArmyStatsContainer);
        _armyGroupContainer.Add(container);

        CreatureElement creatureElement = new(creature);
        creatureElement.CreatureIcon.PlayAnimationAlways();
        container.Add(creatureElement);
    }

    void ShowPickupStats()
    {
        Label pickupCountLabel = new($"Pickups Collected: {_battleManager.CollectedPickups.Count}");
        _pickupsContainer.Add(pickupCountLabel);
        pickupCountLabel.style.opacity = 0;
        DOTween.To(x => pickupCountLabel.style.opacity = x, 0, 1, 0.5f);

        _goldCollected = 0;
        _spiceCollected = 0;
        _itemsCollected = new();

        foreach (Pickup p in _battleManager.CollectedPickups)
        {
            if (p.Gold > 0)
                _goldCollected += p.Gold;
            if (p.Spice > 0)
                _spiceCollected += p.Spice;
            if (p.Item != null)
                _itemsCollected.Add(p.Item);
        }

        VisualElement collectionContainer = new();
        collectionContainer.style.flexDirection = FlexDirection.Row;
        _pickupsContainer.Add(collectionContainer);

        GoldElement goldElement = new(_goldCollected);
        goldElement.style.opacity = 0;
        DOTween.To(x => goldElement.style.opacity = x, 0, 1, 0.5f);

        SpiceElement spiceElement = new(_spiceCollected);
        spiceElement.style.opacity = 0;
        DOTween.To(x => spiceElement.style.opacity = x, 0, 1, 0.5f).SetDelay(0.5f);

        collectionContainer.Add(goldElement);
        collectionContainer.Add(spiceElement);
        for (int i = 0; i < _itemsCollected.Count; i++)
        {
            ItemElement itemElement = new(_itemsCollected[i]);
            itemElement.style.opacity = 0;
            DOTween.To(x => itemElement.style.opacity = x, 0, 1, 0.5f).SetDelay(1f + 0.5f * i);
            collectionContainer.Add(itemElement);
        }

        _givePickupsButton = new("Collect", _ussCommonButtonBasic, GiveCollectedPickups);
        _givePickupsButton.AddToClassList(_ussGivePickupsButton);
        _pickupsContainer.Add(_givePickupsButton);
        _givePickupsButton.style.opacity = 0;
        DOTween.To(x => _givePickupsButton.style.opacity = x, 0, 1, 0.5f)
                .SetDelay(1.5f + 0.5f * _itemsCollected.Count);
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
            CreatureExpElement creatureElement = new(c);
            creatureElement.CreatureIcon.PlayAnimationAlways();
            container.Add(creatureElement);

            VisualElement statsContainer = new();
            _armyStatContainers.Add(statsContainer);
            container.Add(statsContainer);

            Label kills = new($"Kills: {c.TotalKillCount - c.OldKillCount}");
            Label damageDealt = new($"Damage Dealt: {c.TotalDamageDealt - c.OldDamageDealt}");
            Label damageTaken = new($"Damage Taken: {c.TotalDamageTaken - c.OldDamageTaken}");
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
        GiveCollectedPickups();

        DOTween.To(x => _pickupsContainer.style.opacity = x, 1, 0, 0.5f)
                .OnComplete(() => _pickupsContainer.style.display = DisplayStyle.None);

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

    void GiveCollectedPickups()
    {
        if (_pickupsGiven) return;
        _pickupsGiven = true;
        _givePickupsButton.SetEnabled(false);
        DOTween.To(x => _givePickupsButton.style.opacity = x, 1, 0, 0.5f);

        _gameManager.ChangeGoldValue(_goldCollected);
        _gameManager.ChangeSpiceValue(_spiceCollected);
        for (int i = 0; i < _itemsCollected.Count; i++)
            _gameManager.PlayerHero.AddItem(_itemsCollected[i]);
    }
}
