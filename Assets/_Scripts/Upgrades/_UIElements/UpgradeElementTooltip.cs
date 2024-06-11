using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Upgrades
{
    public class UpgradeElementTooltip : VisualElement
    {
        readonly Upgrade _upgrade;

        public UpgradeElementTooltip(Upgrade upgrade)
        {
            _upgrade = upgrade;
            _upgrade.OnLevelChanged += HandleTooltip;

            HandleTooltip();
        }

        void HandleTooltip()
        {
            Clear();
            if (_upgrade.Type == UpgradeType.Building)
            {
                CreateBuildingTooltip();
                return;
            }

            CreateTooltip();
        }

        void CreateTooltip()
        {
            Add(new Label(_upgrade.Description));
            Add(new HorizontalSpacerElement());

            Add(new Label("Current:"));
            if (_upgrade.CurrentLevel == -1)
                Add(new Label("Not unlocked"));
            else
                Add(new Label(_upgrade.GetCurrentLevel().Description));

            Add(new HorizontalSpacerElement());

            Add(new Label("Next:"));
            if (_upgrade.IsMaxLevel())
                Add(new Label("Max level reached"));
            else
                Add(new Label(_upgrade.GetNextLevel().Description));
        }

        void CreateBuildingTooltip()
        {
            Add(new Label(_upgrade.name));
            Add(new HorizontalSpacerElement());

            Add(new Label("Current:"));
            if (_upgrade.CurrentLevel == -1)
            {
                Add(new Label("Not unlocked"));
            }
            else
            {
                UpgradeLevelLair level = (UpgradeLevelLair)_upgrade.GetCurrentLevel();
                Add(new Label($"Max creatures: {level.ProductionLimit}"));
                Add(new Label($"Production delay: {level.ProductionDelay}"));
            }

            Add(new HorizontalSpacerElement());

            Add(new Label("Next:"));
            if (_upgrade.IsMaxLevel())
            {
                Add(new Label("Max level reached"));
            }
            else
            {
                UpgradeLevelLair level = (UpgradeLevelLair)_upgrade.GetNextLevel();
                Add(new Label($"Max creatures: {level.ProductionLimit}"));
                Add(new Label($"Production delay: {level.ProductionDelay}"));
            }
        }
    }
}