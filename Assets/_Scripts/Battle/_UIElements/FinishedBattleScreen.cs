using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class FinishedBattleScreen : FullScreenElement
    {
        const string _ussClassName = "finished-battle-screen__";
        const string _ussMain = _ussClassName + "main";
        protected AudioManager AudioManager;

        protected readonly VisualElement MainContainer;
        protected readonly Label Title;
        protected readonly Label SubTitle;

        protected FinishedBattleScreen()
        {
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.FinishedBattleStyles);
            if (ss != null) styleSheets.Add(ss);
            AudioManager = AudioManager.Instance;

            MainContainer = new();
            MainContainer.AddToClassList(_ussMain);
            Content.Add(MainContainer);
            Content.style.width = Length.Percent(100);
            Content.style.height = Length.Percent(100);

            Title = new();
            Title.style.fontSize = 34;
            MainContainer.Add(Title);

            SubTitle = new();
            MainContainer.Add(SubTitle);

            MainContainer.Add(new HorizontalSpacerElement());
            MainContainer.Add(new StatsElement());
            MainContainer.Add(new HorizontalSpacerElement());

            DisableNavigation();
        }
    }
}