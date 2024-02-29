using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class LostBattleScreen : FinishedBattleScreen
    {
        const string _ussClassName = "finished-battle-screen__";
        const string _ussMain = _ussClassName + "lost-main";

        public LostBattleScreen() : base()
        {
            _mainContainer.AddToClassList(_ussMain);
            AddButtons();

            AudioManager audioManager = AudioManager.Instance;
            audioManager.PlayDialogue(audioManager.GetSound("You Lost"));
        }

        protected override void AddTitle()
        {
            Label text = new("Battle lost!");
            text.style.fontSize = 34;

            _mainContainer.Add(text);
        }

        void AddButtons()
        {
            VisualElement container = new();
            container.style.alignItems = Align.Center;
            _mainContainer.Add(container);

            MyButton noAdvantage = new("Back To Main Menu", _ussCommonMenuButton, BackToMainMenu);
            container.Add(noAdvantage);
        }

        void BackToMainMenu()
        {
            _gameManager.LoadScene(Scenes.MainMenu);
        }
    }
}
