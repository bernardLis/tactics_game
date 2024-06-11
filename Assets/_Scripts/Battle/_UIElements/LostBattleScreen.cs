using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class LostBattleScreen : FinishedBattleScreen
    {
        const string _ussClassName = "finished-battle-screen__";
        const string _ussMain = _ussClassName + "lost-main";

        public LostBattleScreen()
        {
            MainContainer.AddToClassList(_ussMain);
            AddButtons();

            Title.text = "Battle lost!";
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