using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;

public class BattleResultElement : FullScreenElement
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
    CutsceneManager _cutsceneManager;
    BattleManager _battleManager;

    Battle _battle;

    VisualElement _content;

    public GoldElement GoldElement;
    public SpiceElement SpiceElement;

    MyButton _continueButton;

    BattleResultHeroElement _resultHeroElement; 
    BattleResultArmyElement _resultArmyElement; 
    BattleResultRewardElement _resultRewardElement; 
    BattleResultChoiceElement _resultChoiceElement; 

    GameObject _starEffect;

    public event Action OnHeroElementClosed;
    public event Action OnArmyElementClosed;
    public event Action OnRewardElementClosed;
    public event Action OnChoiceElementClosed;

    public BattleResultElement(VisualElement root)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _cutsceneManager = _gameManager.GetComponent<CutsceneManager>();
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

        _audioManager.PlayUI("Reward Chosen");

        // making sure that vfx is underneath the content but visible
        Add(_root.Q<VisualElement>("vfx"));
        _root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;
        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);

        AddResourceContainer();

        _content = new();
        Add(_content);
        _content.AddToClassList(_ussContent);

        _resultHeroElement = new();
        _content.Add(_resultHeroElement);

        _continueButton = new("Continue", _ussContinueButton, ShowStatsContainer);
        _resultHeroElement.OnFinished += () => _content.Add(_continueButton);

        _starEffect = _gameManager.GetComponent<EffectManager>()
                .PlayEffectWithName("TwinklingStarEffect", Vector3.zero, Vector3.one);

        _battleManager.GetComponent<BattleInputManager>().OnContinueClicked += () =>
        {
            if (_continueButton == null) return;
            if (!_continueButton.enabledInHierarchy) return;
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
        Add(resourceContainer);

        GoldElement = new(_gameManager.Gold);
        _gameManager.OnGoldChanged += OnGoldChanged;
        resourceContainer.Add(GoldElement);

        SpiceElement = new(_gameManager.Spice);
        _gameManager.OnSpiceChanged += OnSpiceChanged;
        resourceContainer.Add(SpiceElement);
    }

    void OnGoldChanged(int newValue)
    {
        int change = newValue - GoldElement.Amount;
        Helpers.DisplayTextOnElement(_root, GoldElement, "" + change, Color.yellow);
        GoldElement.ChangeAmount(newValue);
    }

    void OnSpiceChanged(int newValue)
    {
        int change = newValue - SpiceElement.Amount;
        Helpers.DisplayTextOnElement(_root, SpiceElement, "" + change, Color.red);
        SpiceElement.ChangeAmount(newValue);
    }

    void ShowStatsContainer()
    {
        OnHeroElementClosed?.Invoke();
        _resultHeroElement.MoveAway();
        _gameManager.PlayerHero.OnItemAdded +=
                (item) => Helpers.DisplayTextOnElement(_root, _resultHeroElement.HeroCardMini,
                       "+ " + item.ItemName, Helpers.GetColor(item.Rarity.ToString()));

        _content.Remove(_continueButton);

        schedule.Execute(() =>
        {
            _resultArmyElement = new(_content);
            _content.Add(_resultArmyElement);

            _continueButton = new("Continue", _ussContinueButton, ShowRewards);
            _resultArmyElement.OnFinished += () => _content.Add(_continueButton);
        }).StartingIn(1000);
    }

    void ShowRewards()
    {
        OnArmyElementClosed?.Invoke();
        _resultArmyElement.MoveAway();

        _content.Remove(_continueButton);
        schedule.Execute(() =>
        {
            _resultRewardElement = new BattleResultRewardElement();
            _content.Add(_resultRewardElement);

            _continueButton = new("Continue", _ussContinueButton, ShowBattleChoices);
            _resultRewardElement.OnRewardSelected += () => _content.Add(_continueButton);
        }).StartingIn(1000);
    }

    void ShowBattleChoices()
    {
        OnRewardElementClosed?.Invoke();
        _resultRewardElement.MoveAway();
        _content.Remove(_continueButton);

        if (_gameManager.BattleNumber == 2)
        {
            PlayRivalCutscene();
            return;
        }

        schedule.Execute(() =>
        {
            _resultChoiceElement = new();
            _content.Add(_resultChoiceElement);

            _continueButton = new("Continue", _ussContinueButton, LoadBattle);
            _resultChoiceElement.OnBattleSelected += () => _content.Add(_continueButton);
        }).StartingIn(1000);
    }

    void PlayRivalCutscene()
    {
        _content.style.display = DisplayStyle.None;
        _cutsceneManager.Initialize(_root);
        _cutsceneManager.PlayCutscene("Rival Intro");

        Battle b = ScriptableObject.CreateInstance<Battle>();
        b.Opponent = _gameManager.RivalHero;
        _gameManager.SelectedBattle = b;

        _cutsceneManager.OnCutsceneFinished += (c) => LoadBattle();
    }

    void LoadBattle()
    {
        OnChoiceElementClosed?.Invoke();
        _gameManager.LoadScene(Scenes.Battle);
    }

    public void HideContent()
    {
        _content.style.display = DisplayStyle.None;
    }
}
