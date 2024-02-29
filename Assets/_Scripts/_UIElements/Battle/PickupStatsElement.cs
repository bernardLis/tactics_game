using Lis.Core;
using UnityEngine.UIElements;


namespace Lis
{
    public class PickupStatsElement : ElementWithTooltip
    {
        readonly BattleStats _stats;

        public PickupStatsElement(BattleStats stats)
        {
            GameManager gameManager = GameManager.Instance;
            GameDatabase gameDatabase = gameManager.GameDatabase;

            _stats = stats;

            style.flexDirection = FlexDirection.Row;

            Label icon = new();
            icon.style.width = 25;
            icon.style.height = 25;
            icon.style.backgroundImage = new(gameDatabase.VaseIcon);
            Add(icon);

            Label count = new(stats.VasesBroken.ToString());
            Add(count);
        }

        protected override void DisplayTooltip()
        {
            VisualElement tt = new();
            CreateTooltip(tt);
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }

        void CreateTooltip(VisualElement container)
        {
            Label title = new();
            title.text = "Pickups collected: ";
            container.Add(title);

            Label coins = new();
            coins.text = "Coins: " + _stats.CoinsCollected;
            container.Add(coins);

            Label hammers = new();
            hammers.text = "Hammers: " + _stats.HammersCollected;
            container.Add(hammers);

            Label horseshoes = new();
            horseshoes.text = "Horseshoes: " + _stats.HorseshoesCollected;
            container.Add(horseshoes);

            Label bags = new();
            bags.text = "Bags: " + _stats.BagsCollected;
            container.Add(bags);

            Label skulls = new();
            skulls.text = "Skulls: " + _stats.SkullsCollected;
            container.Add(skulls);

            Label friendBalls = new();
            friendBalls.text = "Friend balls: " + _stats.FriendBallsCollected;
            container.Add(friendBalls);
        }
    }
}