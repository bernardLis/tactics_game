using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class ItemDisplayer : MonoBehaviour
    {
        const string _ussCommonTextLarge = "common__text-large";

        const string _ussClassName = "hero-creation__";
        const string _ussSetterTitle = _ussClassName + "setter-title";
        const string _ussSetterContainer = _ussClassName + "setter-container";

        [SerializeField] Material _hair;
        [SerializeField] Material _body;
        [SerializeField] Material _underwear;
        [SerializeField] Material _armor;

        CameraManager _cameraManager;

        List<Color> _allColors = new();

        readonly Dictionary<ItemType, List<Item>> _itemDictionary = new();

        ItemSetter _itemSetter;

        VisualElement _root;
        VisualElement _visualOptionContainer;
        ScrollView _customizationScrollView;
        VisualElement _setterContainer;

        void Start()
        {
            _cameraManager = HeroCreationManager.Instance.GetComponent<CameraManager>();

            _allColors = GameManager.Instance.UnitDatabase.GetAllHeroCustomizationColors();
            _itemSetter = GetComponent<ItemSetter>();
            _root = HeroCreationManager.Instance.Root;
            _visualOptionContainer = _root.Q<VisualElement>("visualOptionContainer");
            _customizationScrollView = _root.Q<ScrollView>("customizationScrollView");
        }

        public void Activate()
        {
            _setterContainer.style.display = DisplayStyle.Flex;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            _setterContainer.style.display = DisplayStyle.None;
            gameObject.SetActive(false);
        }

        public void Initialize(List<Item> allItems)
        {
            SortItems(allItems);

            _setterContainer = new();
            _customizationScrollView.Add(_setterContainer);

            AddBodySetters();
            AddHairSetters();
            AddUnderwearSetters();
            AddArmorSetters();

            Deactivate();
        }

        void SortItems(List<Item> allItems)
        {
            foreach (Item item in allItems)
            {
                if (!_itemDictionary.ContainsKey(item.ItemType))
                    _itemDictionary[item.ItemType] = new();
                _itemDictionary[item.ItemType].Add(item);
            }
        }

        void AddBodySetters()
        {
            VisualElement bodyContainer = new();
            bodyContainer.AddToClassList(_ussSetterContainer);
            _setterContainer.Add(bodyContainer);

            Label title = new("Body");
            title.AddToClassList(_ussCommonTextLarge);
            title.AddToClassList(_ussSetterTitle);
            bodyContainer.Add(title);

            AddBodyColor(bodyContainer);
        }

        void AddHairSetters()
        {
            VisualElement hairContainer = new();
            hairContainer.AddToClassList(_ussSetterContainer);
            _setterContainer.Add(hairContainer);

            Label title = new("Hair");
            title.AddToClassList(_ussCommonTextLarge);
            title.AddToClassList(_ussSetterTitle);
            hairContainer.Add(title);

            AddHairItems(hairContainer);
            AddHairColor(hairContainer);
        }

        void AddUnderwearSetters()
        {
            VisualElement underwearContainer = new();
            underwearContainer.AddToClassList(_ussSetterContainer);
            _setterContainer.Add(underwearContainer);

            Label title = new("Underwear");
            title.AddToClassList(_ussCommonTextLarge);
            title.AddToClassList(_ussSetterTitle);
            underwearContainer.Add(title);

            AddUnderwearItems(underwearContainer);
            AddUnderWearColor(underwearContainer);
        }

        void AddArmorSetters()
        {
            VisualElement armorContainer = new();
            armorContainer.AddToClassList(_ussSetterContainer);
            _setterContainer.Add(armorContainer);

            Label title = new("Armor");
            title.AddToClassList(_ussCommonTextLarge);
            title.AddToClassList(_ussSetterTitle);
            armorContainer.Add(title);

            AddArmorItems(armorContainer);
            AddArmorColor(armorContainer);
        }


        void AddBodyColor(VisualElement parent)
        {
            parent.Add(new ColorSelectorElement("Skin", _body,
                "_Color1", _allColors, _visualOptionContainer));

            ColorSelectorElement eyeElement = new("Eye", _body,
                "_Color2", _allColors, _visualOptionContainer);
            ColorSelectorElement eyebrowElement = new("Eyebrow", _body,
                "_Color3", _allColors, _visualOptionContainer);

            eyeElement.OnColorPickerShowed += _cameraManager.LookAtHead;
            eyebrowElement.OnColorPickerShowed += _cameraManager.LookAtHead;

            eyeElement.OnColorPickerClosed += _cameraManager.LookAtDefault;
            eyebrowElement.OnColorPickerClosed += _cameraManager.LookAtDefault;

            parent.Add(eyeElement);
            parent.Add(eyebrowElement);
        }

        void AddHairItems(VisualElement parent)
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                if (item.Key is not (ItemType.Hair or ItemType.Beard or ItemType.Mustache)) continue;

                ItemSelectorElement itemSelectorElement = new(_itemSetter, item.Key, item.Value);
                parent.Add(itemSelectorElement);
            }
        }


        void AddHairColor(VisualElement parent)
        {
            parent.Add(new ColorSelectorElement("Main", _hair,
                "_Color1", _allColors, _visualOptionContainer));
            parent.Add(new ColorSelectorElement("Detail", _hair,
                "_Color2", _allColors, _visualOptionContainer));
        }

        void AddUnderwearItems(VisualElement parent)
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                if (item.Key is not (ItemType.Underwear or ItemType.Brassiere)) continue;

                ItemSelectorElement itemSelectorElement = new(_itemSetter, item.Key, item.Value);
                parent.Add(itemSelectorElement);
            }
        }

        void AddUnderWearColor(VisualElement parent)
        {
            parent.Add(new ColorSelectorElement("Main", _underwear,
                "_Color1", _allColors, _visualOptionContainer));
            parent.Add(new ColorSelectorElement("Detail", _underwear,
                "_Color2", _allColors, _visualOptionContainer));
        }


        void AddArmorItems(VisualElement parent)
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                if (item.Key is not (ItemType.Helmet or
                    ItemType.Shoulders or ItemType.Torso or ItemType.Waist
                    or ItemType.Legs)) continue;

                ItemSelectorElement itemSelectorElement = new(_itemSetter, item.Key, item.Value);
                parent.Add(itemSelectorElement);
            }
        }

        void AddArmorColor(VisualElement parent)
        {
            parent.Add(new ColorSelectorElement("Main", _armor,
                "_Color1", _allColors, _visualOptionContainer));
            parent.Add(new ColorSelectorElement("Detail", _armor,
                "_Color2", _allColors, _visualOptionContainer));
            parent.Add(new ColorSelectorElement("Detail", _armor,
                "_Color3", _allColors, _visualOptionContainer));
        }
    }
}