using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class ShopItemElement : VisualElement
    {
        readonly MyButton _purchaseButton;
        readonly RewardElement _rewardElement;

        int _price = 200;

        public ShopItemElement(RewardElement rewardElement)
        {
            _rewardElement = rewardElement;

            rewardElement.style.flexGrow = 1;
            rewardElement.style.width = Length.Percent(100);
            rewardElement.IsShop();

            Add(rewardElement);

            _purchaseButton = new("", null, Purchase);
            GoldElement goldElement = new(_price);
            _purchaseButton.Add(goldElement);

            Add(_purchaseButton);
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
            _rewardElement.Reward.GetReward();
        }
    }
}