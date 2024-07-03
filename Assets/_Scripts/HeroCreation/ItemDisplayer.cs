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

        List<Item> _allItems;
        ItemSetter _itemSetter;

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

        void SetVisualHero(VisualHero visualHero)
        {
            _visualHero = visualHero;

            // colors
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

            // items
            _itemSetter.SetItem(GetItemById(_visualHero.HairId));
            if (_visualHero.BodyType == 1) _itemSetter.SetItem(GetItemById(_visualHero.BeardId));
            if (_visualHero.BodyType == 1) _itemSetter.SetItem(GetItemById(_visualHero.MustacheId));
            _itemSetter.SetItem(GetItemById(_visualHero.UnderwearId));
            if (_visualHero.BodyType == 0) _itemSetter.SetItem(GetItemById(_visualHero.BrassiereId));
            _itemSetter.SetItem(GetItemById(_visualHero.HelmetId));
            _itemSetter.SetItem(GetItemById(_visualHero.TorsoId));
            _itemSetter.SetItem(GetItemById(_visualHero.LegsId));
        }

        Item GetItemById(string id)
        {
            foreach (Item item in _allItems)
            {
                if (item.Id == id)
                    return item;
            }

            return null;
        }

        public void Initialize(List<Item> allItems, VisualHero visualHero)
        {
            _visualHero = visualHero;
            _allItems = allItems;
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
            _skinColorElement = new("Skin", _body, "_Color1", _allColors, _visualOptionContainer);
            _eyeColorElement = new("Eye", _body, "_Color2", _allColors, _visualOptionContainer);
            _eyebrowColorElement = new("Eyebrow", _body, "_Color3", _allColors, _visualOptionContainer);

            _eyeColorElement.OnColorPickerShowed += _cameraManager.LookAtHead;
            _eyebrowColorElement.OnColorPickerShowed += _cameraManager.LookAtHead;

            _eyeColorElement.OnColorPickerClosed += _cameraManager.LookAtDefault;
            _eyebrowColorElement.OnColorPickerClosed += _cameraManager.LookAtDefault;

            _skinColorElement.OnColorSelected += (c) => _visualHero.SkinColor = c;
            _eyeColorElement.OnColorSelected += (c) => _visualHero.EyeColor = c;
            _eyebrowColorElement.OnColorSelected += (c) => _visualHero.EyebrowColor = c;

            parent.Add(_skinColorElement);
            parent.Add(_eyeColorElement);
            parent.Add(_eyebrowColorElement);
        }

        void AddHairItems(VisualElement parent)
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                if (item.Key is not (ItemType.Hair or ItemType.Beard or ItemType.Mustache)) continue;

                ItemSelectorElement itemSelectorElement = new(_itemSetter, item.Key, item.Value);
                itemSelectorElement.OnItemChanged += (type, id) => _visualHero.SetItem(type, id);
                parent.Add(itemSelectorElement);
            }
        }


        void AddHairColor(VisualElement parent)
        {
            _mainHairColorElement = new("Main", _hair, "_Color1", _allColors, _visualOptionContainer);
            _detailHairColorElement = new("Detail", _hair, "_Color2", _allColors, _visualOptionContainer);

            _mainHairColorElement.OnColorSelected += (c) => _visualHero.HairMainColor = c;
            _detailHairColorElement.OnColorSelected += (c) => _visualHero.HairDetailColor = c;

            parent.Add(_mainHairColorElement);
            parent.Add(_detailHairColorElement);
        }

        void AddUnderwearItems(VisualElement parent)
        {
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                if (item.Key is not (ItemType.Underwear or ItemType.Brassiere)) continue;

                ItemSelectorElement itemSelectorElement = new(_itemSetter, item.Key, item.Value);
                itemSelectorElement.OnItemChanged += (type, id) => _visualHero.SetItem(type, id);
                parent.Add(itemSelectorElement);
            }
        }

        void AddUnderWearColor(VisualElement parent)
        {
            _mainUnderwearColorElement = new("Main", _underwear, "_Color1", _allColors, _visualOptionContainer);
            _detailUnderwearColorElement = new("Detail", _underwear, "_Color2", _allColors, _visualOptionContainer);

            _mainUnderwearColorElement.OnColorSelected += (c) => _visualHero.UnderwearMainColor = c;
            _detailUnderwearColorElement.OnColorSelected += (c) => _visualHero.UnderwearDetailColor = c;

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

                ItemSelectorElement itemSelectorElement = new(_itemSetter, item.Key, item.Value);
                itemSelectorElement.OnItemChanged += (type, id) => _visualHero.SetItem(type, id);
                parent.Add(itemSelectorElement);
            }
        }

        void AddOutfitColor(VisualElement parent)
        {
            _mainOutfitColorElement = new("Main", _armor, "_Color1", _allColors, _visualOptionContainer);
            _detailOutfitColorElement = new("Detail", _armor, "_Color2", _allColors, _visualOptionContainer);
            _detailOutfitColorSecondaryElement = new("Detail", _armor, "_Color3", _allColors, _visualOptionContainer);

            _mainOutfitColorElement.OnColorSelected += (c) => _visualHero.OutfitMainColor = c;
            _detailOutfitColorElement.OnColorSelected += (c) => _visualHero.OutfitDetailColor = c;
            _detailOutfitColorSecondaryElement.OnColorSelected += (c) => _visualHero.OutfitDetailSecondaryColor = c;

            parent.Add(_mainOutfitColorElement);
            parent.Add(_detailOutfitColorElement);
            parent.Add(_detailOutfitColorSecondaryElement);
        }
    }
}