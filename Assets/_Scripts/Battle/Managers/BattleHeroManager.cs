using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleHeroManager : MonoBehaviour
{
    [SerializeField] EntitySpawner _spawnerPrefab;


    BattleManager _battleManager;
    BattleIntroManager _battleIntroManager;
    VisualElement _root;

    public Hero Hero { get; private set; }
    public void Initialize(Hero hero)
    {
        _battleManager = BattleManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;

        Hero = hero;
        hero.BattleInitialize();
        hero.OnLevelUpReady += OnHeroLevelUp;

        _battleIntroManager = BattleIntroManager.Instance;
        if (_battleIntroManager != null)
            _battleIntroManager.OnIntroFinished += AddHeroCard;
        else
            AddHeroCard();
    }

    void AddHeroCard()
    {
        VisualElement bottomPanel = _root.Q<VisualElement>("bottomPanel");

        HeroCardStats card = new(Hero);
        bottomPanel.Insert(0, card);
        card.style.opacity = 0;

        DOTween.To(x => card.style.opacity = x, 0, 1, 0.5f);
    }

    void OnHeroLevelUp()
    {
        _battleManager.PauseGame();

        VisualElement blackout = new();
        blackout.style.backgroundColor = new Color(0, 0, 0, 0.3f);
        blackout.style.position = Position.Absolute;
        blackout.style.width = Length.Percent(100);
        blackout.style.height = Length.Percent(100);
        _root.Add(blackout);

        BattleRewardElement rewardElement = new();
        _root.Add(rewardElement);

        rewardElement.OnContinueClicked += () =>
        {
            _battleManager.ResumeGame();
            _root.Remove(rewardElement);
            _root.Remove(blackout);
            Hero.CurrentMana.ApplyChange(Hero.BaseTotalMana.Value - Hero.CurrentMana.Value);
            Hero.AddExp(Hero.LeftoverExp);
        };
    }
}
