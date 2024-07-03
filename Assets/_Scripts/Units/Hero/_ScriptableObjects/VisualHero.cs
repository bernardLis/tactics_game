using System;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.HeroCreation
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Hero/Visual Hero")]
    public class VisualHero : BaseScriptableObject
    {
        public int TimesPicked;
        public string Name;

        public int BodyType;

        // all item ids
        public string HairId;
        public string BeardId;
        public string MustacheId;
        public string UnderwearId;
        public string BrassiereId;
        public string HelmetId;
        public string TorsoId;
        public string LegsId;

        // all colors
        public Color SkinColor;
        public Color EyeColor;
        public Color EyebrowColor;

        public Color HairMainColor;
        public Color HairDetailColor;

        public Color UnderwearMainColor;
        public Color UnderwearDetailColor;

        public Color OutfitMainColor;
        public Color OutfitDetailColor;
        public Color OutfitDetailSecondaryColor;

        public void Initialize()
        {
            name = "New Hero";
            BodyType = Random.Range(0, 2);

            Randomize();
        }

        void Randomize()
        {
            UnitDatabase database = GameManager.Instance.UnitDatabase;

            SkinColor = database.GetRandomHeroCustomizationColor();
            EyeColor = database.GetRandomHeroCustomizationColor();
            EyebrowColor = database.GetRandomHeroCustomizationColor();

            HairMainColor = database.GetRandomHeroCustomizationColor();
            HairDetailColor = database.GetRandomHeroCustomizationColor();

            UnderwearMainColor = database.GetRandomHeroCustomizationColor();
            UnderwearDetailColor = database.GetRandomHeroCustomizationColor();

            OutfitMainColor = database.GetRandomHeroCustomizationColor();
            OutfitDetailColor = database.GetRandomHeroCustomizationColor();
            OutfitDetailSecondaryColor = database.GetRandomHeroCustomizationColor();

            if (BodyType == 0)
            {
                HairId = database.GetRandomFemaleItemByType(ItemType.Hair).Id;
                UnderwearId = database.GetRandomFemaleItemByType(ItemType.Underwear).Id;
                BrassiereId = database.GetRandomFemaleItemByType(ItemType.Brassiere).Id;
                HelmetId = database.GetRandomFemaleItemByType(ItemType.Helmet).Id;
                TorsoId = database.GetRandomFemaleItemByType(ItemType.Torso).Id;
                LegsId = database.GetRandomFemaleItemByType(ItemType.Legs).Id;
            }

            if (BodyType == 1)
            {
                HairId = database.GetRandomMaleItemByType(ItemType.Hair).Id;
                BeardId = database.GetRandomMaleItemByType(ItemType.Beard).Id;
                MustacheId = database.GetRandomMaleItemByType(ItemType.Mustache).Id;
                UnderwearId = database.GetRandomMaleItemByType(ItemType.Underwear).Id;
                HelmetId = database.GetRandomMaleItemByType(ItemType.Helmet).Id;
                TorsoId = database.GetRandomMaleItemByType(ItemType.Torso).Id;
                LegsId = database.GetRandomMaleItemByType(ItemType.Legs).Id;
            }
        }

        public void SetItem(ItemType type, string id)
        {
            Debug.Log($"Setting item {type} to {id}");
            // it's a bit shitty, but it'll do.
            switch (type)
            {
                case ItemType.Hair:
                    HairId = id;
                    break;
                case ItemType.Beard:
                    BeardId = id;
                    break;
                case ItemType.Mustache:
                    MustacheId = id;
                    break;
                case ItemType.Underwear:
                    UnderwearId = id;
                    break;
                case ItemType.Brassiere:
                    BrassiereId = id;
                    break;
                case ItemType.Helmet:
                    HelmetId = id;
                    break;
                case ItemType.Torso:
                    TorsoId = id;
                    break;
                case ItemType.Legs:
                    LegsId = id;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        public VisualHeroData SerializeSelf()
        {
            return new VisualHeroData
            {
                Id = Id,
                TimesPicked = TimesPicked,
                Name = Name,
                BodyType = BodyType,
                HairId = HairId,
                BeardId = BeardId,
                MustacheId = MustacheId,
                UnderwearId = UnderwearId,
                BrassiereId = BrassiereId,
                HelmetId = HelmetId,
                TorsoId = TorsoId,
                LegsId = LegsId,
                SkinColor = new ColorSaver(SkinColor),
                EyeColor = new ColorSaver(EyeColor),
                EyebrowColor = new ColorSaver(EyebrowColor),
                HairMainColor = new ColorSaver(HairMainColor),
                HairDetailColor = new ColorSaver(HairDetailColor),
                UnderwearMainColor = new ColorSaver(UnderwearMainColor),
                UnderwearDetailColor = new ColorSaver(UnderwearDetailColor),
                OutfitMainColor = new ColorSaver(OutfitMainColor),
                OutfitDetailColor = new ColorSaver(OutfitDetailColor),
                OutfitDetailSecondaryColor = new ColorSaver(OutfitDetailSecondaryColor)
            };
        }

        public void LoadFromData(VisualHeroData data)
        {
            Id = data.Id;
            TimesPicked = data.TimesPicked;
            Name = data.Name;
            BodyType = data.BodyType;
            HairId = data.HairId;
            BeardId = data.BeardId;
            MustacheId = data.MustacheId;
            UnderwearId = data.UnderwearId;
            BrassiereId = data.BrassiereId;
            HelmetId = data.HelmetId;
            TorsoId = data.TorsoId;
            LegsId = data.LegsId;
            SkinColor = data.SkinColor.Color();
            EyeColor = data.EyeColor.Color();
            EyebrowColor = data.EyebrowColor.Color();
            HairMainColor = data.HairMainColor.Color();
            HairDetailColor = data.HairDetailColor.Color();
            UnderwearMainColor = data.UnderwearMainColor.Color();
            UnderwearDetailColor = data.UnderwearDetailColor.Color();
            OutfitMainColor = data.OutfitMainColor.Color();
            OutfitDetailColor = data.OutfitDetailColor.Color();
            OutfitDetailSecondaryColor = data.OutfitDetailSecondaryColor.Color();
        }
    }

    [Serializable]
    public struct VisualHeroData
    {
        public string Id;
        public int TimesPicked;
        public string Name;

        public int BodyType;

        // all item ids
        public string HairId;
        public string BeardId;
        public string MustacheId;
        public string UnderwearId;
        public string BrassiereId;
        public string HelmetId;
        public string TorsoId;
        public string LegsId;

        // all colors
        public ColorSaver SkinColor;
        public ColorSaver EyeColor;
        public ColorSaver EyebrowColor;

        public ColorSaver HairMainColor;
        public ColorSaver HairDetailColor;

        public ColorSaver UnderwearMainColor;
        public ColorSaver UnderwearDetailColor;

        public ColorSaver OutfitMainColor;
        public ColorSaver OutfitDetailColor;
        public ColorSaver OutfitDetailSecondaryColor;
    }
}

[Serializable]
public class ColorSaver
{
    public float r, g, b, a;

    public ColorSaver(Color color)
    {
        r = color.r;
        g = color.g;
        b = color.b;
        a = color.a;
    }

    public Color Color()
    {
        return new(r, g, b, a);
    }
}