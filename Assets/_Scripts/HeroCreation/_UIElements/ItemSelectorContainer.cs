using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class ItemSelectorContainer : VisualElement
    {
        const string _ussCommonButton = "common__button";

        readonly ItemSetter _itemSetter;
        readonly ItemType _itemType;
        readonly List<Item> _items;
        int _currentItemIndex;
        readonly Label _currentItemLabel;

        public ItemSelectorContainer(ItemSetter itemSetter, ItemType itemType, List<Item> items)
        {
            _itemSetter = itemSetter;
            _itemType = itemType;
            _items = items;
            _currentItemIndex = 0;

            style.flexDirection = FlexDirection.Row;

            MyButton previousButton = new("<", _ussCommonButton, PreviousItem);
            MyButton nextButton = new(">", _ussCommonButton, NextItem);
            Add(previousButton);

            VisualElement middleContainer = new();
            Add(new Label(_itemType.ToString()));
            _currentItemLabel = new(_items[_currentItemIndex].name);
            middleContainer.Add(_currentItemLabel);
            Add(middleContainer);

            Add(nextButton);
        }


        void SetItem()
        {
            _itemSetter.SetItem(_items[_currentItemIndex]);
            _currentItemLabel.text = _items[_currentItemIndex].name;
        }

        void PreviousItem()
        {
            if (_currentItemIndex == 0)
                _currentItemIndex = _items.Count - 1;
            else
                _currentItemIndex--;

            SetItem();
        }

        void NextItem()
        {
            if (_currentItemIndex == _items.Count - 1)
                _currentItemIndex = 0;
            else
                _currentItemIndex++;
            SetItem();
        }
    }
}