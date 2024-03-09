using Lis.Battle;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class MenuScreen : FullScreenElement
    {
        const string _ussClassName = "menu__";
        const string _ussContainer = _ussClassName + "container";
        const string _ussButtonContainer = _ussClassName + "button-container";

        readonly VisualElement _container;

        public MenuScreen()
        {
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.MenuStyles);
            if (ss != null) styleSheets.Add(ss);

            _container = new();
            _container.AddToClassList(_ussContainer);
            _content.Add(_container);

            _container.Add(new StatsElement());
            _container.Add(new HorizontalSpacerElement());

            AddMenuButtons();

            _battleManager = BattleManager.Instance;
            if (_battleManager == null) return;
            if (_battleManager.HeroController == null) return;
            Add(new HeroElement(_battleManager.HeroController.Hero, true));
        }

        void AddMenuButtons()
        {
            VisualElement buttonContainer = new();
            buttonContainer.AddToClassList(_ussButtonContainer);
            _container.Add(buttonContainer);

            AddContinueButton();
            MyButton settingsButton = new("Settings", _ussCommonMenuButton, ShowSettingsScreen);
            MyButton mainMenuButton = new("Abandon Run", _ussCommonMenuButton, GoToMainMenu);

            buttonContainer.Add(_continueButton);
            buttonContainer.Add(settingsButton);
            buttonContainer.Add(mainMenuButton);
        }

        void ShowSettingsScreen()
        {
            new SettingsScreen();
        }

        void GoToMainMenu()
        {
            _gameManager.GameStats.AddStats(_battleManager.Battle.Stats);
            _gameManager.LoadScene(Scenes.MainMenu);
            Hide();
        }
    }
}