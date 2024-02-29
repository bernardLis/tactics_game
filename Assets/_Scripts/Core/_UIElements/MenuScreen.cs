using Lis.Core.Utilities;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class MenuScreen : FullScreenElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "menu__";
        const string _ussContainer = _ussClassName + "container";
        const string _ussButtonContainer = _ussClassName + "button-container";

        readonly VisualElement _container;
        public MenuScreen() : base()
        {
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.MenuStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _container = new();
            _container.AddToClassList(_ussContainer);
            _content.Add(_container);

            _container.Add(new StatsBattleElement());
            _container.Add(new HorizontalSpacerElement());

            AddMenuButtons();

            BattleManager battleManager = BattleManager.Instance;
            if (battleManager == null) return;
            if (battleManager.HeroController == null) return;
            Add(new HeroElement(battleManager.HeroController.Hero, true));
        }

        void AddMenuButtons()
        {
            VisualElement buttonContainer = new();
            buttonContainer.AddToClassList(_ussButtonContainer);
            _container.Add(buttonContainer);

            AddContinueButton();
            MyButton settingsButton = new("Settings", _ussCommonMenuButton, ShowSettingsScreen);
            MyButton mainMenuButton = new("Main Menu", _ussCommonMenuButton, GoToMainMenu);
            MyButton quitButton = new("Quit", _ussCommonMenuButton, ConfirmQuit);

            buttonContainer.Add(_continueButton);
            buttonContainer.Add(settingsButton);
            buttonContainer.Add(mainMenuButton);
            buttonContainer.Add(quitButton);
        }

        void ShowSettingsScreen() { new SettingsScreen(); }

        void GoToMainMenu()
        {
            _gameManager.LoadScene(Scenes.MainMenu);
            Hide();
        }

        void ConfirmQuit()
        {
            ConfirmPopUp pop = new();
            pop.Initialize(GameManager.Instance.Root, Quit);
        }

        void Quit() { Application.Quit(); }
    }
}
