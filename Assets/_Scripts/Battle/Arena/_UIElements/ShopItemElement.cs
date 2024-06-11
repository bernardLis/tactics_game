﻿using Lis.Core;
using Lis.Units.Hero.Rewards;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class ShopItemElement : VisualElement
    {
        private const string _ussCommonButton = "common__button";
        private readonly bool _isMystery;

        private readonly int _price;

        private readonly PurchaseButton _purchaseButton;
        private readonly RewardElement _rewardElement;
        private Label _mysteryElement;

        public ShopItemElement(RewardElement rewardElement, bool isMystery)
        {
            style.alignItems = Align.Center;

            _isMystery = isMystery;
            if (_isMystery) rewardElement.SetMystery();
            _rewardElement = rewardElement;
            _price = rewardElement.Reward.Price;

            rewardElement.style.flexGrow = 1;
            rewardElement.style.width = Length.Percent(100);
            rewardElement.IsShop();
            Add(rewardElement);

            _purchaseButton = new("", _ussCommonButton, Purchase, _price);
            _purchaseButton.style.position = Position.Absolute;
            _purchaseButton.style.bottom = Length.Percent(0);

            rewardElement.Add(_purchaseButton);
        }

        private void Purchase()
        {
            _purchaseButton.SetEnabled(false);
            if (_isMystery) _rewardElement.RevealMystery();
            _rewardElement.DisableCard();
            _rewardElement.Reward.GetReward();
        }
    }
}