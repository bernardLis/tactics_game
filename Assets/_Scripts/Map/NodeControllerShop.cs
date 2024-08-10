using Lis.Camp.Building;
using Lis.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lis.Map
{
    public class NodeControllerShop : NodeController
    {
        MyButton _showShopButton;

        protected override void ResolveNode()
        {
            base.ResolveNode();
            _showShopButton = new("Shop", "common__button", ShowShop);

            MapManager.ButtonContainer.Insert(0, _showShopButton);
            if (!Node.IsVisited) ShowShop();
        }

        void ShowShop()
        {
            ShopScreen shop = new();
            shop.InitializeShop((MapNodeShop)Node);
        }

        public override void LeaveNode()
        {
            base.LeaveNode();
            _showShopButton.RemoveFromHierarchy();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (this == PlayerController.CurrentNode)
            {
                Debug.Log("showing shop on click");
                ShowShop();
            }
        }
    }
}