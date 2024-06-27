using System.Collections.Generic;
using Lis.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class ColorSelectorElement : VisualElement
    {

        const string _ussClassName = "color-selector-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussColorContainer = _ussClassName + "color-container";
        const string _ussSelectColorButton = _ussClassName + "select-color-button";


        readonly VisualElement _pickerParent;
        readonly Material _material;
        readonly string _propertyName;
        readonly VisualElement _colorContainer;

        readonly List<Color> _allColors;

        public ColorSelectorElement(string title, Material material, string propertyName, List<Color> colors,
            VisualElement pickerParent)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.ColorSelectorElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            _material = material;
            _propertyName = propertyName;
            _allColors = colors;
            _pickerParent = pickerParent;

            _colorContainer = new();
            _colorContainer.AddToClassList(_ussColorContainer);
            _colorContainer.style.backgroundColor = material.GetColor(propertyName);

            MyButton selectColorButton = new($"Select {title} Color", _ussSelectColorButton, ShowColorPicker);
            selectColorButton.Insert(0, _colorContainer);
            Add(selectColorButton);
        }

        void ShowColorPicker()
        {
            ColorPickerPopUpElement colorPickerPopUpElement = new(_pickerParent);
            colorPickerPopUpElement.OnColorSelected += OnColorSelected;
            colorPickerPopUpElement.Initialize(_allColors);
        }

        void OnColorSelected(Color c)
        {
            _colorContainer.style.backgroundColor = c;
            _material.SetColor(_propertyName, c);
        }
    }
}