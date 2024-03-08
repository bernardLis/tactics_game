using UnityEngine.UIElements;

namespace Lis.Core
{
    public class NatureElement : ElementWithTooltip
    {
        const string _ussClassName = "nature-element__";
        const string _ussIcon = _ussClassName + "icon";

        readonly Nature _nature;

        readonly Label _icon;

        public NatureElement(Nature nature, int size = 0)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.NatureElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _nature = nature;

            _icon = new();
            _icon.AddToClassList(_ussIcon);
            _icon.style.backgroundImage = new(nature.Icon);
            if (size != 0)
            {
                _icon.style.width = size;
                _icon.style.height = size;
            }

            Add(_icon);
        }

        protected override void DisplayTooltip()
        {
            VisualElement container = new();
            container.Add(new Label(_nature.NatureName.ToString()));
            container.Add(new Label(_nature.Description));
            container.Add(new Label("Strong against: " + _nature.StrongAgainst.NatureName.ToString()));
            container.Add(new Label("Weak against: " + _nature.WeakAgainst.NatureName.ToString()));

            _tooltip = new(this, container);
            base.DisplayTooltip();
        }
    }
}