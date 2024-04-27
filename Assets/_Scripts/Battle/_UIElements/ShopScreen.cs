using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class ShopScreen : RewardScreen
    {
        readonly List<ShopItemElement> _allShopItemElements = new();

        public ShopScreen()
        {
            AddHeroGoldElement();
            ChangeTitle();
            AddContinueButton();
        }

        void AddHeroGoldElement()
        {
            GoldElement goldElement = new(GameManager.Gold);
            GameManager.OnGoldChanged += goldElement.ChangeAmount;
            goldElement.style.position = Position.Absolute;
            goldElement.style.left = Length.Percent(10);
            goldElement.style.top = Length.Percent(3);
            Content.Add(goldElement);
        }

        void ChangeTitle()
        {
            Title.text = "Shop";
        }

        protected override void CreateRewardCards()
        {
            _allShopItemElements.Clear();
            for (int i = 0; i < NumberOfRewards; i++)
            {
                RewardElement el = ChooseRewardElement();
                el ??= ChooseRewardElement(); // backup

                bool isMystery = i == NumberOfRewards - 1;
                ShopItemElement shopItemElement = new(el, isMystery);

                shopItemElement.style.position = Position.Absolute;
                float endLeft = LeftPositions[i];
                shopItemElement.style.left = endLeft;

                shopItemElement.style.width = RewardElementWidth;
                shopItemElement.style.height = RewardElementHeight;

                RewardContainer.Add(shopItemElement);
                AllRewardElements.Add(el);
                _allShopItemElements.Add(shopItemElement);
            }
        }

        protected override RewardElement ChooseRewardElement()
        {
            int v = Random.Range(0, 3);
            if (v == 0) return CreateRewardCardAbility();
            if (v == 1) return CreateRewardTablet();
            return CreateRewardCardCreature();
        }

        protected override void RewardSelected(Reward reward)
        {
            // leave empty
        }
    }
}