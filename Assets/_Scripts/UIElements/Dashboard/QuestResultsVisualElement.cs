using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestResultsVisualElement : FullScreenVisual
{
    GameManager _gameManager;
    Report _report;
    Quest _quest;

    VisualElement _content;
    MyButton _backButton;

    public QuestResultsVisualElement(VisualElement root, Report report)
    {
        Initialize(root, false);
        _quest = report.Quest;

        _gameManager = GameManager.Instance;
        AddToClassList("textPrimary");
        AddToClassList("questResultContainer");

        _content = new();
        Add(_content);
        _content.AddToClassList("questResultContent");

        _content.Add(GetHeader());
        _content.Add(GetSuccessLabel());
        _content.Add(GetTroopsContainer());
        _quest.AwardExp();

        // HERE: 
        // if quest won: on open you should see the exp bar increase - animation with happy sound
        // if level up, even happier sound.
        // btw. characters should not get exp in the battle.
        // if quest lost: you should see characters getting disabled - animation with sad sound
        if (_quest.IsWon)
            _content.Add(GetRewardChest());
        _content.Add(GetBackButton());
    }

    VisualElement GetHeader()
    {
        string t = _quest.IsWon ? "Quest won!" : "Quest lost!";
        Label l = new();
        l.text = t;
        l.style.fontSize = 48;
        return l;
    }

    VisualElement GetSuccessLabel()
    {
        Label success = new();
        success.text = $"Quest roll: {_quest.Roll} Quest success chance: {_quest.GetSuccessChance() * 0.01}, roll needs to be less then success chance to win.";
        return success;
    }

    VisualElement GetTroopsContainer()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;

        foreach (Character c in _quest.AssignedCharacters)
        {
            CharacterCardExtended card = new(c);
            container.Add(card);
        }

        return container;
    }

    VisualElement GetRewardChest()
    {
        RewardContainer rewardContainer = new(_quest.Reward);
        rewardContainer.OnChestOpen += OnChestOpen;
        return rewardContainer;
    }

    void OnChestOpen()
    {
        _quest.Reward.GetReward();

        _backButton.UpdateButtonText("Back");
        _backButton.SetEnabled(true);
        EnableNavigation();
    }

    VisualElement GetBackButton()
    {
        _backButton = new("Back", "menuButton", Hide);
        if (_quest.IsWon)
        {
            _backButton.UpdateButtonText("Open the chest!");
            _backButton.SetEnabled(false);
        }

        return _backButton;
    }
}
