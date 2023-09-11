using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class OpponentPortalCard : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "opponent-portal-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopPanel = _ussClassName + "top-panel";
    const string _ussDifficultyIcon = _ussClassName + "difficulty-icon";


    GameManager _gameManager;
    BattleManager _battleManager;

    VisualElement _topPanel;
    VisualElement _middlePanel;
    VisualElement _bottomPanel;

    LineTimerElement _lineTimerElement;
    TimerElement _nextGroupTimer;

    Element _element;
    BattleWave _currentWave;
    public OpponentPortalCard(Element element, BattleWave currentWave)
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.OpponentPortalCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _element = element;
        _currentWave = currentWave;

        Label header = new("Portal of " + _element.ElementName);
        Add(header);

        _topPanel = new();
        _middlePanel = new();
        _bottomPanel = new();

        Add(_topPanel);
        Add(_middlePanel);
        Add(_bottomPanel);

        _topPanel.AddToClassList(_ussTopPanel);

        PopulateCard();
    }

    void PopulateCard()
    {
        PopulateTopPanel();
        PopulateMiddlePanel();
        PopulateBottomPanel();
    }

    void PopulateTopPanel()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _topPanel.Add(container);

        Label diff = new("Difficulty:");
        container.Add(diff);

        for (int i = 0; i < _currentWave.Difficulty; i++)
        {
            Label icon = new();
            icon.AddToClassList(_ussDifficultyIcon);
            container.Add(icon);
        }
    }

    void PopulateMiddlePanel()
    {
        if (!_currentWave.IsStarted)
        {
            AddNextWaveTimer();
            return;
        }

        AddWaveLineTimer();
    }

    void PopulateBottomPanel()
    {
        if (!_currentWave.IsStarted) return;

        AddTimeToNextGroup();
    }

    void AddNextWaveTimer()
    {
        float timeUntilWave = _currentWave.StartTime - _battleManager.GetTime();
        TimerElement nextWaveTimer = new(timeUntilWave, timeUntilWave, false, "Next wave in: ");
        Add(nextWaveTimer);
    }


    void AddWaveLineTimer()
    {
        float totalTime = _currentWave.DelayBetweenGroups * (_currentWave.OpponentGroups.Count - 1);
        float timePassedFromStart = _battleManager.GetTime() - _currentWave.StartTime;
        float timeLeft = totalTime - timePassedFromStart;

        _lineTimerElement = new(timeLeft, totalTime, false, "");
        _lineTimerElement.HideLabel();
        _middlePanel.Add(_lineTimerElement);

        schedule.Execute(AddGroupMarkers).StartingIn(10);
    }

    void AddGroupMarkers()
    {
        float spaceBetween = _lineTimerElement.resolvedStyle.width / _currentWave.OpponentGroups.Count;
        spaceBetween += spaceBetween / _currentWave.OpponentGroups.Count;

        for (int i = 0; i < _currentWave.OpponentGroups.Count; i++)
        {
            float leftPos = i * spaceBetween;
            OpponentGroupMarkerElement marker = new(_currentWave.OpponentGroups[i]);
            marker.style.position = Position.Absolute;
            marker.style.left = leftPos - 12;
            _lineTimerElement.Add(marker);
        }
    }

    void AddTimeToNextGroup()
    {
        float timeToNextGroup = _currentWave.DelayBetweenGroups
                        - (_battleManager.GetTime() - _currentWave.LastGroupSpawnTime);
        _nextGroupTimer = new(timeToNextGroup, _currentWave.DelayBetweenGroups - 1, true, "Next group in: ");// -1 looks better
        _bottomPanel.Add(_nextGroupTimer);

        _currentWave.OnGroupSpawned += OnGroupSpawned;
    }

    void OnGroupSpawned()
    {
        if (_currentWave.CurrentGroupIndex == _currentWave.OpponentGroups.Count)
        {
            PopulateCard();
            // _nextGroupTimer.RemoveFromHierarchy();
            // _bottomPanel.Add(new Label("Last group spawned"));
        }
    }
}
