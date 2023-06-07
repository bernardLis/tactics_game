using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;

public class BattleResult : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "battle-result__";
    const string _ussMain = _ussClassName + "main";
    const string _ussResourceContainer = _ussClassName + "resource-container";

    const string _ussContent = _ussClassName + "content";
    const string _ussContinueButton = _ussClassName + "continue-button";

    GameManager _gameManager;
    AudioManager _audioManager;
    BattleManager _battleManager;

    Battle _battle;

    VisualElement _content;
    MyButton _continueButton;

    BattleStatsContainer _statsContainer;
    RewardExpContainer _rewardExpContainer;
    RewardContainer _rewardContainer;
    BattleChoiceContainer _battleChoiceContainer;

    GameObject _starEffect;

    public event Action OnExpContainerClosed;
    public event Action OnStatsContainerClosed;
    public event Action OnRewardContainerClosed;
    public event Action OnBattleChoiceContainerClosed;

    public BattleResult(VisualElement root)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _battleManager = BattleManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleResultStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root, false);

        _battle = _battleManager.LoadedBattle;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _audioManager.PlaySFX("QuestWon", Vector3.one);

        // making sure that vfx is underneath the content but visible
        Add(_root.Q<VisualElement>("vfx"));
        _root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;
        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);

        AddResourceContainer();

        _content = new();
        Add(_content);
        _content.AddToClassList(_ussContent);

        _rewardExpContainer = new();
        _content.Add(_rewardExpContainer);

        _continueButton = new("Continue", _ussContinueButton, ShowStatsContainer);
        _rewardExpContainer.OnFinished += () => _content.Add(_continueButton);

        _starEffect = _gameManager.GetComponent<EffectManager>()
                .PlayEffectWithName("TwinklingStarEffect", Vector3.zero, Vector3.one);

        _battleManager.GetComponent<BattleInputManager>().OnContinueClicked += () =>
        {
            using (var e = new NavigationSubmitEvent() { target = _continueButton })
                _continueButton.SendEvent(e);
        };
    }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        GameObject.Destroy(_starEffect);
        // returning vfx to its original parent. TODO: this seems like a bad idea
        _root.Add(_root.Q<VisualElement>("vfx"));
    }

    void AddResourceContainer()
    {
        VisualElement resourceContainer = new();
        resourceContainer.AddToClassList(_ussResourceContainer);
        GoldElement g = new(_gameManager.Gold);
        _gameManager.OnGoldChanged += g.ChangeAmount;
        SpiceElement s = new(_gameManager.Spice);
        _gameManager.OnSpiceChanged += s.ChangeAmount;
        resourceContainer.Add(g);
        resourceContainer.Add(s);
        Add(resourceContainer);
    }

    void ShowStatsContainer()
    {
        OnExpContainerClosed?.Invoke();
        _rewardExpContainer.MoveAway();
        _content.Remove(_continueButton);

        schedule.Execute(() =>
        {
            _statsContainer = new(_content);
            _content.Add(_statsContainer);

            _continueButton = new("Continue", _ussContinueButton, ShowRewards);
            _statsContainer.OnFinished += () => _content.Add(_continueButton);

        }).StartingIn(1000);
    }

    void ShowRewards()
    {
        OnStatsContainerClosed?.Invoke();
        _statsContainer.MoveAway();
        _content.Remove(_continueButton);
        schedule.Execute(() =>
        {

            _rewardContainer = new RewardContainer();
            _content.Add(_rewardContainer);

            _continueButton = new("Continue", _ussContinueButton, ShowBattleChoices);
            _rewardContainer.OnRewardSelected += () => _content.Add(_continueButton);
        }).StartingIn(1000);

    }

    void ShowBattleChoices()
    {
        OnRewardContainerClosed?.Invoke();
        _rewardContainer.MoveAway();
        _content.Remove(_continueButton);

        schedule.Execute(() =>
        {
            _battleChoiceContainer = new();
            _content.Add(_battleChoiceContainer);

            _continueButton = new("Continue", _ussContinueButton, LoadBattle);
            _battleChoiceContainer.OnBattleSelected += () => _content.Add(_continueButton);
        }).StartingIn(1000);
    }

    void LoadBattle()
    {
        OnBattleChoiceContainerClosed?.Invoke();
        _gameManager.LoadScene(Scenes.Battle);
    }

    public void HideContent()
    {
        _content.style.display = DisplayStyle.None;
    }
}
