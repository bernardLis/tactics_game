using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public class QuestResultElement : FullScreenElement
{
    GameManager _gameManager;
    AudioManager _audioManager;

    Quest _quest;

    VisualElement _content;
    VisualElement _topContainer;
    VisualElement _middleContainer;
    VisualElement _middleLeftContainer;
    VisualElement _middleRightContainer;
    VisualElement _bottomContainer;

    List<CharacterCardExp> _characterCardsExp = new();
    RewardContainer _rewardContainer;
    AudioSource _openSfxAudioSource;

    MyButton _backButton;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "quest-result__";
    const string _ussWonMain = _ussClassName + "won-main";
    const string _ussLostMain = _ussClassName + "lost-main";
    const string _ussContent = _ussClassName + "content";
    const string _ussTopContainer = _ussClassName + "top-container";
    const string _ussMiddleContainer = _ussClassName + "middle-container";
    const string _ussMiddleLeftContainer = _ussClassName + "middle-left-container";
    const string _ussMiddleRightContainer = _ussClassName + "middle-right-container";
    const string _ussBottomContainer = _ussClassName + "bottom-container";

    const string _ussResultLabel = _ussClassName + "result-label";

    public QuestResultElement(VisualElement root, Report report)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.QuestResultStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root, false);
        _quest = report.Quest;

        AddToClassList(_ussCommonTextPrimary);
        if (_quest.IsWon)
            AddToClassList(_ussWonMain);
        else
            AddToClassList(_ussLostMain);

        _content = new();
        Add(_content);
        _content.AddToClassList(_ussContent);

        _topContainer = new();
        _middleContainer = new();
        _middleLeftContainer = new();
        _middleRightContainer = new();
        _bottomContainer = new();
        _topContainer.AddToClassList(_ussTopContainer);
        _middleContainer.AddToClassList(_ussMiddleContainer);
        _middleLeftContainer.AddToClassList(_ussMiddleLeftContainer);
        _middleRightContainer.AddToClassList(_ussMiddleRightContainer);
        _bottomContainer.AddToClassList(_ussBottomContainer);

        _content.Add(_topContainer);
        _content.Add(_middleContainer);
        _content.Add(_bottomContainer);

        _middleContainer.Add(_middleLeftContainer);
        _middleContainer.Add(_middleRightContainer);

        _topContainer.Add(GetHeader());

        _middleRightContainer.Add(GetRewardChest());
        _bottomContainer.Add(GetBackButton());

        HandleSpectacle();
    }

    async void HandleSpectacle()
    {
        if (_quest.IsWon)
            _openSfxAudioSource = _audioManager.PlaySFX("QuestWon", Vector3.one);
        else
            _openSfxAudioSource = _audioManager.PlaySFX("QuestLost", Vector3.one);

        await HandleCharacterExp();
        if (!_quest.IsWon)
            await DOTween.To(x => _rewardContainer.style.opacity = x, 1, 0, 2f).AsyncWaitForCompletion();
        await DOTween.To(x => _backButton.style.opacity = x, 0, 1, 0.5f).AsyncWaitForCompletion();
    }

    async Task HandleCharacterExp()
    {
        foreach (Character c in _quest.AssignedCharacters)
        {
            CharacterCardExp card = new(c);
            _characterCardsExp.Add(card);
            _middleLeftContainer.Add(card);
            card.style.opacity = 0;
        }

        await Task.Delay(100);
        ScaleCharacterCards();

        int expReward = _quest.CalculateAwardExp();
        await Task.Delay(1000);
        foreach (CharacterCardExp card in _characterCardsExp)
        {
            await DOTween.To(x => card.style.opacity = x, 0, 1, 0.5f).AsyncWaitForCompletion();
            card.Character.GetExp(expReward);
        }
    }

    VisualElement GetHeader()
    {
        string t = _quest.IsWon ? "Quest won!" : "Quest lost!";
        Label l = new();
        l.AddToClassList(_ussResultLabel);
        l.text = t;
        return l;
    }

    void ScaleCharacterCards()
    {
        VisualElement container = _characterCardsExp[0].parent;

        // TODO: improve this, this container thing could be a separate object that does that by itself.
        // width
        float parentWidth = container.layout.width;
        float targetChildWidth = (parentWidth - 100) / _characterCardsExp.Count;
        if (_characterCardsExp[0].layout.width < targetChildWidth)
            return;
        float targetScaleWidth = targetChildWidth / _characterCardsExp[0].layout.width;

        // height
        float parentHeight = container.layout.height;
        float targetChildHeight = (parentWidth - 100) / _characterCardsExp.Count;
        if (_characterCardsExp[0].layout.height < targetChildHeight)
            return;
        float targetScaleHeight = targetChildHeight / _characterCardsExp[0].layout.height;

        float smallerScale = targetScaleWidth < targetScaleHeight ? targetScaleHeight : targetScaleWidth;

        foreach (CharacterCardExp c in _characterCardsExp)
            c.transform.scale = new Vector3(smallerScale, smallerScale, smallerScale);
    }

    VisualElement GetRewardChest()
    {
        _rewardContainer = new(_quest.Reward, _quest.IsWon);
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
        _backButton = new("Back", _ussCommonMenuButton, Hide);
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
