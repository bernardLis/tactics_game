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

    Report _report;
    Quest _quest;

    VisualElement _content;
    VisualElement _troopsContainer;
    List<CharacterCardExtended> _characterCards = new();
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
        AddToClassList("questResultContainer");

        _content = new();
        Add(_content);
        _content.AddToClassList("questResultContent");

        _content.Add(GetHeader());
        _content.Add(GetSuccessLabel());
        _content.Add(GetTroopsContainer());

        if (_quest.IsWon)
            _content.Add(GetRewardChest());
        _content.Add(GetBackButton());

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
            CharacterCardExtended card = new(c);
            _characterCards.Add(card);
            card.style.opacity = 0;
            _troopsContainer.Add(card);
        }
        await Task.Delay(100);
        ScaleCharacterCards();

        int expReward = _quest.CalculateAwardExp();
        await Task.Delay(1000);
        foreach (CharacterCardExtended card in _characterCards)
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
        _troopsContainer.style.width = Length.Percent(100);
        return _troopsContainer;
    }

    void ScaleCharacterCards()
    {
        VisualElement container = _characterCards[0].parent;
        float parentWidth = container.layout.width;
        float targetChildWidth = (parentWidth - 100) / _characterCards.Count;
        if (_characterCards[0].layout.width < targetChildWidth)
            return;

        float targetScale = targetChildWidth / _characterCards[0].layout.width;
        foreach (CharacterCardExtended c in _characterCards)
            c.transform.scale = new Vector3(targetScale, targetScale, targetScale);
    }

    VisualElement GetRewardChest()
    {
        _rewardContainer = new(_quest.Reward);
        _rewardContainer.style.opacity = 0;
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
