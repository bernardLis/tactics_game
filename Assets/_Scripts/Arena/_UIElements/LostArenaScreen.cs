using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class LostArenaScreen : FinishedArenaScreen
    {
        const string _ussClassName = "finished-arena-screen__";
        const string _ussMain = _ussClassName + "lost-main";

        public LostArenaScreen()
        {
            MainContainer.AddToClassList(_ussMain);
            AddButtons();

            Title.text = "Fight lost!";
        }

        void AddButtons()
        {
            VisualElement container = new();
            container.style.alignItems = Align.Center;
            MainContainer.Add(container);

            MyButton noAdvantage = new("Back To Main Menu", USSCommonButton, BackToMainMenu);
            container.Add(noAdvantage);
        }

        void BackToMainMenu()
        {
            GameManager.LoadScene(Scenes.MainMenu);
        }
    }
}