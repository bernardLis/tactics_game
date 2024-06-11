using Lis.Battle;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitIcon : ElementWithTooltip
    {
        const string _ussClassName = "unit-icon__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIconContainer = _ussClassName + "icon-container";
        const string _ussFrame = _ussClassName + "frame";

        readonly Unit _unit;
        protected readonly VisualElement Frame;

        protected readonly VisualElement IconContainer;
        bool _isAnimationBlocked;

        public UnitIcon(Unit unit, bool blockClick = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitIconStyles);
            if (ss != null) styleSheets.Add(ss);

            _unit = unit;

            AddToClassList(_ussMain);

            IconContainer = new();
            IconContainer.AddToClassList(_ussIconContainer);

            VisualElement icon = new();
            icon.style.backgroundImage = new(unit.Icon);
            IconContainer.Add(icon);

            Frame = new();
            Frame.AddToClassList(_ussFrame);

            Add(IconContainer);
            Add(Frame);

            if (blockClick) return;
            RegisterCallback<ClickEvent>(OnClick);
        }

        void OnClick(ClickEvent evt)
        {
            evt.StopImmediatePropagation();
            UnitScreen screen = UnitScreenFactory.Instance.CreateUnitScreen(_unit);
            screen?.Initialize();
        }
    }
}