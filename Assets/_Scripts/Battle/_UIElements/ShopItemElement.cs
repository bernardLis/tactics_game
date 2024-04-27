using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class ShopItemElement : VisualElement
    {
        const string _ussCommonButton = "common__button";

        readonly MyButton _purchaseButton;
        readonly RewardElement _rewardElement;

        readonly int _price;
        readonly bool _isMystery;
        Label _mysteryElement;

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

            _purchaseButton = new("", _ussCommonButton, Purchase);
            _purchaseButton.style.position = Position.Absolute;
            _purchaseButton.style.bottom = Length.Percent(0);
            _purchaseButton.style.width = 100;
            _purchaseButton.style.minHeight = 60;
            _purchaseButton.style.paddingLeft = 12;

            GoldElement goldElement = new(_price);
            _purchaseButton.Add(goldElement);

            rewardElement.Add(_purchaseButton);
        }

        void Purchase()
        {
            if (GameManager.Instance.Gold < _price)
            {
                Helpers.DisplayTextOnElement(BattleManager.Instance.Root, _purchaseButton, "Not enough gold!",
                    Color.red);
                return;
            }

            GameManager.Instance.ChangeGoldValue(-_price);
            _purchaseButton.SetEnabled(false);
            if (_isMystery) _rewardElement.RevealMystery();
            _rewardElement.DisableCard();
            _rewardElement.Reward.GetReward();
        }
    }
}