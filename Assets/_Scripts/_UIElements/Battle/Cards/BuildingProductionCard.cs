


using Lis.Upgrades;
using UnityEngine.UIElements;

namespace Lis
{
    public class BuildingProductionCard : BuildingCard
    {
        BuildingProduction _buildingProduction;

        public BuildingProductionCard(Building building) : base(building)
        {
        }

        protected override void PopulateCard()
        {
            _buildingProduction = Building as BuildingProduction;

            base.PopulateCard();
            HandleLevelLabel();
        }

        protected override void HandleIcon()
        {
            EntityIcon entityIcon = new(_buildingProduction.ProducedCreature);
            _topLeftContainer.Add(entityIcon);
        }

        void HandleLevelLabel()
        {
            LevelLabel = new();
            LevelLabel.text = $"Level {_buildingProduction.BuildingUpgrade.CurrentLevel + 1}";
            _topRightContainer.Add(LevelLabel);

        }

        protected override void HandleBuildingInfoContainer()
        {
            InfoContainer = new();
            _middleContainer.Add(InfoContainer);

            UpgradeLevelLair currentUpgrade = _buildingProduction.GetCurrentUpgrade();

            Label limitLabel = new($"Max: {currentUpgrade.ProductionLimit}");
            InfoContainer.Add(limitLabel);

            Label delayLabel = new($"Respawn: {currentUpgrade.ProductionDelay}s");
            InfoContainer.Add(delayLabel);
        }
    }
}
