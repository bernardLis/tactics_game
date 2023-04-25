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
    const string _ussWonMain = _ussClassName + "won-main";
    const string _ussLostMain = _ussClassName + "lost-main";
    const string _ussContent = _ussClassName + "content";

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

    public BattleResult(VisualElement root)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _battleManager = BattleManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        // TODO: different styles won/lost
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleResultStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root, false);

        _battle = _battleManager.LoadedBattle;

        AddToClassList(_ussCommonTextPrimary);

        if (_battle.Won)
        {
            AddToClassList(_ussWonMain);
            _audioManager.PlaySFX("QuestWon", Vector3.one);
        }
        else
        {
            _audioManager.PlaySFX("QuestLost", Vector3.one);
            AddToClassList(_ussLostMain);
        }

        // making sure that vfx is underneath the content but visible
        Add(_root.Q<VisualElement>("vfx"));
        _root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;
        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);

        _content = new();
        Add(_content);
        _content.AddToClassList(_ussContent);

        _statsContainer = new();
        _content.Add(_statsContainer);

        _continueButton = new("Continue", _ussCommonMenuButton, ShowRewardExp);
        _statsContainer.OnFinished += () => _content.Add(_continueButton);

        _starEffect = _gameManager.GetComponent<EffectManager>()
                .PlayEffectWithName("TwinklingStarEffect", Vector3.zero, Vector3.one);
    }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        GameObject.Destroy(_starEffect);
        // returning vfx to its original parent. TODO: this seems like a bad idea
        _root.Add(_root.Q<VisualElement>("vfx"));
    }

    void ShowRewardExp()
    {
        _content.Clear();

        _rewardExpContainer = new();
        _content.Add(_rewardExpContainer);

        _continueButton = new("Continue", _ussCommonMenuButton, ShowRewards);
        _rewardExpContainer.OnFinished += () => _content.Add(_continueButton);
    }

    void ShowRewards()
    {
        _content.Clear();
        _content.Add(new HeroCardMini(_gameManager.PlayerHero));

        _rewardContainer = new RewardContainer();
        _content.Add(_rewardContainer);

        _continueButton = new("Continue", _ussCommonMenuButton, ShowBattleChoices);
        _rewardContainer.OnRewardSelected += () => _content.Add(_continueButton);
    }

    void ShowBattleChoices()
    {
        _content.Clear();
        _battleChoiceContainer = new();
        _content.Add(_battleChoiceContainer);

        _continueButton = new("Continue", _ussCommonMenuButton, LoadBattle);
        _battleChoiceContainer.OnBattleSelected += () => _content.Add(_continueButton);
    }

    void LoadBattle() { _gameManager.LoadLevel(Scenes.Battle); }
}
