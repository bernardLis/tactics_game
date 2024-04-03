using UnityEngine.UIElements;

namespace Lis.Core
{
    public class LoadingScreen : FullScreenElement
    {
        const string _ussCommonLoadingScreen = "common__loading-screen";

        public LoadingScreen()
        {
            AddToClassList(_ussCommonLoadingScreen);
            DisableNavigation();
            Content.style.justifyContent = Justify.SpaceAround;
            Content.Add(new Label("Loading..."));
            Content.Add(new HorizontalSpacerElement());
            Content.Add(new NatureComboElement(GameManager.UnitDatabase.GetRandomAdvancedNature()));
        }
    }
}