using System;
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
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.MenuStyles);
            if (ss != null) styleSheets.Add(ss);

            _container = new();
            _container.AddToClassList(_ussContainer);
            Content.Add(_container);
            Content.style.height = Length.Percent(100);

            _container.Add(new StatsElement());
            _container.Add(new HorizontalSpacerElement());

            AddMenuButtons();

            BattleManager = BattleManager.Instance;
            if (BattleManager == null) return;
            HeroController hc = BattleManager.GetComponent<HeroManager>().HeroController;
            if (hc == null) return;
            Add(new HeroElement(hc.Hero, true));
        }

        void AddMenuButtons()
        {
            VisualElement buttonContainer = new();
            buttonContainer.AddToClassList(_ussButtonContainer);
            _container.Add(buttonContainer);

            AddContinueButton();
            MyButton settingsButton = new("Settings", USSCommonButton, ShowSettingsScreen);
            MyButton mainMenuButton = new("Abandon Run", USSCommonButton, GoToMainMenu);

            buttonContainer.Add(ContinueButton);
            buttonContainer.Add(settingsButton);
            buttonContainer.Add(mainMenuButton);
        }

        void ShowSettingsScreen()
        {
            SettingsScreen settingsScreen = new();
            if (settingsScreen == null) throw new ArgumentNullException(nameof(settingsScreen));
        }

        void GoToMainMenu()
        {
            GameManager.GameStats.AddStats(BattleManager.Battle.Stats);
            GameManager.LoadScene(Scenes.MainMenu);
            Hide();
        }
    }
}