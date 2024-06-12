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

        readonly Unit _unit;

        protected readonly VisualElement Icon;
        bool _isAnimationBlocked;

        public UnitIcon(Unit unit, bool blockClick = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitIconStyles);
            if (ss != null) styleSheets.Add(ss);

            _unit = unit;

            AddToClassList(_ussMain);

            Icon = new();
            Icon.AddToClassList(_ussIconContainer);
            Icon.style.backgroundImage = new(unit.Icon);
            Add(Icon);

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