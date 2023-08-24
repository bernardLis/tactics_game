using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleFinishedScreen : FullScreenElement
{
    const string _ussClassName = "battle-finished__";
    const string _ussMain = _ussClassName + "main";

    protected VisualElement _mainContainer;

    public BattleFinishedScreen()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleFinishedStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _mainContainer = new();
        _mainContainer.AddToClassList(_ussMain);
        _content.Add(_mainContainer);

        AddTitle();
        AddTotalGold();
        AddTimeSurvived();
        AddScore();
        AddRewardIcon();
    }

    protected virtual void AddTitle()
    {
        // meant to be overwritten
    }

    void AddTotalGold()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _mainContainer.Add(container);

        Label text = new("Total gold collected: ");
        container.Add(text);

        GoldElement el = new(_gameManager.TotalGoldCollected);
        container.Add(el);

        container.style.opacity = 0;
        DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f).SetDelay(1.5f).SetUpdate(true);

    }

    void AddTimeSurvived()
    {
        int minutes = Mathf.FloorToInt(Time.time / 60F);
        int seconds = Mathf.FloorToInt(Time.time - minutes * 60);

        string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);

        Label text = new($"Time survived: {timerText}");
        _mainContainer.Add(text);

        text.style.opacity = 0;
        DOTween.To(x => text.style.opacity = x, 0, 1, 0.5f).SetDelay(2f).SetUpdate(true);
    }

    void AddScore()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _mainContainer.Add(container);

        Label text = new($"Score: ");

        int score = Mathf.FloorToInt(Time.time * _gameManager.TotalGoldCollected);
        ChangingValueElement el = new();
        el.Initialize(score, 34);

        container.Add(text);
        container.Add(el);

        container.style.opacity = 0;
        DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f).SetDelay(2.5f).SetUpdate(true);

    }

    void AddRewardIcon()
    {
        VisualElement container = new();

        RewardIcon rewardIcon = _gameManager.GameDatabase.RewardIcons[Random.Range(0, _gameManager.GameDatabase.RewardIcons.Length)];

        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(rewardIcon.Sprite);
        container.Add(icon);

        Label text = new(rewardIcon.Text);
        container.Add(text);

        container.style.opacity = 0;
        DOTween.To(x => container.style.opacity = x, 0, 1, 0.5f).SetDelay(3f).SetUpdate(true);
    }

}
