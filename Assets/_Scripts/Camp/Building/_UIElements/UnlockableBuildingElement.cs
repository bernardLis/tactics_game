using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class UnlockableBuildingElement : VisualElement
    {
        const string _ussCommonButton = "common__button";
        const string _ussClassName = "unlockable-building-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIcon = _ussClassName + "icon";

        readonly Building _building;
        readonly PurchaseButton _purchaseButton;

        public UnlockableBuildingElement(Building building)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnlockableBuildingElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            _building = building;

            Label label = new();
            label.text = Helpers.ParseScriptableObjectName(building.name);
            Add(label);

            VisualElement icon = new();
            icon.AddToClassList(_ussIcon);
            icon.style.backgroundImage = new(building.Icon);
            Add(icon);

            _purchaseButton = new("", _ussCommonButton, Purchase, _building.UnlockCost);
            Add(_purchaseButton);
        }

        void Purchase()
        {
            _building.Unlock();
            _purchaseButton.RemoveFromHierarchy();
        }
    }
}