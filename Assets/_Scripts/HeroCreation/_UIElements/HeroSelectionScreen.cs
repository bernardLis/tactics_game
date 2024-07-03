using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class HeroSelectionScreen : FullScreenElement
    {
        public HeroSelectionScreen()
        {
            MyButton createHeroButton = new("Create Hero", USSCommonButton, CreateHero);
            MyButton randomHeroButton = new("Random Hero", USSCommonButton, StartGameWithRandomHero);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.Add(createHeroButton);
            container.Add(randomHeroButton);
            Content.Add(container);

            Content.Add(new HorizontalSpacerElement());

            ResolvePreviousHeroes();
        }

        void CreateHero()
        {
            GameManager.LoadScene(Scenes.HeroCreation);
            Hide();
        }

        void StartGameWithRandomHero()
        {
            GameManager.AddCurrentVisualHero();
            GameManager.StartGame();
            Hide();
        }

        void ResolvePreviousHeroes()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            Content.Add(container);

            foreach (VisualHero vh in GameManager.VisualHeroes)
            {
                MyButton heroButton = new(vh.Name, USSCommonButton, () =>
                {
                    GameManager.SelectHero(vh);
                    GameManager.StartGame();
                    Hide();
                });
                container.Add(heroButton);
            }
        }
    }
}