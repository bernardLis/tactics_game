using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis
{
    public class BuildingCard : TooltipCard
    {
        const string _ussClassName = "building-card__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIcon = _ussClassName + "icon";

        Label _nameLabel;
        protected Label LevelLabel;
        protected VisualElement InfoContainer;
        protected readonly Building Building;

        public BuildingCard(Building building)
        {
            Initialize();

            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.BuildingCardStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            Building = building;
            PopulateCard();
        }

        protected virtual void PopulateCard()
        {
            HandleIcon();
            HandleNameLabel();
            HandleBuildingInfoContainer();
        }

        protected virtual void HandleIcon()
        {
            VisualElement icon = new();
            icon.AddToClassList(_ussIcon);
            if (Building.Icon != null)
                icon.style.backgroundImage = Building.Icon.texture;

            _topLeftContainer.Add(icon);
        }

        protected virtual void HandleNameLabel()
        {
            _nameLabel = new(Helpers.ParseScriptableObjectName(Building.name));
            _nameLabel.AddToClassList(_ussName);
            _topRightContainer.Add(_nameLabel);
        }

        protected virtual void HandleBuildingInfoContainer()
        {
        }
    }
}