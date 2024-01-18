

using UnityEngine.UIElements;

namespace Lis
{
    public class FinishedBattleScreen : FullScreenElement
    {
        const string _ussClassName = "finished-battle-screen__";
        const string _ussMain = _ussClassName + "main";

        protected VisualElement _mainContainer;

        public FinishedBattleScreen() : base()
        {
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.FinishedBattleScreenStyles);
            if (ss != null) styleSheets.Add(ss);


            _mainContainer = new();
            _mainContainer.AddToClassList(_ussMain);
            _content.Add(_mainContainer);

            AddTitle();

            _mainContainer.Add(new StatsBattleElement());
            _mainContainer.Add(new HorizontalSpacerElement());

            DisableNavigation();
        }

        protected virtual void AddTitle()
        {
            // meant to be overwritten
        }
    }
}
