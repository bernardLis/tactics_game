
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis
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

        protected const string _ussName = _ussClassName + "name";

        protected VisualElement _topContainer;
        protected VisualElement _middleContainer;
        protected VisualElement _bottomContainer;

        protected VisualElement _topLeftContainer;
        protected VisualElement _topRightContainer;

        protected GameManager _gameManager;

        protected void Initialize()
        {
            _gameManager = GameManager.Instance;

            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TooltipCardStyles);
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
            _bottomContainer = new();
            _bottomContainer.AddToClassList(_ussBottomContainer);

            Add(_topContainer);
            Add(_middleContainer);
            Add(_bottomContainer);

            _topLeftContainer = new();
            _topLeftContainer.AddToClassList(_ussTopLeftContainer);
            _topRightContainer = new();
            _topRightContainer.AddToClassList(_ussTopMiddleContainer);

            _topContainer.Add(_topLeftContainer);
            _topContainer.Add(_topRightContainer);
        }
    }
}
