using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class ItemSelectorElement : VisualElement
    {
        const string _ussCommonButtonArrow = "common__button-arrow";

        const string _ussClassName = "item-selector-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussOdd = _ussClassName + "odd";
        const string _ussEven = _ussClassName + "even";


        readonly ItemSetter _itemSetter;
        readonly ItemType _itemType;
        readonly List<Item> _items;
        int _currentItemIndex;
        readonly Label _currentItemLabel;

        public ItemSelectorElement(ItemSetter itemSetter, ItemType itemType, List<Item> items)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.ItemSelectorElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            _itemSetter = itemSetter;
            _itemType = itemType;
            _items = items;
            _currentItemIndex = 0;

            MyButton previousButton = new("<", _ussCommonButtonArrow, PreviousItem);
            MyButton nextButton = new(">", _ussCommonButtonArrow, NextItem);
            Add(previousButton);

            _currentItemLabel = new(_items[_currentItemIndex].name);
            _currentItemLabel.style.fontSize = 26;
            Add(_currentItemLabel);

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