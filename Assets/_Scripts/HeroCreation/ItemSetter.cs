using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class ItemSetter : MonoBehaviour
    {
        const string _ussCommonTextLarge = "common__text-large";

        const string _ussClassName = "hero-creation__";
        const string _ussSetterTitle = _ussClassName + "setter-title";
        const string _ussSetterContainer = _ussClassName + "setter-container";

        CameraManager _cameraManager;

        List<Color> _allColors = new();

        readonly Dictionary<ItemType, List<Item>> _itemDictionary = new();

        ItemDisplayer _itemDisplayer;

        VisualElement _root;
        VisualElement _visualOptionContainer;
        ScrollView _customizationScrollView;
        VisualElement _setterContainer;

        ColorSelectorElement _skinColorElement;
        ColorSelectorElement _eyeColorElement;
        ColorSelectorElement _eyebrowColorElement;

        ColorSelectorElement _mainHairColorElement;
        ColorSelectorElement _detailHairColorElement;

        ColorSelectorElement _mainUnderwearColorElement;
        ColorSelectorElement _detailUnderwearColorElement;

        ColorSelectorElement _mainOutfitColorElement;
        ColorSelectorElement _detailOutfitColorElement;
        ColorSelectorElement _detailOutfitColorSecondaryElement;

        VisualHero _visualHero;

        void Awake()
        {
            _cameraManager = HeroCreationManager.Instance.GetComponent<CameraManager>();

            _allColors = GameManager.Instance.UnitDatabase.GetAllHeroCustomizationColors();
            _itemDisplayer = GetComponent<ItemDisplayer>();
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

        void SetVisualHero(VisualHero visualHero)
        {
            _visualHero = visualHero;
            _itemDisplayer.SetVisualHero(visualHero);

            _skinColorElement.SetColor(_visualHero.SkinColor);
            _eyeColorElement.SetColor(_visualHero.EyeColor);
            _eyebrowColorElement.SetColor(_visualHero.EyebrowColor);

            _mainHairColorElement.SetColor(_visualHero.HairMainColor);
            _detailHairColorElement.SetColor(_visualHero.HairDetailColor);

            _mainUnderwearColorElement.SetColor(_visualHero.UnderwearMainColor);
            _detailUnderwearColorElement.SetColor(_visualHero.UnderwearDetailColor);

            _mainOutfitColorElement.SetColor(_visualHero.OutfitMainColor);
            _detailOutfitColorElement.SetColor(_visualHero.OutfitDetailColor);
            _detailOutfitColorSecondaryElement.SetColor(_visualHero.OutfitDetailSecondaryColor);
        }

        public void Initialize(List<Item> allItems, VisualHero visualHero)
        {
            _visualHero = visualHero;
            SortItems(allItems);

            _setterContainer = new();
            _customizationScrollView.Add(_setterContainer);

            AddBodySetters();
            AddHairSetters();
            AddUnderwearSetters();
            AddOutfitSetters();

            SetVisualHero(_visualHero);
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

        void AddOutfitSetters()
        {
            VisualElement outfitContainer = new();
            outfitContainer.AddToClassList(_ussSetterContainer);
            _setterContainer.Add(outfitContainer);

            Label title = new("Outfit");
            title.AddToClassList(_ussCommonTextLarge);
            title.AddToClassList(_ussSetterTitle);
            outfitContainer.Add(title);

            AddOutfitItems(outfitContainer);
            AddOutfitColor(outfitContainer);
        }


        void AddBodyColor(VisualElement parent)
        {
            _skinColorElement = new("Skin", _visualHero.SkinColor, _allColors, _visualOptionContainer);
            _eyeColorElement = new("Eye", _visualHero.EyeColor, _allColors, _visualOptionContainer);
            _eyebrowColorElement = new("Eyebrow", _visualHero.EyebrowColor, _allColors, _visualOptionContainer);

            _eyeColorElement.OnColorPickerShowed += _cameraManager.LookAtHead;
            _eyebrowColorElement.OnColorPickerShowed += _cameraManager.LookAtHead;

            _eyeColorElement.OnColorPickerClosed += _cameraManager.LookAtDefault;
            _eyebrowColorElement.OnColorPickerClosed += _cameraManager.LookAtDefault;

            _skinColorElement.OnColorSelected += _itemDisplayer.SetSkinColor;
            _eyeColorElement.OnColorSelected += _itemDisplayer.SetEyeColor;
            _eyebrowColorElement.OnColorSelected += _itemDisplayer.SetEyebrowColor;

            parent.Add(_skinColorElement);
            parent.Add(_eyeColorElement);
            parent.Add(_eyebrowColorElement);
        }

        void AddHairItems(VisualElement parent)
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                if (item.Key is not (ItemType.Hair or ItemType.Beard or ItemType.Mustache)) continue;

                ItemSelectorElement itemSelectorElement = new(_itemDisplayer, item.Key, item.Value);
                itemSelectorElement.OnItemChanged += (type, id) => _visualHero.SetItem(type, id);
                parent.Add(itemSelectorElement);
            }
        }

        void AddHairColor(VisualElement parent)
        {
            _mainHairColorElement = new("Main", _visualHero.HairMainColor, _allColors, _visualOptionContainer);
            _detailHairColorElement = new("Detail", _visualHero.HairDetailColor, _allColors, _visualOptionContainer);

            _mainHairColorElement.OnColorSelected += _itemDisplayer.SetMainHairColor;
            _detailHairColorElement.OnColorSelected += _itemDisplayer.SetDetailHairColor;

            parent.Add(_mainHairColorElement);
            parent.Add(_detailHairColorElement);
        }

        void AddUnderwearItems(VisualElement parent)
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                if (item.Key is not (ItemType.Underwear or ItemType.Brassiere)) continue;

                ItemSelectorElement itemSelectorElement = new(_itemDisplayer, item.Key, item.Value);
                itemSelectorElement.OnItemChanged += (type, id) => _visualHero.SetItem(type, id);
                parent.Add(itemSelectorElement);
            }
        }

        void AddUnderWearColor(VisualElement parent)
        {
            _mainUnderwearColorElement =
                new("Main", _visualHero.UnderwearMainColor, _allColors, _visualOptionContainer);
            _detailUnderwearColorElement =
                new("Detail", _visualHero.UnderwearDetailColor, _allColors, _visualOptionContainer);

            _mainUnderwearColorElement.OnColorSelected += _itemDisplayer.SetMainUnderwearColor;
            _detailUnderwearColorElement.OnColorSelected += _itemDisplayer.SetDetailUnderwearColor;

            parent.Add(_mainUnderwearColorElement);
            parent.Add(_detailUnderwearColorElement);
        }

        void AddOutfitItems(VisualElement parent)
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                if (item.Key is not (ItemType.Helmet or
                    ItemType.Shoulders or ItemType.Torso or ItemType.Waist
                    or ItemType.Legs)) continue;

                ItemSelectorElement itemSelectorElement = new(_itemDisplayer, item.Key, item.Value);
                itemSelectorElement.OnItemChanged += (type, id) => _visualHero.SetItem(type, id);
                parent.Add(itemSelectorElement);
            }
        }

        void AddOutfitColor(VisualElement parent)
        {
            _mainOutfitColorElement = new("Main", _visualHero.OutfitMainColor, _allColors, _visualOptionContainer);
            _detailOutfitColorElement =
                new("Detail", _visualHero.OutfitDetailColor, _allColors, _visualOptionContainer);
            _detailOutfitColorSecondaryElement = new("Detail", _visualHero.OutfitDetailSecondaryColor, _allColors,
                _visualOptionContainer);

            _mainOutfitColorElement.OnColorSelected += _itemDisplayer.SetMainOutfitColor;
            _detailOutfitColorElement.OnColorSelected += _itemDisplayer.SetDetailOutfitColor;
            _detailOutfitColorSecondaryElement.OnColorSelected += _itemDisplayer.SetDetailSecondaryOutfitColor;

            parent.Add(_mainOutfitColorElement);
            parent.Add(_detailOutfitColorElement);
            parent.Add(_detailOutfitColorSecondaryElement);
        }
    }
}