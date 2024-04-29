using System.Collections.Generic;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class ShopScreen : RewardScreen
    {
        Shop _shop;

        public void InitializeShop(Shop shop)
        {
            _shop = shop;
            Initialize();
        }

        public override void Initialize()
        {
            Title = "Shop";

            base.Initialize();
            AddHeroGoldElement();
            AddContinueButton();
        }

        protected override void CreateRewardCards()
        {
            AllRewardElements.Clear();

            if (_shop.ShouldReset)
                CreateNewRewardCards();
            else
                foreach (Reward r in _shop.GetRewards())
                    ParseRewardCard(r);

            AddShopItems();
        }

        void ParseRewardCard(Reward reward)
        {
            RewardElement el = null;
            if (reward is RewardAbility abilityReward)
                el = (RewardElementAbility)new(abilityReward);
            if (reward is RewardTablet tabletReward)
                el = (RewardElementTablet)new(tabletReward);
            if (reward is RewardCreature creatureReward)
                el = (RewardElementCreature)new(creatureReward);
            if (reward is RewardGold goldReward)
                el = (RewardElementGold)new(goldReward);

            AllRewardElements.Add(el);
        }

        void CreateNewRewardCards()
        {
            for (int i = 0; i < NumberOfRewards; i++)
            {
                RewardElement el = ChooseRewardElement();
                AllRewardElements.Add(el);
            }

            List<Reward> rewards = new();
            foreach (RewardElement el in AllRewardElements)
                rewards.Add(el.Reward);
            _shop.SetRewards(rewards);
            _shop.ShouldReset = false;
        }

        void AddShopItems()
        {
            for (int i = 0; i < NumberOfRewards; i++)
            {
                bool isMystery = i == NumberOfRewards - 1;
                ShopItemElement shopItemElement = new(AllRewardElements[i], isMystery);

                shopItemElement.style.position = Position.Absolute;
                float endLeft = LeftPositions[i];
                shopItemElement.style.left = endLeft;

                shopItemElement.style.width = RewardElementWidth;
                shopItemElement.style.height = RewardElementHeight;

                RewardContainer.Add(shopItemElement);
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