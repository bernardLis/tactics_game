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
    // 2. TODO:  player level up container

    RewardCardsContainer _rewardContainer;

    // 4. choose next battle container 


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

        _content = new();
        Add(_content);
        _content.AddToClassList(_ussContent);

        _statsContainer = new();
        _content.Add(_statsContainer);
        _statsContainer.OnContinue += ShowRewardExp;
    }

    void ShowRewardExp()
    {
        _content.Clear();

        _rewardExpContainer = new();
        _content.Add(_rewardExpContainer);
        _rewardExpContainer.OnContinue += ShowRewards;
    }

    void ShowRewards()
    {
        _content.Clear();
        _content.Add(new HeroCardMini(_gameManager.PlayerHero));

        _rewardContainer = new RewardCardsContainer();
        _content.Add(_rewardContainer);

        _rewardContainer.OnRewardSelected += RewardSelected;
    }

    void RewardSelected()
    {
        _continueButton = new("Continue", _ussCommonMenuButton, LoadMap);
        _content.Add(_continueButton);
    }

    void LoadMap() { _gameManager.LoadMap(); }
}
