using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardExpContainer : VisualElement
{
    const string _ussCommonMenuButton = "common__menu-button";

    GameManager _gameManager;
    BattleManager _battleManager;

    Hero _playerHero;

    VisualElement _showContainer;
    IVisualElementScheduledItem _enemiesKilledShowSchedule;
    int _enemyIndex;


    public event Action OnFinished;
    public RewardExpContainer()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);

        _playerHero = _gameManager.PlayerHero;

        Label title = new Label("Winner!");
        HeroCardExp card = new HeroCardExp(_playerHero);
        card.OnPointAdded += OnHeroPointAdded;
        Add(card);

        _showContainer = new();
        _showContainer.style.width = Length.Percent(100);
        _showContainer.style.height = Length.Percent(70);
        Add(_showContainer);

        ShowKilledEnemies();

        // after show is over, and points are added we can move to next screen

        // TODO: normally, if the hero is not leveled up, we should wait a bit and show the rewards
    }

    void ShowKilledEnemies()
    {
        List<BattleEntity> killedEnemies = new(_battleManager.KilledEnemyEntities);
        _enemiesKilledShowSchedule = schedule.Execute(() => ShowKilledEnemy()).Every(100);
    }

    void ShowKilledEnemy()
    {
        if (_enemyIndex >= _battleManager.KilledEnemyEntities.Count)
        {
            _enemiesKilledShowSchedule.Pause();
            ShowDefeatedHero();
            return;
        }

        BattleEntity enemy = _battleManager.KilledEnemyEntities[_enemyIndex];
        // create an element with enemy icon
        VisualElement icon = new();
        icon.style.width = 50;
        icon.style.height = 50;
        icon.style.backgroundImage = new StyleBackground(enemy.Stats.Icon);




        // add it to arc movement element and move it in the show container
        // add some exp to player hero
        // _showContainer
        // display floating number on exp bar
        //_playerHero.GetExp(100);


        _enemyIndex++;

    }

    void ShowDefeatedHero()
    {


        if (_playerHero.LevelUpPointsLeft == 0)
            OnFinished?.Invoke();
    }

    void OnHeroPointAdded()
    {
        if (_playerHero.LevelUpPointsLeft == 0)
            OnFinished?.Invoke();
    }
}
