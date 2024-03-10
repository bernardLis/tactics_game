using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class FinishedBattleScreen : FullScreenElement
    {
        const string _ussClassName = "finished-battle-screen__";
        const string _ussMain = _ussClassName + "main";

        protected VisualElement _mainContainer;

        public FinishedBattleScreen() : base()
        {
            var ss = GameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.FinishedBattleScreenStyles);
            if (ss != null) styleSheets.Add(ss);


            _mainContainer = new();
            _mainContainer.AddToClassList(_ussMain);
            Content.Add(_mainContainer);

            AddTitle();

            _mainContainer.Add(new StatsElement());
            _mainContainer.Add(new HorizontalSpacerElement());

            DisableNavigation();
        }

        protected virtual void AddTitle()
        {
            // meant to be overwritten
        }
    }
}
