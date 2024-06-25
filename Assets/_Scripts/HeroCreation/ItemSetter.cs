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

        public void Start()
        {
            foreach (ItemRenderer itemRenderer in _itemRenderers)
            {
                if (itemRenderer.ItemType != ItemType.Hair) continue;
                _hairRenderer = itemRenderer.Renderer;
            }
        }

        public void SetItem(Item newItem)
        {
            if (newItem.ItemType == ItemType.Helmet) HandleHair(newItem);

            foreach (ItemRenderer itemRenderer in _itemRenderers)
            {
                if (itemRenderer.ItemType != newItem.ItemType) continue;
                itemRenderer.Renderer.sharedMesh = newItem.ItemMeshRenderer.sharedMesh;
                itemRenderer.Renderer.sharedMaterials = newItem.ItemMeshRenderer.sharedMaterials;
            }
        }

        void HandleHair(Item newItem)
        {
            if (_hairRenderer == null) return;
            _hairRenderer.enabled = !newItem.DisableHair;
        }
    }
}