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

    const string _ussClassName = "reward-exp-container__";
    const string _ussMain = _ussClassName + "main";
    const string _ussCardContainer = _ussClassName + "card-container";
    const string _ussDefeatedEntitiesContainer = _ussClassName + "defeated-entities-container";
    const string _ussScoreContainer = _ussClassName + "score-container";

    GameManager _gameManager;
    BattleManager _battleManager;

    Hero _playerHero;

    VisualElement _heroContainer;
    HeroCardExp _heroCard;
    VisualElement _defeatedEntitiesContainer;
    ChangingValueElement _scoreCounter;
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
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardExpStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _playerHero = _gameManager.PlayerHero;

        AddToClassList(_ussMain);

        AddWinner();
        AddShowContainer();
        AddLoser();

        RunShow();
    }

    void RunShow()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Pause();
        mySequence.Append(DOTween.To(x => _heroContainer.style.opacity = x, 0, 1, 0.5f));
        mySequence.Append(DOTween.To(x => _opponentContainer.style.opacity = x, 0, 1, 0.5f));
        mySequence.Append(DOTween.To(x => _defeatedEntitiesContainer.style.opacity = x, 0, 1, 0.5f));
        mySequence.AppendCallback(() => ShowKilledEnemies());
        mySequence.Play();
    }

    void AddWinner()
    {
        _heroContainer = new();
        _heroContainer.AddToClassList(_ussCardContainer);

        Label title = new Label("Winner: ");
        title.style.fontSize = 64;
        title.AddToClassList(_ussCommonTextPrimary);
        _heroContainer.Add(title);

        _heroCard = new HeroCardExp(_playerHero);
        _heroCard.OnPointAdded += OnHeroPointAdded;
        _heroContainer.Add(_heroCard);

        Add(_heroContainer);
    }

    void AddShowContainer()
    {
        _defeatedEntitiesContainer = new();
        _defeatedEntitiesContainer.AddToClassList(_ussDefeatedEntitiesContainer);

        VisualElement scoreContainer = new();
        _defeatedEntitiesContainer.Add(scoreContainer);

        Label title = new Label("Score: ");
        title.style.fontSize = 32;
        scoreContainer.Add(title);

        _scoreCounter = new();
        _scoreCounter.Initialize(0, 64);
        scoreContainer.Add(_scoreCounter);

        _defeatedEntitiesContainer.style.height = Screen.height * 0.33f;
        Add(_defeatedEntitiesContainer);
    }

    void AddLoser()
    {
        _opponentContainer = new();
        _opponentContainer.AddToClassList(_ussCardContainer);

        Label title = new Label("Loser: ");
        title.style.fontSize = 64;
        title.AddToClassList(_ussCommonTextPrimary);
        _opponentContainer.Add(title);

        HeroCardStats oppCard = new HeroCardStats(_gameManager.SelectedBattle.Opponent);
        _opponentContainer.Add(oppCard);

        Add(_opponentContainer);
    }

    void ShowKilledEnemies()
    {
        int delay = 2000 / _battleManager.KilledOpponentEntities.Count;
        List<BattleEntity> killedEnemies = new(_battleManager.KilledOpponentEntities);

        _enemiesKilledShowSchedule = schedule.Execute(() => ShowKilledEnemy()).Every(delay);
    }

    void ShowKilledEnemy()
    {
        if (_enemyIndex >= _battleManager.KilledOpponentEntities.Count)
        {
            _enemiesKilledShowSchedule.Pause();
            if (_playerHero.LevelUpPointsLeft == 0) OnFinished?.Invoke();
            return;
        }

        BattleEntity enemy = _battleManager.KilledOpponentEntities[_enemyIndex];
        // create an element with enemy icon
        // HERE: animation of the icon

        VisualElement icon = new();
        icon.style.width = 100;
        icon.style.height = 100;

        if (enemy.Stats.IconAnimation.Length > 0)
        {
            icon.Add(new EntityIcon(enemy.Stats));
        }
        else
        {
            icon.style.backgroundImage = new StyleBackground(enemy.Stats.Icon);
        }

        // middle of the screen
        Vector3 start = new(0, _defeatedEntitiesContainer.resolvedStyle.height * 0.5f, 0);

        float xChange = Random.Range(50, Screen.width * 0.25f);
        if (_enemyIndex % 2 == 0)
            xChange *= -1;
        Vector3 end = new Vector3(start.x + xChange, Random.Range(20, 200), 0);

        ArcMovementElement arcMovement = new(icon, start, end);
        _defeatedEntitiesContainer.Add(arcMovement);

        // TODO: is price good for score?
        _playerHero.GetExp(enemy.Stats.Price);
        _scoreCounter.ChangeAmount(_scoreCounter.Amount + enemy.Stats.Price);
        _enemyIndex++;
    }

    void OnHeroPointAdded()
    {
        if (_playerHero.LevelUpPointsLeft == 0)
            OnFinished?.Invoke();
    }
}
