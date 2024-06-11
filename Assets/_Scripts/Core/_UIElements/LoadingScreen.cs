using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class LoadingScreen : FullScreenElement
    {
        const string _ussCommonLoadingScreen = "common__loading-screen";

        readonly Label _dotsLabel;

        int _dots;

        public LoadingScreen()
        {
            AddToClassList(_ussCommonLoadingScreen);
            DisableNavigation();

            Content.style.justifyContent = Justify.SpaceAround;

            VisualElement loadingContainer = new();
            loadingContainer.style.width = Length.Percent(100);
            loadingContainer.style.flexDirection = FlexDirection.Row;
            loadingContainer.style.justifyContent = Justify.Center;
            Content.Add(loadingContainer);

            loadingContainer.Add(new Label("Loading"));
            _dotsLabel = new("");
            _dotsLabel.style.width = 50;
            _dotsLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            loadingContainer.Add(_dotsLabel);

            Content.Add(new HorizontalSpacerElement());
            Content.Add(new NatureComboElement(GameManager.UnitDatabase.GetRandomAdvancedNature()));

            IVisualElementScheduledItem loadingDots = schedule.Execute(AddDots).Every(400);
            OnHide += () => loadingDots.Pause();
        }

        void AddDots()
        {
            _dots++;
            if (_dots > 3) _dots = 0;
            _dotsLabel.text = new('.', _dots);
        }
    }
}