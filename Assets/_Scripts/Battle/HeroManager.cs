using System;
using Cinemachine;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class HeroManager : Singleton<HeroManager>
    {
        GameManager _gameManager;
        AudioManager _audioManager;
        FightManager _fightManager;

        VisualElement _root;
        HeroElement _heroElement;

        public CinemachineVirtualCamera HeroFollowCamera;

        [SerializeField] AudioListener _placeholderAudioListener;

        [HideInInspector] public HeroController HeroController;
        public Hero Hero { get; private set; }

        [SerializeField] Sound _levelUpSound;

        public int RewardRerollsAvailable;

        // HERE: testing
        readonly bool _turnOffAbility = true;

        public event Action<Hero> OnHeroInitialized;

        public void Initialize(Hero hero)
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _root = GetComponent<UIDocument>().rootVisualElement;
            _fightManager = GetComponent<FightManager>();

            Hero = hero;
            hero.InitializeBattle(0);
            hero.OnLevelUp += OnHeroLevelUp;

            InitializeHeroGameObject(hero);
            AddHeroUI();

            RewardRerollsAvailable = _gameManager.UpgradeBoard.GetUpgradeByName("Reward Reroll").GetCurrentLevel()
                .Value;

            OnHeroInitialized?.Invoke(hero);

            Hero.OnTabletAdvancedAdded += OnTabletAdvancedAdded;
            if (hero.StartingAbility == null || _turnOffAbility) return;
            Hero.AddAbility(hero.StartingAbility);
        }

        void AddHeroUI()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.position = Position.Absolute;
            container.style.alignItems = Align.Center;
            container.style.right = 0;
            container.style.top = 0;
            container.style.backgroundColor = new(new Color(0, 0, 0, 0.5f));
            _root.Add(container);

            GoldElement goldElement = new(_gameManager.Gold);
            _gameManager.OnGoldChanged += goldElement.ChangeAmount;
            container.Add(goldElement);

            MyButton viewHeroButton = new("", "common__hero-button", () =>
            {
                HeroScreen heroScreen = new(Hero);
                heroScreen.Initialize();
            });
            container.Add(viewHeroButton);

            MyButton viewArmyButton = new("", "common__army-button", () =>
            {
                ArmyScreen ass = new();
            });
            container.Add(viewArmyButton);
        }


        void InitializeHeroGameObject(Hero hero)
        {
            Vector3 pos = GetComponent<ArenaManager>().GetRandomPositionInPlayerLockerRoom();
            GameObject heroGameObject = Instantiate(hero.Prefab, pos, Quaternion.identity);
            HeroController = heroGameObject.GetComponentInChildren<HeroController>();
            HeroController.InitializeGameObject();
            HeroFollowCamera.Follow = heroGameObject.transform;
            _fightManager.AddPlayerUnit(HeroController);

            _placeholderAudioListener.enabled = false;
            HeroController.InitializeUnit(hero, 0);

            _heroElement = new(hero);
            _root.Add(_heroElement);
        }

        void OnHeroLevelUp()
        {
            _audioManager.PlayUI(_levelUpSound);
        }

        void OnTabletAdvancedAdded(TabletAdvanced tabletAdvanced)
        {
            Hero.AddAbility(tabletAdvanced.Ability);
            TabletAdvancedScreen tabletAdvancedScreen = new(tabletAdvanced);
        }

        public void DiceCollected()
        {
            RewardRerollsAvailable++;
        }
    }
}