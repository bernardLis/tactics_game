using System.Collections.Generic;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Tablets
{
    public class TabletElement : ElementWithTooltip
    {
        const string _ussCommonTextPrimary = "common__text-primary";
        const string _ussCommonButtonBasic = "common__button-basic";

        const string _ussClassName = "tablet-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussDotContainer = _ussClassName + "dot-container";
        const string _ussLevelDotEmpty = _ussClassName + "level-dot-empty";
        const string _ussLevelDotFull = _ussClassName + "level-dot-full";

        readonly Tablet _tablet;
        
        public TabletElement(Tablet tablet, bool showLevel = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TabletElementStyles);
            if (ss != null) styleSheets.Add(ss);

            _tablet = tablet;

            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);
            AddToClassList(_ussCommonButtonBasic);

            style.backgroundImage = tablet.Icon.texture;

            if (showLevel) AddLevelUpDots();
        }
        
        void AddLevelUpDots()
        {
            VisualElement dotContainer = new();
            dotContainer.AddToClassList(_ussDotContainer);
            Add(dotContainer);
            List<VisualElement> dots = new();
            for (int i = 0; i < _tablet.MaxLevel; i++)
            {
                VisualElement dot = new();
                dot.AddToClassList(_ussLevelDotEmpty);
                dots.Add(dot);
                dotContainer.Add(dot);
            }

            for (int i = 0; i < _tablet.Level.Value; i++)
                dots[i].AddToClassList(_ussLevelDotFull);

            _tablet.OnLevelUp += (t) =>
            {
                for (int i = 0; i < _tablet.Level.Value; i++)
                    dots[i].AddToClassList(_ussLevelDotFull);
            };
        }

        protected override void DisplayTooltip()
        {
            TabletTooltipElement tt = new(_tablet);
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }
    }
}