using Cinemachine;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.HeroCreation;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Camp
{
    public class CampManager : Singleton<CampManager>
    {
        public CinemachineVirtualCamera HeroFollowCamera;
        [SerializeField] Transform _armyHolder;

        Hero _hero;

        void Start()
        {
            _hero = GameManager.Instance.Campaign.Hero;
            _hero.InitializeFight(0);

            // HERE: testing
            GameManager.Instance.ChangeGoldValue(1000);
            for (int i = 0; i < 5; i++)
                _hero.AddArmy(Instantiate(GameManager.Instance.UnitDatabase.Peasant));

            InitializeHeroGameObject(_hero);

            VisualElement container =
                GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("mapButtonContainer");
            container.Add(
                new MyButton("Back To Map", "common__button", () => GameManager.Instance.LoadScene(Scenes.Map)));

            InitializeHeroArmy();

            GrabManager.Instance.Initialize();
        }

        void InitializeHeroGameObject(Hero hero)
        {
            Vector3 pos = Vector3.zero;
            GameObject heroGameObject = Instantiate(hero.CampPrefab, pos, Quaternion.identity);
            HeroFollowCamera.Follow = heroGameObject.transform;

            ItemDisplayer id = heroGameObject.GetComponentInChildren<ItemDisplayer>();
            id.SetVisualHero(hero.VisualHero);
        }

        void InitializeHeroArmy()
        {
            foreach (Unit u in _hero.Army)
            {
                UnitCampController ucc = SpawnUnit(u, new(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)));
                ucc.StartCampCoroutine();
            }
        }

        public UnitCampController SpawnUnit(Unit unit, Vector3 pos)
        {
            GameObject unitGameObject = Instantiate(unit.CampPrefab,
                pos,
                Quaternion.identity, _armyHolder);
            UnitCampController ucc = unitGameObject.GetComponent<UnitCampController>();
            ucc.Initialize(unit);
            return ucc;
        }
    }
}