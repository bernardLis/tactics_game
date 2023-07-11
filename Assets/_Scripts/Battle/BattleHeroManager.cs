using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleHeroManager : MonoBehaviour
{
    [SerializeField] EntitySpawner _spawnerPrefab;

    BattleManager _battleManager;
    VisualElement _root;

    Hero _hero;
    public void Initialize(Hero hero)
    {
        _battleManager = BattleManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;

        _hero = hero;
        hero.BattleInitialize();
        hero.OnLevelUp += OnHeroLevelUp;
        hero.OnCreatureAdded += AddCreature;

        AddHeroCard();
    }

    void AddHeroCard()
    {
        VisualElement bottomPanel = _root.Q<VisualElement>("bottomPanel");

        HeroCardStats card = new(_hero);
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
            _hero.CurrentMana.ApplyChange(_hero.BaseMana.Value - _hero.CurrentMana.Value);
        };
    }

    void AddCreature(Creature c)
    {
        EntitySpawner instance = Instantiate(_spawnerPrefab, Vector3.forward, Quaternion.identity);

        List<Creature> creatures = new();
        creatures.Add(c);
        instance.SpawnCreatures(creatures: creatures);
        instance.OnSpawnComplete += (list) =>
        {
            _battleManager.AddPlayerArmyEntities(list);
            instance.DestroySelf();
        };
    }
}
