using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public class QuestResultsVisualElement : FullScreenVisual
{
    GameManager _gameManager;
    AudioManager _audioManager;

    Quest _quest;

    VisualElement _content;
    VisualElement _troopsContainer;
    List<CharacterCard> _characterCards = new();
    RewardContainer _rewardContainer;
    AudioSource _openSfxAudioSource;

    MyButton _backButton;

    public QuestResultsVisualElement(VisualElement root, Report report)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;

        Initialize(root, false);
        _quest = report.Quest;

        AddToClassList("textPrimary");
        if (_quest.IsWon)
            AddToClassList("questResultContainerWon");
        else
            AddToClassList("questResultContainerLost");

        _content = new();
        Add(_content);
        _content.AddToClassList("questResultContent");

        VisualElement topContainer = new();
        _content.Add(topContainer);
        topContainer.Add(GetHeader());
        topContainer.Add(GetSuccessLabel());

        VisualElement midContainer = new();
        midContainer.style.flexDirection = FlexDirection.Row;
        midContainer.style.width = Length.Percent(100);
        _content.Add(midContainer);
        midContainer.Add(GetTroopsContainer());
        if (_quest.IsWon)
            midContainer.Add(GetRewardChest());

        VisualElement bottomContainer = new();
        _content.Add(bottomContainer);
        bottomContainer.Add(GetBackButton());

        HandleSpectacle();
    }

    async void HandleSpectacle()
    {
        if (_quest.IsWon)
            _openSfxAudioSource = _audioManager.PlaySFX("QuestWon", Vector3.one);
        else
            _openSfxAudioSource = _audioManager.PlaySFX("QuestLost", Vector3.one);

        await HandleCharacterExp();
        await DOTween.To(x => _rewardContainer.style.opacity = x, 0, 1, 0.5f).AsyncWaitForCompletion();
        await DOTween.To(x => _backButton.style.opacity = x, 0, 1, 0.5f).AsyncWaitForCompletion();
    }

    async Task HandleCharacterExp()
    {
        foreach (Character c in _quest.AssignedCharacters)
        {
            CharacterCard card = new(c, true, false, false);
            _characterCards.Add(card);
            card.style.opacity = 0;
            _troopsContainer.Add(card);
        }

        await Task.Delay(100);
        ScaleCharacterCards();

        int expReward = _quest.CalculateAwardExp();
        await Task.Delay(1000);
        foreach (CharacterCard card in _characterCards)
        {
            await DOTween.To(x => card.style.opacity = x, 0, 1, 0.5f).AsyncWaitForCompletion();
            card.Character.GetExp(expReward);
        }

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
        _troopsContainer = new();
        _troopsContainer.style.flexDirection = FlexDirection.Row;
        _troopsContainer.style.width = Length.Percent(50);
        return _troopsContainer;
    }

    void ScaleCharacterCards()
    {
        VisualElement container = _characterCards[0].parent;
        container.style.flexDirection = FlexDirection.Column;

        // TODO: improve this, this container thing could be a separate object that does that by itself.
        // width
        float parentWidth = container.layout.width;
        float targetChildWidth = (parentWidth - 100) / _characterCards.Count;
        if (_characterCards[0].layout.width < targetChildWidth)
            return;
        float targetScaleWidth = targetChildWidth / _characterCards[0].layout.width;

        // height
        float parentHeight = container.layout.height;
        float targetChildHeight = (parentWidth - 100) / _characterCards.Count;
        if (_characterCards[0].layout.height < targetChildHeight)
            return;
        float targetScaleHeight = targetChildHeight / _characterCards[0].layout.height;

        float smallerScale = targetScaleWidth < targetScaleHeight ? targetScaleHeight : targetScaleWidth;

        foreach (CharacterCard c in _characterCards)
            c.transform.scale = new Vector3(smallerScale, smallerScale, smallerScale);
    }

    VisualElement GetRewardChest()
    {
        _rewardContainer = new(_quest.Reward);
        _rewardContainer.style.opacity = 0;
        _rewardContainer.style.width = Length.Percent(50);
        _rewardContainer.style.alignItems = Align.Center;
        _rewardContainer.OnChestOpen += OnChestOpen;
        return _rewardContainer;
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
        _backButton.style.opacity = 0;

        if (_quest.IsWon)
        {
            _backButton.UpdateButtonText("Open the chest!");
            _backButton.SetEnabled(false);
        }

        return _backButton;
    }

    public override void Hide()
    {
        base.Hide();
        _openSfxAudioSource.Stop();
    }

}
