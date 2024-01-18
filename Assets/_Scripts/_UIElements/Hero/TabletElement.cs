using System.Collections.Generic;



using UnityEngine.UIElements;

namespace Lis
{
    public class TabletElement : ElementWithTooltip
    {
        const string _ussCommonTextPrimary = "common__text-primary";
        const string _ussCommonButtonBasic = "common__button-basic";

        const string _ussClassName = "tablet-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIcon = _ussClassName + "icon";
        const string _ussDotContainer = _ussClassName + "dot-container";
        const string _ussLevelDotEmpty = _ussClassName + "level-dot-empty";
        const string _ussLevelDotFull = _ussClassName + "level-dot-full";
    
        public Tablet Tablet;

        public TabletElement(Tablet tablet, bool showLevel = false) : base()
        {
            var ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TabletElementStyles);
            if (ss != null) styleSheets.Add(ss);

            Tablet = tablet;

            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);
            AddToClassList(_ussCommonButtonBasic);

            VisualElement icon = new();
            icon.AddToClassList(_ussIcon);
            icon.style.backgroundImage = tablet.Icon.texture;
            Add(icon);

            if (showLevel) AddLevelUpDots();
        }

        void AddLevelUpDots()
        {
            VisualElement dotContainer = new();
            dotContainer.AddToClassList(_ussDotContainer);
            Add(dotContainer);
            List<VisualElement> dots = new();
            for (int i = 0; i < Tablet.MaxLevel; i++)
            {
                VisualElement dot = new();
                dot.AddToClassList(_ussLevelDotEmpty);
                dots.Add(dot);
                dotContainer.Add(dot);
            }

            for (int i = 0; i < Tablet.Level.Value; i++)
                dots[i].AddToClassList(_ussLevelDotFull);

            Tablet.OnLevelUp += () =>
            {
                for (int i = 0; i < Tablet.Level.Value; i++)
                    dots[i].AddToClassList(_ussLevelDotFull);
            };
        }

        protected override void DisplayTooltip()
        {
            TabletTooltipElement tt = new(Tablet);
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }
    }
}