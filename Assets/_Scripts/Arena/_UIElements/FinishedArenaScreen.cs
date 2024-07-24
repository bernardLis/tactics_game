using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class FinishedArenaScreen : FullScreenElement
    {
        const string _ussClassName = "finished-arena-screen__";
        const string _ussMain = _ussClassName + "main";

        protected readonly VisualElement MainContainer;
        protected readonly Label SubTitle;
        protected readonly Label Title;

        protected FinishedArenaScreen()
        {
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.FinishedArenaStyles);
            if (ss != null) styleSheets.Add(ss);

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