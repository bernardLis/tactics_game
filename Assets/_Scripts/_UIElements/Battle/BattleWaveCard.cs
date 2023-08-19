using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleWaveCard : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-wave-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopPanel = _ussClassName + "top-panel";
    const string _ussDifficultyIcon = _ussClassName + "difficulty-icon";

    GameManager _gameManager;
    BattleManager _battleManager;

    VisualElement _topPanel;
    VisualElement _middlePanel;
    VisualElement _bottomPanel;

    BattleWave _battleWave;
    float _lastWaveSpawnTime;

    LineTimerElement _lineTimerElement;
    TimerElement _nextGroupTimer;
    public BattleWaveCard(BattleWave battleWave, float lastWaveSpawnTime)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleWaveCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleManager = BattleManager.Instance;

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _battleWave = battleWave;
        _lastWaveSpawnTime = lastWaveSpawnTime;

        _topPanel = new();
        _middlePanel = new();
        _bottomPanel = new();

        Add(_topPanel);
        Add(_middlePanel);
        Add(_bottomPanel);

        _topPanel.AddToClassList(_ussTopPanel);

        PopulateTopPanel();
        PopulateMiddlePanel();
        PopulateBottomPanel();

        _battleWave.OnGroupSpawned += OnGroupSpawned;
    }

    void OnGroupSpawned()
    {
        if (_battleWave.CurrentGroupIndex == _battleWave.OpponentGroups.Count)
        {
            _nextGroupTimer.RemoveFromHierarchy();
            _bottomPanel.Add(new Label("Last group spawned"));
        }
    }

    void PopulateTopPanel()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _topPanel.Add(container);

        Label diff = new("Difficulty:");
        container.Add(diff);

        for (int i = 0; i < _battleWave.Difficulty; i++)
        {
            Label icon = new();
            icon.AddToClassList(_ussDifficultyIcon);
            container.Add(icon);
        }
    }

    void PopulateMiddlePanel()
    {
        float totalTime = _battleWave.DelayBetweenGroups * (_battleWave.OpponentGroups.Count - 1);
        float timePassedFromStart = Time.time - _battleWave.StartTime;
        float timeLeft = totalTime - timePassedFromStart;

        _lineTimerElement = new(timeLeft, totalTime, false, "");
        _lineTimerElement.HideLabel();
        _middlePanel.Add(_lineTimerElement);

        schedule.Execute(AddGroupMarkers).StartingIn(10);
    }

    void AddGroupMarkers()
    {
        float spaceBetween = _lineTimerElement.resolvedStyle.width / _battleWave.OpponentGroups.Count;
        spaceBetween += spaceBetween / _battleWave.OpponentGroups.Count;

        for (int i = 0; i < _battleWave.OpponentGroups.Count; i++)
        {
            float leftPos = i * spaceBetween;
            OpponentGroupMarkerElement marker = new(_battleWave.OpponentGroups[i]);
            marker.style.position = Position.Absolute;
            marker.style.left = leftPos - 12;
            _lineTimerElement.Add(marker);
        }
    }

    void PopulateBottomPanel()
    {
        float timeLeft = _battleWave.DelayBetweenGroups - (Time.time - _lastWaveSpawnTime);
        _nextGroupTimer = new(timeLeft, _battleWave.DelayBetweenGroups - 1, true, "Next group in: ");// -1 looks better
        _bottomPanel.Add(_nextGroupTimer);
    }
}
