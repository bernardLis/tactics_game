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
            if (HeroManager.RewardRerollsAvailable <= 0)
            {
                Helpers.DisplayTextOnElement(FightManager.Root, RerollButton, "Not More Rerolls!", Color.red);
                return;
            }

            HeroManager.RewardRerollsAvailable--;
            RerollsLeft.text = $"Rerolls left: {HeroManager.RewardRerollsAvailable}";
            AudioManager.CreateSound().WithSound(AudioManager.GetSound("Dice Roll")).Play();
            CreateRewardCards();

            if (HeroManager.RewardRerollsAvailable <= 0)
                RerollButton.SetEnabled(false);
        }


        protected override void CreateRewardCards()
        {
            AllRewardElements.Clear();
            RewardContainer.Clear();

            ParseRewardCards(_shop.GetRewards());

            AddShopItems();
        }

        void ParseRewardCards(List<Reward> cards)
        {
            foreach (Reward r in cards)
            {
                RewardElement el = null;
                if (r is RewardAbility abilityReward)
                    el = (RewardElementAbility)new(abilityReward);
                if (r is RewardTablet tabletReward)
                    el = (RewardElementTablet)new(tabletReward);
                if (r is RewardCreature creatureReward)
                    el = (RewardElementCreature)new(creatureReward);
                if (r is RewardPawn pawnReward)
                    el = (RewardElementPawn)new(pawnReward);
                if (r is RewardGold goldReward)
                    el = (RewardElementGold)new(goldReward);

                AllRewardElements.Add(el);
            }
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
            int v = Random.Range(0, 4);
            if (v == 0) return CreateRewardCardAbility();
            if (v == 1) return CreateRewardTablet();
            if (v == 2) return CreateRewardCardPawn();
            return CreateRewardCardCreature();
        }

        protected override void RewardSelected(Reward reward)
        {
            // leave empty
        }
    }
}