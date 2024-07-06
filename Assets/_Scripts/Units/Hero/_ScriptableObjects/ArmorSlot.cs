using Lis.Core;
using Lis.Units.Hero.Items;

namespace Lis.Units.Hero
{
    public class ArmorSlot : BaseScriptableObject
    {
        public ItemType ItemType;
        public Armor CurrentItem;
        public Armor PreviousItem;
    }
}