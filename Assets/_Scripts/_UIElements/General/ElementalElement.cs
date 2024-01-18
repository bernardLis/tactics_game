


using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class ElementalElement : ElementWithTooltip
    {
        const string _ussClassName = "elemental-element__";
        const string _ussIcon = _ussClassName + "icon";

        readonly Element _element;

        readonly Label _icon;

        bool _isEffectDisabled;

        EffectHolder _effectHolderInstance;

        public ElementalElement(Element element, int size = 0)
        {
            var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ElementalElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _element = element;

            _icon = new();
            _icon.AddToClassList(_ussIcon);
            _icon.style.backgroundImage = new StyleBackground(element.Icon);
            if (size != 0)
            {
                _icon.style.width = size;
                _icon.style.height = size;
            }
            Add(_icon);
        }

        public void PlayEffect()
        {
            if (_isEffectDisabled) return;

            Vector3 pos = this.worldTransform.GetPosition();
            pos.x = pos.x + this.resolvedStyle.width / 2;
            pos.y = Camera.main.pixelHeight - pos.y - this.resolvedStyle.height; // inverted, plus play on bottom of element
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
            worldPos.z = 0;

            _effectHolderInstance = ScriptableObject.Instantiate(_element.VFXEffect);
            _effectHolderInstance.PlayEffect(worldPos, _element.VFXEffect.VisualEffectPrefab.transform.localScale);
        }

        public void DisableEffect() { _isEffectDisabled = true; }

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
