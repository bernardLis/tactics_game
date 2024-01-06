using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Cinemachine;
using UnityEngine.AI;

public class BattleHeroManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;

    VisualElement _root;
    VisualElement _bottomPanel;
    HeroElement _heroBattleElement;

    [SerializeField] AudioListener _placeholderAudioListener;

    [SerializeField] GameObject _heroPrefab;
    [SerializeField] Ability _abilityToGive;
    [HideInInspector] public BattleHero BattleHero;
    public Hero Hero { get; private set; }

    LevelUpScreen _levelUpScreen;

    public int RewardRerollsAvailable = 0;
    public void Initialize(Hero hero)
    {
        _gameManager = GameManager.Instance;
        _battleManager = GetComponent<BattleManager>();
        _battleAreaManager = GetComponent<BattleAreaManager>();
        _root = GetComponent<UIDocument>().rootVisualElement;
        _bottomPanel = _root.Q<VisualElement>("bottomPanel");

        Hero = hero;
        hero.InitializeBattle(0);
        hero.OnLevelUp += OnHeroLevelUp;

        BattleHero = Instantiate(_heroPrefab, _battleAreaManager.HomeTile.transform.position + Vector3.up * 10f,
                                Quaternion.identity).GetComponent<BattleHero>();

        BattleHero.OnDeath += (a, b) => _battleManager.LoseBattle();

        _placeholderAudioListener.enabled = false;
        StartCoroutine(MakeHeroFall(hero));

        RewardRerollsAvailable = _gameManager.UpgradeBoard.GetUpgradeByName("Reward Reroll").GetCurrentLevel().Value;

        if (_abilityToGive != null)
            Hero.AddAbility(_abilityToGive);

        Hero.OnTabletAdvancedAdded += OnTabletAdvancedAdded;
    }

    IEnumerator MakeHeroFall(Hero hero)
    {
        Animator heroAnimator = BattleHero.GetComponentInChildren<Animator>();
        heroAnimator.SetBool("FreeFall", true);
        BattleHero.transform.DOMoveY(0f, 1f);
        yield return new WaitForSeconds(0.5f);
        heroAnimator.SetBool("FreeFall", false);
        heroAnimator.SetBool("Grounded", true);

        BattleHero.InitializeEntity(hero, 0);
        _battleManager.AddPlayerArmyEntity(BattleHero);
        _heroBattleElement = new(hero);
        _bottomPanel.Add(_heroBattleElement);
        BattleHero.GetComponent<NavMeshAgent>().enabled = true;
    }

    void OnHeroLevelUp()
    {
        _levelUpScreen = new();

        _levelUpScreen.OnHide += () => Hero.AddExp(Hero.LeftoverExp);
    }

    void OnTabletAdvancedAdded(TabletAdvanced tabletAdvanced)
    {
        _levelUpScreen.Hide();
        TabletAdvancedScreen tabletAdvancedScreen = new(tabletAdvanced);
    }

}
