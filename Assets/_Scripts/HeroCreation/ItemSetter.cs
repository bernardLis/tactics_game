using System;
using Lis.Units.Hero.Items;
using UnityEngine;

namespace Lis.HeroCreation
{
    public class ItemSetter : MonoBehaviour
    {
        [Serializable]
        public struct ItemRenderer
        {
            public ItemType ItemType;
            public SkinnedMeshRenderer Renderer;
        }

        [SerializeField] ItemRenderer[] _itemRenderers;
        SkinnedMeshRenderer _hairRenderer;
        SkinnedMeshRenderer _beardRenderer;
        SkinnedMeshRenderer _mustacheRenderer;

        SkinnedMeshRenderer _brassiereRenderer;

        public void Start()
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

        public void SetItem(Item newItem)
        {
            if (newItem.ItemType == ItemType.Helmet) HandleHair(newItem);
            if (newItem.ItemType == ItemType.Torso) HandleBrassiere(newItem);

            foreach (ItemRenderer itemRenderer in _itemRenderers)
            {
                if (itemRenderer.ItemType != newItem.ItemType) continue;
                Debug.Log($"Setting item {newItem.ItemType} to {newItem.name}");
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
            if (_hairRenderer == null)
                _hairRenderer.enabled = !newItem.DisableHair;

            if (_beardRenderer == null)
                _beardRenderer.enabled = !newItem.DisableHair;

            if (_mustacheRenderer != null)
                _mustacheRenderer.enabled = !newItem.DisableHair;
        }

        public void SetColor(ItemType itemType, Color color)
        {
            foreach (ItemRenderer itemRenderer in _itemRenderers)
            {
                if (itemRenderer.ItemType != itemType) continue;
                itemRenderer.Renderer.material.color = color;
            }
        }
    }
}