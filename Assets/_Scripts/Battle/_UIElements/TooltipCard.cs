using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class TooltipCard : VisualElement
    {
        private const string _ussCommonTextPrimary = "common__text-primary";

        private const string _ussClassName = "tooltip-card__";
        private const string _ussMain = _ussClassName + "main";
        private const string _ussBackground = _ussClassName + "background";

        private const string _ussTopContainer = _ussClassName + "top-container";
        private const string _ussMiddleContainer = _ussClassName + "middle-container";
        private const string _ussBottomContainer = _ussClassName + "bottom-container";

        private const string _ussTopLeftContainer = _ussClassName + "top-left-container";
        private const string _ussTopMiddleContainer = _ussClassName + "top-middle-container";

        protected const string USSName = _ussClassName + "name";
        private VisualElement _middleContainer;

        private VisualElement _topContainer;
        protected VisualElement BottomContainer;

        protected GameManager GameManager;

        protected VisualElement TopLeftContainer;
        protected VisualElement TopRightContainer;


        protected void Initialize()
        {
            GameManager = GameManager.Instance;

            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TooltipCardStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);

            VisualElement bg = new();
            bg.AddToClassList(_ussBackground);
            Add(bg);

            _topContainer = new();
            _topContainer.AddToClassList(_ussTopContainer);
            _middleContainer = new();
            _middleContainer.AddToClassList(_ussMiddleContainer);
            BottomContainer = new();
            BottomContainer.AddToClassList(_ussBottomContainer);

            Add(_topContainer);
            Add(_middleContainer);
            Add(BottomContainer);

            TopLeftContainer = new();
            TopLeftContainer.AddToClassList(_ussTopLeftContainer);
            TopRightContainer = new();
            TopRightContainer.AddToClassList(_ussTopMiddleContainer);

            _topContainer.Add(TopLeftContainer);
            _topContainer.Add(TopRightContainer);
        }
    }
}