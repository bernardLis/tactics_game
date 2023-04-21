using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    Battle _battle;

    VisualElement _content;
    MyButton _continueButton;


    VisualElement _rewardContainer;
    List<RewardCard> _allRewardCards = new();
    List<RewardCard> _selectedRewardCards = new();



    public BattleResult(VisualElement root, Battle battle, List<BattleEntity> entities)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        // TODO: different styles won/lost
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleResultStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root, false);

        _battle = battle;

        AddToClassList(_ussCommonTextPrimary);

        if (battle.Won)
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

        /*
        0nd.show battle stats

        1st.show character getting exp and allow choosing level

        2nd.show rewards

        3rd.go back
        */

        AddEntityWithMostKills(entities);
        _continueButton = new("Continue", _ussCommonMenuButton, ShowCharacterCard);
        _content.Add(_continueButton);
    }

    void ShowCharacterCard()
    {
        _content.Clear();

        HeroCardQuest card = new HeroCardQuest(_gameManager.PlayerHero);
        _gameManager.PlayerHero.GetExp(100);
        // TODO: normally, if the hero is not leveled up, we should wait a bit and show the rewards
        card.OnLeveledUp += ShowRewards;
        _content.Add(card);
    }

    void ShowRewards()
    {
        _content.Clear();
        _content.Add(new HeroCardMini(_gameManager.PlayerHero));
        AddRewardContainer();
    }

    void AddEntityWithMostKills(List<BattleEntity> entities)
    {
        BattleEntity entityWithMostKills = entities[0];
        int topKillCount = entities[0].KilledEnemiesCount;
        foreach (BattleEntity e in entities)
        {
            if (e.KilledEnemiesCount > topKillCount)
            {
                topKillCount = e.KilledEnemiesCount;
                entityWithMostKills = e;
            }
        }
        _content.Add(new Label($"Entity With Most Kills: {entityWithMostKills.name}, # kills: {topKillCount} "));
    }

    void AddRewardContainer()
    {
        VisualElement parentContainer = new();
        _content.Add(parentContainer);

        _rewardContainer = new();
        _rewardContainer.style.flexDirection = FlexDirection.Row;
        parentContainer.Add(_rewardContainer);
        PopulateRewards();

        MyButton rerollButton = new("Reroll", _ussCommonMenuButton, RerollReward);
        // TODO: dice + gold element
        parentContainer.Add(rerollButton);
    }

    void RerollReward()
    {
        // you gotta pay for it.
        PopulateRewards();
    }

    void PopulateRewards()
    {
        _rewardContainer.Clear();
        _allRewardCards.Clear();
        CreateRewardCards();
        ChooseRewardCards();
    }

    void CreateRewardCards()
    {
        _allRewardCards.Add(CreateRewardCardItem());
        _allRewardCards.Add(CreateRewardCardAbility());
        _allRewardCards.Add(CreateRewardCardGold());
        _allRewardCards.Add(CreateRewardCardArmy());
    }

    void ChooseRewardCards()
    {
        for (int i = 0; i < 3; i++)
        {
            RewardCard card = _allRewardCards[Random.Range(0, _allRewardCards.Count)];
            _allRewardCards.Remove(card);
            _selectedRewardCards.Add(card);
            _rewardContainer.Add(card);
        }
    }

    RewardCard CreateRewardCardItem()
    {
        RewardItem reward = ScriptableObject.CreateInstance<RewardItem>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardItem(reward);
    }

    RewardCard CreateRewardCardAbility()
    {
        RewardAbility reward = ScriptableObject.CreateInstance<RewardAbility>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardAbility(reward);
    }

    RewardCard CreateRewardCardGold()
    {
        RewardGold reward = ScriptableObject.CreateInstance<RewardGold>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardGold(reward);

    }

    RewardCard CreateRewardCardArmy()
    {
        RewardArmy reward = ScriptableObject.CreateInstance<RewardArmy>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardArmy(reward);
    }

    void RewardSelected()
    {
        foreach (RewardCard card in _selectedRewardCards)
            card.SetEnabled(false);
    }

    void LoadMap() { _gameManager.LoadMap(); }
}
