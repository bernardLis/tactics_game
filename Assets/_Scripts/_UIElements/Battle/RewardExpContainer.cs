using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;

public class RewardExpContainer : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    GameManager _gameManager;
    BattleManager _battleManager;

    Hero _playerHero;


    VisualElement _heroContainer;
    VisualElement _defeatedEntitiesContainer;
    VisualElement _opponentContainer;

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

        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.justifyContent = Justify.Center;

        AddWinner();
        AddShowContainer();
        AddLoser();

        RunShow();
    }

    void RunShow()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(DOTween.To(x => _heroContainer.style.opacity = x, 0, 1, 0.5f));
        mySequence.Append(DOTween.To(x => _opponentContainer.style.opacity = x, 0, 1, 0.5f));
        mySequence.Append(DOTween.To(x => _defeatedEntitiesContainer.style.opacity = x, 0, 1, 0.5f));
        mySequence.AppendCallback(() => ShowKilledEnemies());
    }

    void AddWinner()
    {
        _heroContainer = new();
        _heroContainer.style.flexDirection = FlexDirection.Row;
        _heroContainer.style.alignItems = Align.Center;
        _heroContainer.style.justifyContent = Justify.Center;
        _heroContainer.style.opacity = 0;

        Label title = new Label("Winner: ");
        title.style.fontSize = 64;
        title.AddToClassList(_ussCommonTextPrimary);
        _heroContainer.Add(title);

        HeroCardExp heroCard = new HeroCardExp(_playerHero);
        heroCard.OnPointAdded += OnHeroPointAdded;
        _heroContainer.Add(heroCard);

        Add(_heroContainer);
    }

    void AddShowContainer()
    {
        _defeatedEntitiesContainer = new();
        _defeatedEntitiesContainer.style.width = Length.Percent(100);
        _defeatedEntitiesContainer.style.alignItems = Align.Center;
        _defeatedEntitiesContainer.style.justifyContent = Justify.Center;
        _defeatedEntitiesContainer.style.opacity = 0;

        Label title = new Label("Defeated enemies");
        title.style.fontSize = 64;
        _defeatedEntitiesContainer.Add(title);

        _defeatedEntitiesContainer.style.height = Screen.height * 0.33f;
        Add(_defeatedEntitiesContainer);
    }

    void AddLoser()
    {
        _opponentContainer = new();
        _opponentContainer.style.flexDirection = FlexDirection.Row;
        _opponentContainer.style.alignItems = Align.Center;
        _opponentContainer.style.justifyContent = Justify.Center;
        _opponentContainer.style.opacity = 0;

        Label title = new Label("Loser: ");
        title.style.fontSize = 64;
        title.AddToClassList(_ussCommonTextPrimary);
        _opponentContainer.Add(title);

        HeroCardExp oppCard = new HeroCardExp(_gameManager.SelectedBattle.Opponent);
        _opponentContainer.Add(oppCard);

        Add(_opponentContainer);

    }

    void ShowKilledEnemies()
    {
        List<BattleEntity> killedEnemies = new(_battleManager.KilledEnemyEntities);
        _enemiesKilledShowSchedule = schedule.Execute(() => ShowKilledEnemy()).Every(500);
    }

    void ShowKilledEnemy()
    {
        if (_enemyIndex >= _battleManager.KilledEnemyEntities.Count)
        {
            _enemiesKilledShowSchedule.Pause();
            if (_playerHero.LevelUpPointsLeft == 0) OnFinished?.Invoke();
            return;
        }

        BattleEntity enemy = _battleManager.KilledEnemyEntities[_enemyIndex];
        // create an element with enemy icon
        VisualElement icon = new();
        icon.style.width = 50;
        icon.style.height = 50;
        icon.style.backgroundImage = new StyleBackground(enemy.Stats.Icon);

        // middle of the screen
        Vector3 start = new(Screen.width * 0.5f,
                 _defeatedEntitiesContainer.resolvedStyle.bottom, 0);
        float xChange = Random.Range(50, Screen.width * 0.25f);
        if (_enemyIndex % 2 == 0)
            xChange *= -1;
        Vector3 end = new Vector3(start.x + xChange,
                _defeatedEntitiesContainer.resolvedStyle.bottom - Random.Range(20, 200), 0);

        Debug.Log($"Screen.width / 2: {Screen.width * 0.5f}");
        Debug.Log($"_defeatedEntitiesContainer.resolvedStyle.width {_defeatedEntitiesContainer.resolvedStyle.width}");
        Debug.Log($"_defeatedEntitiesContainer.resolvedStyle.bottom {_defeatedEntitiesContainer.resolvedStyle.bottom}");
        Debug.Log($"_showContainer.layout.yMax {_defeatedEntitiesContainer.layout.yMax}");
        Debug.Log($"start {start}");
        Debug.Log($"end {end}");

        ArcMovementElement arcMovement = new(icon, start, end);
        _defeatedEntitiesContainer.Add(arcMovement);

        _playerHero.GetExp(enemy.Stats.Price);
        _enemyIndex++;
    }

    void OnHeroPointAdded()
    {
        if (_playerHero.LevelUpPointsLeft == 0)
            OnFinished?.Invoke();
    }
}
