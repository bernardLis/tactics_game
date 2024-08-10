using System.Collections.Generic;
using Lis.Core.Utilities;
using Lis.Map;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class ShopScreen : RewardScreen
    {
        MapNodeShop _shop;

        public void InitializeShop(MapNodeShop shop)
        {
            _shop = shop;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            AddContinueButton();
            SetTitle("Shop");
        }

        protected override void RerollReward()
        {
            if (Hero.RewardRerolls <= 0)
            {
                Helpers.DisplayTextOnElement(FightManager.Root, RerollButton, "Not More Rerolls!", Color.red);
                return;
            }

            Hero.RewardRerolls--;
            RerollsLeft.text = $"Rerolls left: {Hero.RewardRerolls}";
            AudioManager.CreateSound().WithSound(AudioManager.GetSound("Dice Roll")).Play();
            _shop.SelectItems();
            CreateRewardCards();

            if (Hero.RewardRerolls <= 0)
                RerollButton.SetEnabled(false);
        }

        protected override void CreateRewardCards()
        {
            Debug.Log("CreateRewardCards");

            AllRewardElements.Clear();
            RewardContainer.Clear();

            ParseRewardCards(_shop.GetRewards());

            AddShopItems();
        }

        void ParseRewardCards(List<Reward> rewards)
        {
            foreach (Reward r in rewards)
            {
                RewardElement el = null;
                if (r is RewardAbility abilityReward)
                    el = (RewardElementAbility)new(abilityReward);
                if (r is RewardPawn pawnReward)
                    el = (RewardElementPawn)new(pawnReward);
                if (r is RewardGold goldReward)
                    el = (RewardElementGold)new(goldReward);
                if (r is RewardArmor armorReward)
                    el = (RewardElementArmor)new(armorReward);

                AllRewardElements.Add(el);
            }
        }

        void AddShopItems()
        {
            for (int i = 0; i < NumberOfRewards; i++)
            {
                ShopItemElement shopItemElement = new(AllRewardElements[i]);

                shopItemElement.style.position = Position.Absolute;
                float endLeft = LeftPositions[i];
                shopItemElement.style.left = endLeft;

                shopItemElement.style.width = RewardElementWidth;
                shopItemElement.style.height = RewardElementHeight;

                RewardContainer.Add(shopItemElement);
            }
        }

        protected override void RewardSelected(Reward reward)
        {
            // leave empty
        }
    }
}