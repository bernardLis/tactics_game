using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero.Items
{
    public class Item : BaseScriptableObject
    {
        public ItemType ItemType;
        public ArmorType ArmorType;

        public SkinnedMeshRenderer ItemMeshRenderer;

        public bool DisableHair;
        public bool DisableBeard;
        public bool DisableMustache;

    }

    public enum ItemType
    {
        Helmet,
        Shoulders,
        Torso,
        Waist,
        Legs,
        Hair,
        Beard,
        Mustache,
        Underwear,
        Brassiere,
        None
    }

    public enum ArmorType
    {
        Cloth,
        Leather,
        Plate,
        Outfit,
        None
    }
}