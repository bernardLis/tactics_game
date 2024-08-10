using System;
using Cinemachine;
using Lis.Camp.Building;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.HeroCreation;
using Lis.Units.Hero;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class HeroManager : Singleton<HeroManager>
    {
        public CinemachineVirtualCamera HeroFollowCamera;

        [SerializeField] AudioListener _placeholderAudioListener;

        [HideInInspector] public HeroController HeroController;

        [SerializeField] Sound _levelUpSound;

        // HERE: testing
        readonly bool _turnOffAbility = false;
        AudioManager _audioManager;
        FightManager _fightManager;
        GameManager _gameManager;

        HeroElement _heroElement;
        public Hero Hero { get; private set; }

        public event Action<Hero> OnHeroInitialized;

        public void Initialize(Hero hero)
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _fightManager = GetComponent<FightManager>();

            Hero = hero;
            hero.InitializeFight(0);

            GetComponent<UIDocument>().rootVisualElement.Add(new HeroElement(GameManager.Instance.Campaign.Hero));

            InitializeHeroGameObject(hero);

            OnHeroInitialized?.Invoke(hero);

            Hero.OnTabletAdvancedAdded += OnTabletAdvancedAdded;
            Hero.OnLevelUp += OnHeroLevelUp;

            if (hero.StartingAbility == null || _turnOffAbility) return;
            Hero.AddAbility(hero.StartingAbility);
        }

        void InitializeHeroGameObject(Hero hero)
        {
            Vector3 pos = Vector3.zero;
            GameObject heroGameObject = Instantiate(hero.Prefab, pos, Quaternion.identity);
            HeroController = heroGameObject.GetComponentInChildren<HeroController>();
            HeroController.InitializeGameObject();
            HeroFollowCamera.Follow = heroGameObject.transform;
            _fightManager.AddPlayerUnit(HeroController);

            _placeholderAudioListener.enabled = false;
            HeroController.InitializeUnit(hero, 0);

            ItemDisplayer id = heroGameObject.GetComponentInChildren<ItemDisplayer>();
            id.SetVisualHero(hero.VisualHero);
        }

        void OnHeroLevelUp()
        {
            _audioManager.CreateSound().WithSound(_levelUpSound).Play();
            LevelUpRewardScreen levelUpScreen = new();
            levelUpScreen.Initialize();
        }

        void OnTabletAdvancedAdded(TabletAdvanced tabletAdvanced)
        {
            Hero.AddAbility(tabletAdvanced.Ability);
            TabletAdvancedScreen tabletAdvancedScreen = new(tabletAdvanced);
        }

        public void DiceCollected()
        {
            Hero.RewardRerolls++;
        }
    }
}