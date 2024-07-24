using Cinemachine;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.HeroCreation;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class CampManager : Singleton<CampManager>
    {
        public CinemachineVirtualCamera HeroFollowCamera;

        void Start()
        {
            // spawn hero that can walk around
            Hero hero = GameManager.Instance.Campaign.Hero;
            hero.InitializeFight(0);

            InitializeHeroGameObject(hero);

            VisualElement container =
                GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("mapButtonContainer");

            container.Add(
                new MyButton("Back To Map", "common__button", () => GameManager.Instance.LoadScene(Scenes.Map)));
        }

        void InitializeHeroGameObject(Hero hero)
        {
            Vector3 pos = Vector3.zero;
            GameObject heroGameObject = Instantiate(hero.CampPrefab, pos, Quaternion.identity);
            HeroController heroController = heroGameObject.GetComponentInChildren<HeroController>();
            HeroFollowCamera.Follow = heroGameObject.transform;

            ItemDisplayer id = heroGameObject.GetComponentInChildren<ItemDisplayer>();
            id.SetVisualHero(hero.VisualHero);
        }
    }
}