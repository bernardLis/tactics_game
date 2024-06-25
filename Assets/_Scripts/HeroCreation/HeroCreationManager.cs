using System.Collections.Generic;
using Lis.Units.Hero.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class HeroCreationManager : MonoBehaviour
    {
        [SerializeField] Item[] _items;
        [SerializeField] ItemSetter _itemSetter;

        readonly Dictionary<ItemType, List<Item>> _itemDictionary = new();

        VisualElement _root;
        VisualElement _visualOptionContainer;

        void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _visualOptionContainer = _root.Q<VisualElement>("visualOptionContainer");
        }

        void Start()
        {
            SortItems();
            AddItemSelectors();
        }

        void SortItems()
        {
            foreach (Item item in _items)
            {
                if (!_itemDictionary.ContainsKey(item.ItemType))
                    _itemDictionary[item.ItemType] = new List<Item>();
                _itemDictionary[item.ItemType].Add(item);
            }
        }

        void AddItemSelectors()
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                ItemSelectorContainer itemSelectorContainer = new(_itemSetter, item.Key, item.Value);
                _visualOptionContainer.Add(itemSelectorContainer);
            }
        }
    }
}