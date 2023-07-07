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

    VisualElement _vfx;

    VisualElement _content;

    public GoldElement GoldElement;
    public SpiceElement SpiceElement;

    MyButton _continueButton;

    BattleResultHeroElement _resultHeroElement;
    BattleResultArmyElement _resultArmyElement;
    BattleResultRewardElement _resultRewardElement;
    BattleResultChoiceElement _resultChoiceElement;

    GameObject _starEffect;
    VisualElement _transitionOverlay;

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

        _vfx = _root.Q<VisualElement>("vfx");
        // making sure that vfx is underneath the content but visible
        Add(_vfx);
        _vfx.pickingMode = PickingMode.Ignore;
        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);

        AddResourceContainer();

        _content = new();
        Add(_content);
        _content.AddToClassList(_ussContent);

        _resultHeroElement = new();
        _content.Add(_resultHeroElement);

        _continueButton = new("Continue", _ussContinueButton, ShowArmyContainer);
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

    void ShowArmyContainer()
    {
        OnHeroElementClosed?.Invoke();
        _resultHeroElement.MoveAway();
        _gameManager.PlayerHero.OnItemAdded +=
                (item) => Helpers.DisplayTextOnElement(_root, _resultHeroElement.HeroCardMini,
                       "+ " + item.ItemName, Helpers.GetColor(item.Rarity.ToString()));
        _gameManager.PlayerHero.OnCreatureAdded +=
                (creature) => Helpers.DisplayTextOnElement(_root, _resultHeroElement.HeroCardMini,
                       "+ " + creature.Name, creature.Element.Color);

        _content.Remove(_continueButton);

        schedule.Execute(() =>
        {
            _resultArmyElement = new(_content);
            _content.Add(_resultArmyElement);

            _continueButton = new("Continue", _ussContinueButton, ShowRewardContainer);
            _resultArmyElement.OnFinished += () => _content.Add(_continueButton);

            _content.Remove(_resultHeroElement);
        }).StartingIn(1000);
    }

    void ShowRewardContainer()
    {
        OnArmyElementClosed?.Invoke();
        _resultArmyElement.MoveAway();

        _content.Remove(_continueButton);
        schedule.Execute(() =>
        {
            _resultRewardElement = new BattleResultRewardElement();
            _content.Add(_resultRewardElement);

            _continueButton = new("Continue", _ussContinueButton, ShowBattleChoiceContainer);
            _resultRewardElement.OnRewardSelected += () => _content.Add(_continueButton);

            _content.Remove(_resultArmyElement);
        }).StartingIn(1000);
    }

    void ShowBattleChoiceContainer()
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

            _content.Remove(_resultRewardElement);
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

    public override void Hide()
    {
        _transitionOverlay = new();
        _transitionOverlay.style.position = Position.Absolute;
        _transitionOverlay.style.width = Length.Percent(100);
        _transitionOverlay.style.height = Length.Percent(100);
        _transitionOverlay.style.backgroundColor = new Color(1, 1, 1, 0);

        Add(_transitionOverlay);
        DOTween.To(x => _transitionOverlay.style.opacity = x, 0, 1, 0.5f)
            .SetDelay(0.5f)
            .OnComplete(() =>
            {
                _starEffect.SetActive(false);
                style.display = DisplayStyle.None;
            });
    }

    public void Show()
    {
        _starEffect.SetActive(true);
        style.display = DisplayStyle.Flex;
        _resultArmyElement.RefreshArmy();

        DOTween.To(x => _transitionOverlay.style.opacity = x, 1, 0, 0.5f)
            .OnComplete(() =>
            {
                _transitionOverlay.RemoveFromHierarchy();
            });
    }

}
