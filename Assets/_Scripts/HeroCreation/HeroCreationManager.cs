using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class HeroCreationManager : MonoBehaviour
    {
        [SerializeField] ItemSetter _itemSetter;

        readonly Dictionary<ItemType, List<Item>> _itemDictionary = new();

        VisualElement _root;
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
            _customizationScrollView = _root.Q<ScrollView>("customizationScrollView");
        }

        void Start()
        {
            _allColors = GameManager.Instance.UnitDatabase.GetAllHeroCustomizationColors();

            SortItems();
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

        void AddItemSelectors()
        {
            bool isOdd = false;
            foreach (KeyValuePair<ItemType, List<Item>> item in _itemDictionary)
            {
                ItemSelectorContainer itemSelectorContainer = new(_itemSetter, item.Key, item.Value, isOdd);
                _customizationScrollView.Add(itemSelectorContainer);
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

        void AddUnderWearColorSetter()
        {
            VisualElement underwearColorSetter = new();
            _colorContainer.Add(underwearColorSetter);

            underwearColorSetter.Add(new HorizontalSpacerElement());
            underwearColorSetter.Add(new Label("Underwear"));

            underwearColorSetter.Add(CreateColorSwatches("Main: ", _underwear, "_Color1"));
            underwearColorSetter.Add(CreateColorSwatches("Detail: ", _underwear, "_Color2"));
        }

        void AddBodyColorSetter()
        {
            VisualElement bodyColorSetter = new();
            _colorContainer.Add(bodyColorSetter);

            bodyColorSetter.Add(new HorizontalSpacerElement());
            bodyColorSetter.Add(new Label("Body"));

            bodyColorSetter.Add(CreateColorSwatches("Skin: ", _body, "_Color1"));
            bodyColorSetter.Add(CreateColorSwatches("Eye: ", _body, "_Color2"));
            bodyColorSetter.Add(CreateColorSwatches("Eyebrow: ", _body, "_Color3"));
        }

        void AddHairColorSetter()
        {
            VisualElement hairColorSetter = new();
            _colorContainer.Add(hairColorSetter);
            hairColorSetter.Add(new HorizontalSpacerElement());
            hairColorSetter.Add(new Label("Hair"));

            hairColorSetter.Add(CreateColorSwatches("Main: ", _hair, "_Color1"));
            hairColorSetter.Add(CreateColorSwatches("Detail: ", _hair, "_Color2"));
        }

        void AddArmorColorSetter()
        {
            VisualElement armorColorSetter = new();
            _colorContainer.Add(armorColorSetter);
            armorColorSetter.Add(new HorizontalSpacerElement());
            armorColorSetter.Add(new Label("Armor"));

            armorColorSetter.Add(CreateColorSwatches("Main: ", _armor, "_Color1"));
            armorColorSetter.Add(CreateColorSwatches("Detail: ", _armor, "_Color2"));
            armorColorSetter.Add(CreateColorSwatches("Detail: ", _armor, "_Color3"));
        }

        VisualElement CreateColorSwatches(string title, Material mat, string colorName)
        {
            VisualElement colorContainer = new();

            colorContainer.style.flexDirection = FlexDirection.Row;
            colorContainer.style.width = Length.Percent(100);
            colorContainer.style.justifyContent = Justify.FlexEnd;
            Label titleLabel = new($"{title}: ");
            colorContainer.Add(titleLabel);
            foreach (Color c in _allColors)
            {
                ColorSwatch colorSwatch = new(c);
                colorSwatch.OnSelected += color => { mat.SetColor(colorName, color); };
                colorContainer.Add(colorSwatch);
            }

            return colorContainer;
        }
    }
}