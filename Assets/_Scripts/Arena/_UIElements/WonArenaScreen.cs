using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class WonArenaScreen : FinishedArenaScreen
    {
        const string _ussClassName = "finished-arena-screen__";
        const string _ussMain = _ussClassName + "won-main";

        public WonArenaScreen()
        {
            AddToClassList(_ussMain);
            AddButtons();

            Title.text = "Fight Won!";

            SubTitle.text = "You won, congratz! Here, I am giving you a virtual handshake and a medal!";
            SubTitle.style.whiteSpace = WhiteSpace.Normal;
            SubTitle.style.fontSize = 24;
        }

        void AddButtons()
        {
            VisualElement container = new();
            container.style.alignItems = Align.Center;
            MainContainer.Add(container);

            MyButton noAdvantage = new("Back To Main Menu", USSCommonButton, QuitButton);
            container.Add(noAdvantage);
        }

        void QuitButton()
        {
            GameManager.LoadScene(Scenes.MainMenu);
        }
    }
}