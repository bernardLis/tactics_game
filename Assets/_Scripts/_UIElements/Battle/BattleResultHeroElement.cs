using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BattleResultHeroElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";
    const string _ussCommonButtonBasic = "common__button-basic";

    const string _ussClassName = "battle-result-hero__";
    const string _ussMain = _ussClassName + "main";
    const string _ussCardContainer = _ussClassName + "card-container";
    const string _ussShowContainer = _ussClassName + "show-container";
    const string _ussPickupsContainer = _ussClassName + "pickups-container";

    const string _ussDefeatedEntitiesContainer = _ussClassName + "defeated-entities-container";
    const string _ussScoreContainer = _ussClassName + "score-container";
    const string _ussGivePickupsButton = _ussClassName + "give-pickups-button";

    GameManager _gameManager;
    AudioManager _audioManager;
    BattleManager _battleManager;

    Battle _selectedBattle;

    Hero _playerHero;

    VisualElement _heroContainer;
    HeroCardExp _heroCard;

    VisualElement _showContainer;
    VisualElement _defeatedEntitiesContainer;

    VisualElement _pickupsContainer;
    GoldElement _goldElement;
    SpiceElement _spiceElement;
    int _goldCollected = 0;
    int _spiceCollected = 0;
    List<Item> _itemsCollected = new();
    bool _pickupsGiven;

    IVisualElementScheduledItem _enemiesKilledShowSchedule;
    int _enemyIndex;

    List<MinionArmyCard> _killedMinionArmies = new();

    public HeroCardMini HeroCardMini;

    public event Action OnFinished;
    public BattleResultHeroElement()
    {
        _gameManager = GameManager.Instance;
        _audioManager = _gameManager.GetComponent<AudioManager>();
        _battleManager = BattleManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardExpStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _playerHero = _gameManager.PlayerHero;
        _selectedBattle = _gameManager.SelectedBattle;

        AddToClassList(_ussMain);

        AddWinner();

        _showContainer = new();
        _showContainer.AddToClassList(_ussShowContainer);
        Add(_showContainer);

        AddPickupsContainer();
        AddOpponentContainer();

        RunShow();
    }

    void RunShow()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Pause();
        mySequence.Append(DOTween.To(x => _heroContainer.style.opacity = x, 0, 1, 0.5f));
        mySequence.Append(DOTween.To(x => _defeatedEntitiesContainer.style.opacity = x, 0, 1, 0.5f));
        mySequence.AppendCallback(() => ResolveKilledEntities());
        mySequence.Play();
    }

    void AddWinner()
    {
        _heroContainer = new();
        _heroContainer.AddToClassList(_ussCardContainer);
        Add(_heroContainer);

        Label title = new Label("Battle won!");
        title.style.fontSize = 64;
        title.AddToClassList(_ussCommonTextPrimary);
        _heroContainer.Add(title);

        _heroCard = new HeroCardExp(_playerHero);
        _heroCard.OnPointAdded += OnHeroPointAdded;
        _heroContainer.Add(_heroCard);
    }

    void AddOpponentContainer()
    {
        _defeatedEntitiesContainer = new();
        _defeatedEntitiesContainer.AddToClassList(_ussDefeatedEntitiesContainer);
        _showContainer.Add(_defeatedEntitiesContainer);

        VisualElement scoreContainer = new();
        scoreContainer.style.flexDirection = FlexDirection.Row;
        _defeatedEntitiesContainer.Add(scoreContainer);

        Label title = new Label("Defeated: ");
        title.style.fontSize = 32;
        scoreContainer.Add(title);

        _defeatedEntitiesContainer.style.height = Screen.height * 0.33f;

        HeroCardMini oppCard = new HeroCardMini(_selectedBattle.Opponent);
        _defeatedEntitiesContainer.Add(oppCard);
    }

    void AddPickupsContainer()
    {
        _pickupsContainer = new();
        _pickupsContainer.AddToClassList(_ussPickupsContainer);
        _showContainer.Add(_pickupsContainer);

        Label pickupCountLabel = new($"Pickups Collected: {_battleManager.CollectedPickups.Count}");
        pickupCountLabel.style.fontSize = 32;
        _pickupsContainer.Add(pickupCountLabel);
        pickupCountLabel.style.opacity = 0;
        DOTween.To(x => pickupCountLabel.style.opacity = x, 0, 1, 0.5f);

        VisualElement collectionContainer = new();
        collectionContainer.style.flexDirection = FlexDirection.Row;
        _pickupsContainer.Add(collectionContainer);

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

        _goldElement = new(_goldCollected);
        _goldElement.style.opacity = 0;
        DOTween.To(x => _goldElement.style.opacity = x, 0, 1, 0.5f);

        _spiceElement = new(_spiceCollected);
        _spiceElement.style.opacity = 0;
        DOTween.To(x => _spiceElement.style.opacity = x, 0, 1, 0.5f).SetDelay(0.5f);

        collectionContainer.Add(_goldElement);
        collectionContainer.Add(_spiceElement);

        for (int i = 0; i < _itemsCollected.Count; i++)
        {
            ItemElement itemElement = new(_itemsCollected[i]);
            itemElement.style.opacity = 0;
            DOTween.To(x => itemElement.style.opacity = x, 0, 1, 0.5f).SetDelay(1f + 0.5f * i);
            collectionContainer.Add(itemElement);
        }
    }

    void GiveCollectedPickups()
    {
        if (_pickupsGiven) return;
        _pickupsGiven = true;

        _goldElement.ChangeAmount(0);
        _spiceElement.ChangeAmount(0);

        DOTween.To(x => _pickupsContainer.style.opacity = x, 1, 0, 0.5f).SetDelay(1f);

        _gameManager.ChangeGoldValue(_goldCollected);
        _gameManager.ChangeSpiceValue(_spiceCollected);
        for (int i = 0; i < _itemsCollected.Count; i++)
            _gameManager.PlayerHero.AddItem(_itemsCollected[i]);
    }

    void ResolveKilledEntities()
    {
        if (_battleManager.KilledOpponentEntities.Count == 0)
        {
            if (_playerHero.LevelUpPointsLeft == 0) OnFinished?.Invoke();
            return;
        }

        if (_selectedBattle.BattleType == BattleType.Duel)
            DisplayKilledCreatures();
        if (_selectedBattle.BattleType == BattleType.Waves)
            DisplayKilledWaves();
    }

    void DisplayKilledCreatures()
    {
        int delay = 2000 / _battleManager.KilledOpponentEntities.Count;
        List<BattleEntity> killedEnemies = new(_battleManager.KilledOpponentEntities);

        _enemiesKilledShowSchedule = schedule.Execute(() => ShowKilledCreature()).Every(delay);
    }

    void ShowKilledCreature()
    {
        if (_enemyIndex >= _battleManager.KilledOpponentEntities.Count)
        {
            _enemiesKilledShowSchedule.Pause();
            if (_playerHero.LevelUpPointsLeft == 0) OnFinished?.Invoke();
            return;
        }

        // HERE: broken minion vs creature
        //  BattleEntity enemy = _battleManager.KilledOpponentEntities[_enemyIndex];

        // create an element with icon
        //CreatureIcon icon = new(enemy.Creature);
        // Add(icon);

        // MoveDefeatedEntity(icon);

        // TODO: is price good for score?
        // _playerHero.GetExp(enemy.Entity.Price);
        _enemyIndex++;
    }

    void DisplayKilledWaves()
    {
        List<Minion> minions = new(_gameManager.HeroDatabase.GetAllMinions());
        foreach (Minion minion in minions)
        {
            int count = _selectedBattle.GetTotalNumberOfMinionsByName(minion.Name);
            if (count == 0) continue;

            _killedMinionArmies.Add(new(minion, count));
        }

        int delay = 2000 / _killedMinionArmies.Count;
        _enemiesKilledShowSchedule = schedule.Execute(() => ShowKilledMinion()).Every(delay);
    }

    void ShowKilledMinion()
    {
        if (_enemyIndex >= _killedMinionArmies.Count)
        {
            _enemiesKilledShowSchedule.Pause();
            if (_playerHero.LevelUpPointsLeft == 0) OnFinished?.Invoke();
            return;
        }

        MoveDefeatedEntity(_killedMinionArmies[_enemyIndex]);

        // TODO: is price good for score?
        int exp = _killedMinionArmies[_enemyIndex].Minion.Price * _killedMinionArmies[_enemyIndex].Count;
        _playerHero.GetExp(exp);
        _enemyIndex++;
    }

    void MoveDefeatedEntity(VisualElement elToMove)
    {
        _audioManager.PlayUI("Show Killed Creature");

        // middle of the screen
        Vector3 start = new(0, _defeatedEntitiesContainer.resolvedStyle.height * 0.5f, 0);

        float xChange = Random.Range(50, Screen.width * 0.25f);
        if (_enemyIndex % 2 == 0)
            xChange *= -1;
        Vector3 end = new Vector3(start.x + xChange, Random.Range(20, 200), 0);

        ArcMovementElement arcMovement = new(elToMove, start, end);
        _defeatedEntitiesContainer.Add(arcMovement);
    }

    void OnHeroPointAdded()
    {
        if (_playerHero.LevelUpPointsLeft == 0)
            OnFinished?.Invoke();
    }

    public void MoveAway()
    {
        GiveCollectedPickups();

        HeroCardMini = _heroCard.HeroCardMini;
        HeroCardMini.SmallCard();
        style.position = Position.Absolute;
        HeroCardMini.style.position = Position.Absolute;
        HeroCardMini.style.left = _heroCard.worldBound.x;
        HeroCardMini.style.top = _heroCard.worldBound.y;
        parent.Add(HeroCardMini);

        // everything except hero card mini opacity > 0
        DOTween.To(x => _heroContainer.style.opacity = x, 1, 0, 0.5f);
        DOTween.To(x => _defeatedEntitiesContainer.style.opacity = x, 1, 0, 0.5f)
                .OnComplete(() => _defeatedEntitiesContainer.Clear());

        DOTween.To(x => style.opacity = x, 1, 0, 0.5f).SetDelay(0.5f);

        _audioManager.PlayUIDelayed("Paper Flying", 0.5f);

        DOTween.To(x => HeroCardMini.style.left = x, _heroCard.worldBound.x, 40, 0.5f)
            .SetDelay(0.5f)
            .SetEase(Ease.InOutFlash)
            .OnComplete(() =>
            {
                // TODO: UI scaling, park it beneath spice element
                float heroCardMiniYPercent = HeroCardMini.worldBound.y / Screen.height * 100;
                DOTween.To(x => HeroCardMini.style.top = Length.Percent(x), heroCardMiniYPercent, 25, 0.5f);
            });
    }
}
