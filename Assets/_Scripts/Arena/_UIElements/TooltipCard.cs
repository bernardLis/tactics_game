using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class TooltipCard : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "tooltip-card__";
        const string _ussMain = _ussClassName + "main";
        const string _ussBackground = _ussClassName + "background";

        const string _ussTopContainer = _ussClassName + "top-container";
        const string _ussMiddleContainer = _ussClassName + "middle-container";
        const string _ussBottomContainer = _ussClassName + "bottom-container";

        const string _ussTopLeftContainer = _ussClassName + "top-left-container";
        const string _ussTopMiddleContainer = _ussClassName + "top-middle-container";

        protected const string USSName = _ussClassName + "name";
        VisualElement _middleContainer;

        VisualElement _topContainer;
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