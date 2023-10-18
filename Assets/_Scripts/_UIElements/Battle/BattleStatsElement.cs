using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleStatsElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-stats__";
    const string _ussMain = _ussClassName + "main";
    const string _ussPanel = _ussClassName + "panel";


    const string _ussCreatureContainer = _ussClassName + "creature-container";
    const string _ussCreatureLabel = _ussClassName + "creature-label";
    const string _ussCreatureIcon = _ussClassName + "creature-icon";

    GameManager _gameManager;
    BattleManager _battleManager;

    VisualElement _leftPanel;
    VisualElement _middlePanel;
    VisualElement _rightPanel;

    public BattleStatsElement()
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleStatsStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleManager = BattleManager.Instance;
        if (_battleManager == null)
        {
            Debug.LogError("BattleManager is null");
            return;
        }

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _leftPanel = new VisualElement();
        _middlePanel = new VisualElement();
        _rightPanel = new VisualElement();
        _leftPanel.AddToClassList(_ussPanel);
        _middlePanel.AddToClassList(_ussPanel);
        _rightPanel.AddToClassList(_ussPanel);
        Add(_leftPanel);
        Add(_middlePanel);
        Add(_rightPanel);

        PopulateLeftPanel();
        PopulateMiddlePanel();
        PopulateRightPanel();
    }

    void PopulateLeftPanel()
    {
    }

    void PopulateMiddlePanel()
    {
        AddTotalGold();
        AddTimeSurvived();
        AddWavesSurvived();
    }

    void PopulateRightPanel()
    {
        AddMinionsKilled();
        AddCreatureKills();
    }

    void AddTotalGold()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;

        Label text = new("Total gold collected: ");
        container.Add(text);

        GoldElement el = new(_gameManager.TotalGoldCollected);
        container.Add(el);
        _middlePanel.Add(container);
    }

    void AddTimeSurvived()
    {
        VisualElement container = new();
        _middlePanel.Add(container);

        int minutes = Mathf.FloorToInt(_battleManager.GetTime() / 60f);
        int seconds = Mathf.FloorToInt(_battleManager.GetTime() - minutes * 60);

        string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);

        Label text = new($"Time survived: {timerText}");
        container.Add(text);
    }

    void AddWavesSurvived()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _middlePanel.Add(container);

        Label text = new($"Waves survived: ");
        container.Add(text);

        int fightIndex = _battleManager.GetComponent<BattleFightManager>().CurrentDifficulty - 1;
        ChangingValueElement waveCount = new();
        waveCount.Initialize(fightIndex, 18);
        container.Add(waveCount);
    }

    void AddMinionsKilled()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _rightPanel.Add(container);


        Label text = new($"Minions defeated: ");
        container.Add(text);

        int minionKillCount = 0;
        foreach (BattleEntity be in _battleManager.KilledOpponentEntities)
            if (be is BattleMinion)
                minionKillCount++;

        ChangingValueElement minionCount = new();
        minionCount.Initialize(minionKillCount, 18);
        container.Add(minionCount);
    }

    void AddCreatureKills()
    {
        VisualElement container = new();
        container.AddToClassList(_ussCreatureContainer);
        _rightPanel.Add(container);

        Label text = new("Creatures defeated:");
        text.AddToClassList(_ussCreatureLabel);
        container.Add(text);

        VisualElement iconContainer = new();
        iconContainer.style.flexWrap = Wrap.Wrap;
        iconContainer.style.flexDirection = FlexDirection.Row;
        container.Add(iconContainer);

        foreach (BattleEntity be in _battleManager.KilledOpponentEntities)
        {
            if (be is not BattleCreature) continue;

            VisualElement icon = new();
            icon.AddToClassList(_ussCreatureIcon);
            icon.style.backgroundImage = new StyleBackground(be.Entity.Icon);
            iconContainer.Add(icon);
        }
    }
}
