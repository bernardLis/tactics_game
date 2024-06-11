using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class LostBattleScreen : FinishedBattleScreen
    {
        private const string _ussClassName = "finished-battle-screen__";
        private const string _ussMain = _ussClassName + "lost-main";

        public LostBattleScreen()
        {
            MainContainer.AddToClassList(_ussMain);
            AddButtons();

            Title.text = "Battle lost!";
        }

        private void AddButtons()
        {
            VisualElement container = new();
            container.style.alignItems = Align.Center;
            MainContainer.Add(container);

            MyButton noAdvantage = new("Back To Main Menu", USSCommonButton, BackToMainMenu);
            container.Add(noAdvantage);
        }

        private void BackToMainMenu()
        {
            GameManager.LoadScene(Scenes.MainMenu);
        }
    }
}