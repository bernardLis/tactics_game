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
        public string Name = "Tavski";

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

        public void Initialize(int bodyType = 420)
        {
            name = "New Hero";

            BodyType = bodyType == 420 ? Random.Range(0, 2) : bodyType;

            RandomizeColors();
            InitializeOutfit();
        }

        void RandomizeColors()
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
        }

        void InitializeOutfit()
        {
            if (BodyType == 0)
            {
                HairId = "6bf69b15-42c3-4480-940d-3021bd4adee9";
                UnderwearId = "2ec8f3d5-1b58-4396-b26b-6a60f9f56ba9";
                BrassiereId = "40f7f1c6-be09-4de8-880b-9444505d0c7c";
                HelmetId = "fd2de552-99c3-4fd3-a974-ad696acf3aa1";
                TorsoId = "5fd353c0-3da2-4179-89d2-709b7db52c6b";
                LegsId = "23e75713-7897-460d-b180-44993959afa2";
            }

            if (BodyType == 1)
            {
                HairId = "cb102bfd-50ef-4750-8162-cf461ed62c4e";
                BeardId = "b7c77213-710c-4c2d-8b88-a80bb83d73c7";
                MustacheId = "da706c3c-51be-4c34-a237-9c214d806389";
                UnderwearId = "5e1187a6-8249-4547-9524-d06ca83f4771";
                HelmetId = "cb102bfd-50ef-4750-8162-cf461ed62c4e";
                TorsoId = "65bfd62f-8c71-4f27-91ca-c263b8ac863b";
                LegsId = "2f9b11ba-1ca7-47cb-bf28-c3e0f6801445";
            }
        }

        void RandomizeOutfits()
        {
            UnitDatabase database = GameManager.Instance.UnitDatabase;
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