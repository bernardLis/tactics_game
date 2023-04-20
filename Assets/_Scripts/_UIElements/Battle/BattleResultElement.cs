using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleResult : FullScreenElement
{

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "quest-result__";
    const string _ussWonMain = _ussClassName + "won-main";
    const string _ussLostMain = _ussClassName + "lost-main";
    const string _ussContent = _ussClassName + "content";

    GameManager _gameManager;
    AudioManager _audioManager;

    Battle _battle;

    VisualElement _content;
    MyButton _backToMapButton;

    public BattleResult(VisualElement root, Battle battle, List<BattleEntity> entities)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        // TODO: different styles won/lost
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.QuestResultStyles);
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

        AddEntityWithMostKills(entities);
        AddHeroCard(battle);
        AddRewardCards();

        _backToMapButton = new("Back", _ussCommonMenuButton, LoadMap);
        _content.Add(_backToMapButton);
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

    void AddHeroCard(Battle battle)
    {
        HeroCardQuest card = new HeroCardQuest(battle.Hero);
        if (battle.Won)
            battle.Hero.GetExp(100);
        _content.Add(card);
    }

    void AddRewardCards()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _content.Add(container);

        for (int i = 0; i < 3; i++)
        {
            RewardCard card = CreateRewardCard();
            container.Add(card);
        }

        // TODO: skip & reroll buttons
    }

    RewardCard CreateRewardCard()
    {
        Reward reward;
        float v = Random.value;
        if (v < 0.25f)
        {
            reward = ScriptableObject.CreateInstance<RewardItem>();
            reward.CreateRandom(_battle.Hero);
            return new RewardCard(reward);
        }
        else if (v < 0.5f)
        {
            reward = ScriptableObject.CreateInstance<RewardAbility>();
            reward.CreateRandom(_battle.Hero);
            return new RewardCard(reward);
        }
        else if (v < 0.75f)
        {
            reward = ScriptableObject.CreateInstance<RewardGold>();
            reward.CreateRandom(_battle.Hero);
            return new RewardCard(reward);
        }
        else
        {
            reward = ScriptableObject.CreateInstance<RewardArmy>();
            reward.CreateRandom(_battle.Hero);
            return new RewardCard(reward);
        }

    }

    void LoadMap() { _gameManager.LoadMap(); }

}
