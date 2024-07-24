using Lis.Arena;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Items
{
    public class ArmorComparisonTooltipElement : VisualElement
    {
        Armor _newArmor;

        public ArmorComparisonTooltipElement(Armor armor)
        {
            _newArmor = armor;
            style.flexDirection = FlexDirection.Row;

            AddCurrentArmorContainer();
            Add(new VerticalSpacerElement());
            AddNewArmorContainer();
        }

        void AddNewArmorContainer()
        {
            VisualElement newContainer = new();
            Add(newContainer);

            newContainer.Add(new Label("New:"));
            newContainer.Add(new ArmorElement(_newArmor, true));
        }

        void AddCurrentArmorContainer()
        {
            VisualElement currentContainer = new();
            Add(currentContainer);

            currentContainer.Add(new Label("Current: "));
            if (HeroManager.Instance == null)
            {
                Remove(currentContainer);
                return;
            }

            Armor currentArmor = HeroManager.Instance.Hero.GetArmorAtSlot(_newArmor.ItemType);
            if (currentArmor == null)
            {
                currentContainer.Add(new Label("None"));
                return;
            }

            currentContainer.Add(new ArmorElement(currentArmor, true));
        }
    }
}