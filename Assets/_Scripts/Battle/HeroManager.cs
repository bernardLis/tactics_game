using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Lis.Core;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class HeroManager : MonoBehaviour
    {
        GameManager _gameManager;
        AudioManager _audioManager;
        BattleManager _battleManager;

        VisualElement _root;
        VisualElement _bottomPanel;
        HeroElement _heroBattleElement;

        static readonly int AnimFreeFall = Animator.StringToHash("FreeFall");
        static readonly int AnimGrounded = Animator.StringToHash("Grounded");

        public CinemachineVirtualCamera HeroFollowCamera;

        [SerializeField] AudioListener _placeholderAudioListener;
        [SerializeField] GameObject _battleIntro;

        [HideInInspector] public HeroController HeroController;
        public Hero Hero { get; private set; }

        LevelUpScreen _levelUpScreen;
        [SerializeField] Sound _levelUpSound;

        public int RewardRerollsAvailable;

        // HERE: testing
        readonly bool _turnOffAbility = false;

        public void Initialize(Hero hero)
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _battleManager = GetComponent<BattleManager>();
            _root = GetComponent<UIDocument>().rootVisualElement;
            _bottomPanel = _root.Q<VisualElement>("bottomPanel");

            Hero = hero;
            hero.InitializeBattle(0);
            hero.OnLevelUp += OnHeroLevelUp;

            GameObject heroGameObject = Instantiate(hero.Prefab, Vector3.zero + Vector3.up * 10f,
                Quaternion.identity);
            HeroController = heroGameObject.GetComponentInChildren<HeroController>();
            HeroController.InitializeGameObject();
            HeroFollowCamera.Follow = heroGameObject.transform;

            _battleManager.PlayerEntities.Add(HeroController);
            HeroController.OnDeath += (_, _) => _battleManager.LoseBattle();

            _placeholderAudioListener.enabled = false;
            StartCoroutine(MakeHeroFall(hero));

            RewardRerollsAvailable =
                _gameManager.UpgradeBoard.GetUpgradeByName("Reward Reroll").GetCurrentLevel().Value;

            Hero.OnTabletAdvancedAdded += OnTabletAdvancedAdded;

            if (hero.StartingAbility == null || _turnOffAbility) return;
            Hero.AddAbility(hero.StartingAbility);
            _currentIndex = _allAbilities.IndexOf(_abilityToGive) + 1;
        }

        IEnumerator MakeHeroFall(Hero hero)
        {
            GameObject g = Instantiate(_battleIntro);
            g.GetComponent<IntroManager>().Initialize();

            Animator heroAnimator = HeroController.GetComponentInChildren<Animator>();
            heroAnimator.SetBool(AnimFreeFall, true);
            HeroController.transform.DOMoveY(0f, 1f);
            yield return new WaitForSeconds(0.5f);
            heroAnimator.SetBool(AnimFreeFall, false);
            heroAnimator.SetBool(AnimGrounded, true);

            HeroController.InitializeUnit(hero, 0);
            _battleManager.AddPlayerArmyEntity(HeroController);
            _heroBattleElement = new(hero);
            _bottomPanel.Add(_heroBattleElement);
            HeroController.GetComponent<NavMeshAgent>().enabled = true;
        }

        void OnHeroLevelUp()
        {
            _levelUpScreen = new();
            _audioManager.PlaySfx(_levelUpSound, HeroController.transform.position);
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