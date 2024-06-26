﻿using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class ItemSelectorContainer : VisualElement
    {
        const string _ussCommonButtonArrow = "common__button-arrow";

        const string _ussClassName = "item-selector-container__";
        const string _ussMain = _ussClassName + "main";
        const string _ussOdd = _ussClassName + "odd";
        const string _ussEven = _ussClassName + "even";


        readonly ItemSetter _itemSetter;
        readonly ItemType _itemType;
        readonly List<Item> _items;
        int _currentItemIndex;
        readonly Label _currentItemLabel;

        VisualElement _middleContainer;

        public ItemSelectorContainer(ItemSetter itemSetter, ItemType itemType, List<Item> items, bool isOdd = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.ItemSelectorContainerStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            AddToClassList(isOdd ? _ussOdd : _ussEven);

            _itemSetter = itemSetter;
            _itemType = itemType;
            _items = items;
            _currentItemIndex = 0;

            MyButton previousButton = new("<", _ussCommonButtonArrow, PreviousItem);
            MyButton nextButton = new(">", _ussCommonButtonArrow, NextItem);
            Add(previousButton);

            _middleContainer = new();
            _middleContainer.Add(new Label(_itemType.ToString()));
            _currentItemLabel = new(_items[_currentItemIndex].name);
            _middleContainer.Add(_currentItemLabel);
            Add(_middleContainer);

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