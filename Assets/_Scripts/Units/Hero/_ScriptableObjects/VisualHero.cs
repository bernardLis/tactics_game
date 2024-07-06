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
        public Item Hair;
        public Item Beard;
        public Item Mustache;
        public Item Underwear;
        public Item Brassiere;
        public Item Helmet;
        public Item Torso;
        public Item Legs;

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

        public event Action<Item> OnItemChanged;

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
            UnitDatabase database = GameManager.Instance.UnitDatabase;

            if (BodyType == 0)
            {
                Hair = database.GetFemaleOutfitById("6bf69b15-42c3-4480-940d-3021bd4adee9");
                Underwear = database.GetFemaleOutfitById("2ec8f3d5-1b58-4396-b26b-6a60f9f56ba9");
                Brassiere = database.GetFemaleOutfitById("40f7f1c6-be09-4de8-880b-9444505d0c7c");
                Helmet = database.GetFemaleOutfitById("fd2de552-99c3-4fd3-a974-ad696acf3aa1");
                Torso = database.GetFemaleOutfitById("5fd353c0-3da2-4179-89d2-709b7db52c6b");
                Legs = database.GetFemaleOutfitById("23e75713-7897-460d-b180-44993959afa2");
            }

            if (BodyType == 1)
            {
                Hair = database.GetMaleOutfitById("cb102bfd-50ef-4750-8162-cf461ed62c4e");
                Beard = database.GetMaleOutfitById("b7c77213-710c-4c2d-8b88-a80bb83d73c7");
                Mustache = database.GetMaleOutfitById("da706c3c-51be-4c34-a237-9c214d806389");
                Underwear = database.GetMaleOutfitById("5e1187a6-8249-4547-9524-d06ca83f4771");
                Helmet = database.GetMaleOutfitById("cb102bfd-50ef-4750-8162-cf461ed62c4e");
                Torso = database.GetMaleOutfitById("65bfd62f-8c71-4f27-91ca-c263b8ac863b");
                Legs = database.GetMaleOutfitById("2f9b11ba-1ca7-47cb-bf28-c3e0f6801445");
            }
        }

        public void RandomizeOutfit()
        {
            UnitDatabase database = GameManager.Instance.UnitDatabase;
            if (BodyType == 0)
            {
                Hair = database.GetRandomFemaleItemByType(ItemType.Hair);
                Underwear = database.GetRandomFemaleItemByType(ItemType.Underwear);
                Brassiere = database.GetRandomFemaleItemByType(ItemType.Brassiere);
                Helmet = database.GetRandomFemaleItemByType(ItemType.Helmet);
                Torso = database.GetRandomFemaleItemByType(ItemType.Torso);
                Legs = database.GetRandomFemaleItemByType(ItemType.Legs);
            }

            if (BodyType == 1)
            {
                Hair = database.GetRandomMaleItemByType(ItemType.Hair);
                Beard = database.GetRandomMaleItemByType(ItemType.Beard);
                Mustache = database.GetRandomMaleItemByType(ItemType.Mustache);
                Underwear = database.GetRandomMaleItemByType(ItemType.Underwear);
                Helmet = database.GetRandomMaleItemByType(ItemType.Helmet);
                Torso = database.GetRandomMaleItemByType(ItemType.Torso);
                Legs = database.GetRandomMaleItemByType(ItemType.Legs);
            }
        }

        public void SetItem(Item item)
        {
            // it's a bit shitty, but it'll do.
            switch (item.ItemType)
            {
                case ItemType.Hair:
                    Hair = item;
                    break;
                case ItemType.Beard:
                    Beard = item;
                    break;
                case ItemType.Mustache:
                    Mustache = item;
                    break;
                case ItemType.Underwear:
                    Underwear = item;
                    break;
                case ItemType.Brassiere:
                    Brassiere = item;
                    break;
                case ItemType.Helmet:
                    Helmet = item;
                    break;
                case ItemType.Torso:
                    Torso = item;
                    break;
                case ItemType.Legs:
                    Legs = item;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnItemChanged?.Invoke(item);
        }


        public VisualHeroData SerializeSelf()
        {
            string braId = "";
            string beardId = "";
            string mustacheId = "";
            if (BodyType == 0) braId = Brassiere.Id;
            if (BodyType == 1) beardId = Beard.Id;
            if (BodyType == 1) mustacheId = Mustache.Id;


            return new VisualHeroData
            {
                Id = Id,
                TimesPicked = TimesPicked,
                Name = Name,
                BodyType = BodyType,
                HairId = Hair.Id,
                BeardId = beardId,
                MustacheId = mustacheId,
                UnderwearId = Underwear.Id,
                BrassiereId = braId,
                HelmetId = Helmet.Id,
                TorsoId = Torso.Id,
                LegsId = Legs.Id,
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
            UnitDatabase database = GameManager.Instance.UnitDatabase;

            Id = data.Id;
            TimesPicked = data.TimesPicked;
            Name = data.Name;
            BodyType = data.BodyType;

            if (BodyType == 0)
            {
                Hair = database.GetFemaleOutfitById(data.HairId);
                Underwear = database.GetFemaleOutfitById(data.UnderwearId);
                Brassiere = database.GetFemaleOutfitById(data.BrassiereId);
                Helmet = database.GetFemaleOutfitById(data.HelmetId);
                Torso = database.GetFemaleOutfitById(data.TorsoId);
                Legs = database.GetFemaleOutfitById(data.LegsId);
            }
            else
            {
                Hair = database.GetMaleOutfitById(data.HairId);
                Beard = database.GetMaleOutfitById(data.BeardId);
                Mustache = database.GetMaleOutfitById(data.MustacheId);
                Underwear = database.GetMaleOutfitById(data.UnderwearId);
                Helmet = database.GetMaleOutfitById(data.HelmetId);
                Torso = database.GetMaleOutfitById(data.TorsoId);
                Legs = database.GetMaleOutfitById(data.LegsId);
            }

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
    public float R, G, B, A;

    public ColorSaver(Color color)
    {
        R = color.r;
        G = color.g;
        B = color.b;
        A = color.a;
    }

    public Color Color()
    {
        return new(R, G, B, A);
    }
}