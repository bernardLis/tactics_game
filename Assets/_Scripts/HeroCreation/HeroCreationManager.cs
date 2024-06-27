using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class HeroCreationManager : MonoBehaviour
    {
        const string _ussCommonTextLarge = "common__text-large";
        const string _ussCommonTextVeryLarge = "common__text-very-large";

        [SerializeField] ItemSetter _itemSetter;

        readonly Dictionary<ItemType, List<Item>> _itemDictionary = new();

        VisualElement _root;
        VisualElement _visualOptionContainer;
        ScrollView _customizationScrollView;
        VisualElement _colorContainer;

        List<Color> _allColors = new();

        [SerializeField] Material _hair;
        [SerializeField] Material _body;
        [SerializeField] Material _underwear;
        [SerializeField] Material _armor;

        void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _visualOptionContainer = _root.Q<VisualElement>("visualOptionContainer");
            _customizationScrollView = _root.Q<ScrollView>("customizationScrollView");
        }

        void Start()
        {
            _allColors = GameManager.Instance.UnitDatabase.GetAllHeroCustomizationColors();

            SortItems();
            AddTitle();
            AddItemSelectors();
            AddColorSetters();
        }

        void SortItems()
        {
            foreach (Item item in GameManager.Instance.UnitDatabase.GetAllFemaleHeroOutfits)
            {
                if (!_itemDictionary.ContainsKey(item.ItemType))
                    _itemDictionary[item.ItemType] = new();
                _itemDictionary[item.ItemType].Add(item);
            }
        }

        void AddTitle()
        {
            Label title = new("Hero Customization");
            title.AddToClassList(_ussCommonTextVeryLarge);
            _visualOptionContainer.Insert(0, title);

            _visualOptionContainer.Insert(1, new HorizontalSpacerElement());
        }

        void AddItemSelectors()
        {
            bool isOdd = false;
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                ItemSelectorElement itemSelectorElement = new(_itemSetter, item.Key, item.Value, isOdd);
                _customizationScrollView.Add(itemSelectorElement);
                isOdd = !isOdd;
            }
        }

        void AddColorSetters()
        {
            _colorContainer = new();
            _colorContainer.style.alignItems = Align.Center;
            _customizationScrollView.Add(_colorContainer);

            AddHairColorSetter();
            AddBodyColorSetter();
            AddUnderWearColorSetter();
            AddArmorColorSetter();
        }

        void AddHairColorSetter()
        {
            VisualElement hairColorSetter = new();
            _colorContainer.Add(hairColorSetter);
            hairColorSetter.Add(new HorizontalSpacerElement());
            Label title = new("Hair");
            title.AddToClassList(_ussCommonTextLarge);
            hairColorSetter.Add(title);


            hairColorSetter.Add(new ColorSelectorElement("Main", _hair,
                "_Color1", _allColors, _visualOptionContainer));
            hairColorSetter.Add(new ColorSelectorElement("Detail", _hair,
                "_Color2", _allColors, _visualOptionContainer));
        }

        void AddBodyColorSetter()
        {
            VisualElement bodyColorSetter = new();
            _colorContainer.Add(bodyColorSetter);

            bodyColorSetter.Add(new HorizontalSpacerElement());
            Label title = new("Body");
            title.AddToClassList(_ussCommonTextLarge);

            bodyColorSetter.Add(title);

            bodyColorSetter.Add(new ColorSelectorElement("Skin", _body,
                "_Color1", _allColors, _visualOptionContainer));
            bodyColorSetter.Add(new ColorSelectorElement("Eye", _body,
                "_Color2", _allColors, _visualOptionContainer));
            bodyColorSetter.Add(new ColorSelectorElement("Eyebrow", _body,
                "_Color3", _allColors, _visualOptionContainer));
        }


        void AddUnderWearColorSetter()
        {
            VisualElement underwearColorSetter = new();
            _colorContainer.Add(underwearColorSetter);

            underwearColorSetter.Add(new HorizontalSpacerElement());
            Label title = new("Underwear");
            title.AddToClassList(_ussCommonTextLarge);

            underwearColorSetter.Add(title);

            underwearColorSetter.Add(new ColorSelectorElement("Main", _underwear,
                "_Color1", _allColors, _visualOptionContainer));
            underwearColorSetter.Add(new ColorSelectorElement("Detail", _underwear,
                "_Color2", _allColors, _visualOptionContainer));
        }

        void AddArmorColorSetter()
        {
            VisualElement armorColorSetter = new();
            _colorContainer.Add(armorColorSetter);
            armorColorSetter.Add(new HorizontalSpacerElement());
            Label title = new("Armor");
            title.AddToClassList(_ussCommonTextLarge);

            armorColorSetter.Add(title);

            armorColorSetter.Add(new ColorSelectorElement("Main", _armor,
                "_Color1", _allColors, _visualOptionContainer));
            armorColorSetter.Add(new ColorSelectorElement("Detail", _armor,
                "_Color2", _allColors, _visualOptionContainer));
            armorColorSetter.Add(new ColorSelectorElement("Detail", _armor,
                "_Color3", _allColors, _visualOptionContainer));
        }
    }
}