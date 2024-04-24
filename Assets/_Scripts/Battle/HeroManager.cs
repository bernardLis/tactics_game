using Cinemachine;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
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
        FightManager _fightManager;

        VisualElement _root;
        VisualElement _bottomPanel;
        HeroElement _heroBattleElement;

        public CinemachineVirtualCamera HeroFollowCamera;

        [SerializeField] AudioListener _placeholderAudioListener;

        [HideInInspector] public HeroController HeroController;
        public Hero Hero { get; private set; }

        LevelUpRewardScreen _levelUpRewardScreen;
        [SerializeField] Sound _levelUpSound;

        public int RewardRerollsAvailable;

        // HERE: testing
        readonly bool _turnOffAbility = true;

        public void Initialize(Hero hero)
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _root = GetComponent<UIDocument>().rootVisualElement;
            _bottomPanel = _root.Q<VisualElement>("bottomPanel");
            _fightManager = GetComponent<FightManager>();

            Hero = hero;
            hero.InitializeBattle(0);
            hero.OnLevelUp += OnHeroLevelUp;

            InitializeHeroGameObject(hero);

            RewardRerollsAvailable = _gameManager.UpgradeBoard.GetUpgradeByName("Reward Reroll").GetCurrentLevel()
                .Value;

            Hero.OnTabletAdvancedAdded += OnTabletAdvancedAdded;
            if (hero.StartingAbility == null || _turnOffAbility) return;
            Hero.AddAbility(hero.StartingAbility);
        }

        void InitializeHeroGameObject(Hero hero)
        {
            GameObject heroGameObject = Instantiate(hero.Prefab, Vector3.zero, Quaternion.identity);
            HeroController = heroGameObject.GetComponentInChildren<HeroController>();
            HeroController.InitializeGameObject();
            HeroFollowCamera.Follow = heroGameObject.transform;
            _fightManager.AddPlayerUnit(HeroController);

            _placeholderAudioListener.enabled = false;

            HeroController.InitializeUnit(hero, 0);
            HeroController.GetComponent<NavMeshAgent>().enabled = true;

            _heroBattleElement = new(hero);
            _bottomPanel.Add(_heroBattleElement);
        }

        void OnHeroLevelUp()
        {
            _levelUpRewardScreen = new();
            _audioManager.PlayUI(_levelUpSound);
            _levelUpRewardScreen.OnHide += () => Hero.AddExp(Hero.LeftoverExp);
        }

        void OnTabletAdvancedAdded(TabletAdvanced tabletAdvanced)
        {
            if (_levelUpRewardScreen != null) _levelUpRewardScreen.Hide();
            Hero.AddAbility(tabletAdvanced.Ability);
            TabletAdvancedScreen tabletAdvancedScreen = new(tabletAdvanced);
        }
    }
}