using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ElementalElement : ElementWithTooltip
    {
        const string _ussClassName = "elemental-element__";
        const string _ussIcon = _ussClassName + "icon";

        readonly Element _element;

        readonly Label _icon;

        public ElementalElement(Element element, int size = 0)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.ElementalElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _element = element;

            _icon = new();
            _icon.AddToClassList(_ussIcon);
            _icon.style.backgroundImage = new(element.Icon);
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
            container.Add(new Label(_element.ElementName.ToString()));
            container.Add(new Label(_element.Description));
            container.Add(new Label("Strong against: " + _element.StrongAgainst.ElementName.ToString()));
            container.Add(new Label("Weak against: " + _element.WeakAgainst.ElementName.ToString()));

            _tooltip = new(this, container);
            base.DisplayTooltip();
        }
    }
}