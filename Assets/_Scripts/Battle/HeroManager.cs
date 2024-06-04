using System;
using Cinemachine;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using Lis.Units.Hero.Tablets;
using UnityEngine;

namespace Lis.Battle
{
    public class HeroManager : Singleton<HeroManager>
    {
        GameManager _gameManager;
        AudioManager _audioManager;
        FightManager _fightManager;

        HeroElement _heroElement;

        public CinemachineVirtualCamera HeroFollowCamera;

        [SerializeField] AudioListener _placeholderAudioListener;

        [HideInInspector] public HeroController HeroController;
        public Hero Hero { get; private set; }

        [SerializeField] Sound _levelUpSound;

        public int RewardRerollsAvailable;

        // HERE: testing
        readonly bool _turnOffAbility = false;

        public event Action<Hero> OnHeroInitialized;

        public void Initialize(Hero hero)
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _fightManager = GetComponent<FightManager>();

            Hero = hero;
            hero.InitializeBattle(0);
            hero.OnLevelUp += OnHeroLevelUp;

            InitializeHeroGameObject(hero);

            RewardRerollsAvailable = _gameManager.UpgradeBoard.GetUpgradeByName("Reward Reroll").GetCurrentLevel()
                .Value;

            OnHeroInitialized?.Invoke(hero);

            Hero.OnTabletAdvancedAdded += OnTabletAdvancedAdded;
            if (hero.StartingAbility == null || _turnOffAbility) return;
            Hero.AddAbility(hero.StartingAbility);
        }

        void InitializeHeroGameObject(Hero hero)
        {
            Vector3 pos = Vector3.zero; //GetComponent<ArenaManager>().GetRandomPositionInPlayerLockerRoom();
            GameObject heroGameObject = Instantiate(hero.Prefab, pos, Quaternion.identity);
            HeroController = heroGameObject.GetComponentInChildren<HeroController>();
            HeroController.InitializeGameObject();
            HeroFollowCamera.Follow = heroGameObject.transform;
            _fightManager.AddPlayerUnit(HeroController);

            _placeholderAudioListener.enabled = false;
            HeroController.InitializeUnit(hero, 0);
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