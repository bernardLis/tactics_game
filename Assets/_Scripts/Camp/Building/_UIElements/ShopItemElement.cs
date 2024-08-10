using Lis.Core;
using Lis.Units.Hero.Rewards;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class ShopItemElement : VisualElement
    {
        const string _ussCommonButton = "common__button";
        readonly bool _isMystery;

        readonly PurchaseButton _purchaseButton;
        readonly RewardElement _rewardElement;

        public ShopItemElement(RewardElement rewardElement)
        {
            style.alignItems = Align.Center;

            _rewardElement = rewardElement;
            int price = rewardElement.Reward.Price;

            rewardElement.style.flexGrow = 1;
            rewardElement.style.width = Length.Percent(100);
            rewardElement.IsShop();
            Add(rewardElement);

            _purchaseButton = new("", _ussCommonButton, Purchase, price);
            _purchaseButton.style.position = Position.Absolute;
            _purchaseButton.style.bottom = Length.Percent(0);
            rewardElement.Add(_purchaseButton);

            if (_rewardElement.Reward.IsUnavailable)
            {
                _rewardElement.DisableCard();
                _purchaseButton.SetEnabled(false);
            }
        }

        void Purchase()
        {
            _purchaseButton.SetEnabled(false);
            _rewardElement.DisableCard();
            _rewardElement.Reward.GetReward();
            _rewardElement.Reward.IsUnavailable = true;
        }
    }
}