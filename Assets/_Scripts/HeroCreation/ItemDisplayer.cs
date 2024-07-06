using System;
using Lis.Units.Hero.Items;
using UnityEngine;

namespace Lis.HeroCreation
{
    public class ItemDisplayer : MonoBehaviour
    {
        [Serializable]
        public struct ItemRenderer
        {
            public ItemType ItemType;
            public SkinnedMeshRenderer Renderer;
        }

        VisualHero _visualHero;

        [SerializeField] Material _body;
        [SerializeField] Material _hair;
        [SerializeField] Material _underwear;
        [SerializeField] Material _outfit;

        [SerializeField] ItemRenderer[] _itemRenderers;
        SkinnedMeshRenderer _hairRenderer;
        SkinnedMeshRenderer _beardRenderer;
        SkinnedMeshRenderer _mustacheRenderer;

        SkinnedMeshRenderer _brassiereRenderer;

        static readonly int Color1 = Shader.PropertyToID("_Color1");
        static readonly int Color2 = Shader.PropertyToID("_Color2");
        static readonly int Color3 = Shader.PropertyToID("_Color3");

        public void Awake()
        {
            foreach (ItemRenderer itemRenderer in _itemRenderers)
            {
                if (itemRenderer.ItemType == ItemType.Hair)
                    _hairRenderer = itemRenderer.Renderer;
                if (itemRenderer.ItemType == ItemType.Beard)
                    _beardRenderer = itemRenderer.Renderer;
                if (itemRenderer.ItemType == ItemType.Mustache)
                    _mustacheRenderer = itemRenderer.Renderer;
                if (itemRenderer.ItemType == ItemType.Brassiere)
                    _brassiereRenderer = itemRenderer.Renderer;
            }
        }

        public void SetVisualHero(VisualHero visualHero)
        {
            _visualHero = visualHero;
            _visualHero.OnItemChanged += SetItem;

            SetSkinColor(_visualHero.SkinColor);
            SetEyeColor(_visualHero.EyeColor);
            SetEyebrowColor(_visualHero.EyebrowColor);

            SetMainHairColor(_visualHero.HairMainColor);
            SetDetailHairColor(_visualHero.HairDetailColor);

            SetMainUnderwearColor(_visualHero.UnderwearMainColor);
            SetDetailUnderwearColor(_visualHero.UnderwearDetailColor);

            SetMainUnderwearColor(_visualHero.OutfitMainColor);
            SetDetailOutfitColor(_visualHero.OutfitDetailColor);
            SetDetailSecondaryOutfitColor(_visualHero.OutfitDetailSecondaryColor);

            SetItem(_visualHero.Hair);
            if (visualHero.BodyType == 1) SetItem(visualHero.Beard);
            if (visualHero.BodyType == 1) SetItem(visualHero.Mustache);
            SetItem(visualHero.Underwear);
            if (visualHero.BodyType == 0) SetItem(visualHero.Brassiere);
            SetItem(_visualHero.Helmet);
            SetItem(_visualHero.Torso);
            SetItem(_visualHero.Legs);
        }

        public void SetSkinColor(Color c)
        {
            _body.SetColor(Color1, c);
            _visualHero.SkinColor = c;
        }

        public void SetEyeColor(Color c)
        {
            _body.SetColor(Color2, c);
            _visualHero.EyeColor = c;
        }

        public void SetEyebrowColor(Color c)
        {
            _body.SetColor(Color3, c);
            _visualHero.EyebrowColor = c;
        }

        public void SetMainHairColor(Color c)
        {
            _hair.SetColor(Color1, c);
            _visualHero.HairMainColor = c;
        }

        public void SetDetailHairColor(Color c)
        {
            _hair.SetColor(Color2, c);
            _visualHero.HairDetailColor = c;
        }

        public void SetMainUnderwearColor(Color c)
        {
            _underwear.SetColor(Color1, c);
            _visualHero.UnderwearMainColor = c;
        }

        public void SetDetailUnderwearColor(Color c)
        {
            _underwear.SetColor(Color2, c);
            _visualHero.UnderwearDetailColor = c;
        }

        public void SetMainOutfitColor(Color c)
        {
            _outfit.SetColor(Color1, c);
            _visualHero.OutfitMainColor = c;
        }

        public void SetDetailOutfitColor(Color c)
        {
            _outfit.SetColor(Color2, c);
            _visualHero.OutfitDetailColor = c;
        }

        public void SetDetailSecondaryOutfitColor(Color c)
        {
            _outfit.SetColor(Color3, c);
            _visualHero.OutfitDetailSecondaryColor = c;
        }

        public void SetItem(Item newItem)
        {
            if (newItem.ItemType == ItemType.Helmet) HandleHair(newItem);
            if (newItem.ItemType == ItemType.Torso) HandleBrassiere(newItem);

            foreach (ItemRenderer itemRenderer in _itemRenderers)
            {
                if (itemRenderer.ItemType != newItem.ItemType) continue;
                itemRenderer.Renderer.sharedMesh = newItem.ItemMeshRenderer.sharedMesh;
                itemRenderer.Renderer.sharedMaterials = newItem.ItemMeshRenderer.sharedMaterials;
            }
        }

        void HandleBrassiere(Item newItem)
        {
            if (_brassiereRenderer == null) return;
            if (newItem.Id == "5fd353c0-3da2-4179-89d2-709b7db52c6b") // base torso
            {
                _brassiereRenderer.enabled = true;
                return;
            }

            _brassiereRenderer.enabled = false;
        }

        void HandleHair(Item newItem)
        {
            if (_hairRenderer != null)
                _hairRenderer.enabled = !newItem.DisableHair;

            if (_beardRenderer != null)
                _beardRenderer.enabled = !newItem.DisableHair;

            if (_mustacheRenderer != null)
                _mustacheRenderer.enabled = !newItem.DisableHair;
        }
    }
}