using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Cinemachine;

namespace Lis
{
    public class BattleHeroManager : MonoBehaviour
    {
        GameManager _gameManager;
        BattleManager _battleManager;

        VisualElement _root;
        VisualElement _bottomPanel;
        HeroElement _heroBattleElement;

        [SerializeField] CinemachineVirtualCamera _heroFollowCamera;

        [SerializeField] AudioListener _placeholderAudioListener;

        [SerializeField] GameObject _heroPrefab;
        [HideInInspector] public BattleHero BattleHero;
        public Hero Hero { get; private set; }

        LevelUpScreen _levelUpScreen;

        public int RewardRerollsAvailable;

        public void Initialize(Hero hero)
        {
            _gameManager = GameManager.Instance;
            _battleManager = GetComponent<BattleManager>();
            _root = GetComponent<UIDocument>().rootVisualElement;
            _bottomPanel = _root.Q<VisualElement>("bottomPanel");

            Hero = hero;
            hero.InitializeBattle(0);
            hero.OnLevelUp += OnHeroLevelUp;

            GameObject heroGameObject = Instantiate(_heroPrefab, Vector3.zero + Vector3.up * 10f,
                Quaternion.identity);
            BattleHero = heroGameObject.GetComponentInChildren<BattleHero>();
            _heroFollowCamera.Follow = heroGameObject.transform;

            BattleHero.OnDeath += (_, _) => _battleManager.LoseBattle();

            _placeholderAudioListener.enabled = false;
            StartCoroutine(MakeHeroFall(hero));

            RewardRerollsAvailable =
                _gameManager.UpgradeBoard.GetUpgradeByName("Reward Reroll").GetCurrentLevel().Value;
            
            Hero.OnTabletAdvancedAdded += OnTabletAdvancedAdded;

#if UNITY_EDITOR
            if (_abilityToGive != null)
            {
                Hero.AddAbility(_abilityToGive);
                _currentIndex = _allAbilities.IndexOf(_abilityToGive) + 1;
            }
#endif
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

#if UNITY_EDITOR
        [SerializeField] Ability _abilityToGive;
        [SerializeField] List<Ability> _allAbilities = new();
        int _currentIndex;
        [ContextMenu("Next Ability")]
        void NextAbility()
        {
            Hero.StopAllAbilities();

            _abilityToGive = _allAbilities[_currentIndex];
            _currentIndex++;
            if (_currentIndex >= _allAbilities.Count)
                _currentIndex = 0;
            if (Hero.Abilities.Contains(_abilityToGive))
            {
                _abilityToGive.StartAbility();
                return;
            }

            Hero.AddAbility(_abilityToGive);
        }
#endif
    }
}