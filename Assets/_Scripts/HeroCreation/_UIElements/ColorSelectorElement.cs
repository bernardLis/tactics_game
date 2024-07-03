using System;
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
        readonly VisualElement _colorContainer;

        readonly List<Color> _allColors;

        public event Action OnColorPickerShowed;
        public event Action OnColorPickerClosed;
        public event Action<Color> OnColorSelected;

        public ColorSelectorElement(string title, Color startColor, List<Color> colors,
            VisualElement pickerParent)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.ColorSelectorElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            _allColors = colors;
            _pickerParent = pickerParent;

            _colorContainer = new();
            _colorContainer.AddToClassList(_ussColorContainer);
            _colorContainer.style.backgroundColor = startColor;

            MyButton selectColorButton = new($"Select {title} Color", _ussSelectColorButton, ShowColorPicker);
            selectColorButton.Insert(0, _colorContainer);
            Add(selectColorButton);
        }

        void ShowColorPicker()
        {
            OnColorPickerShowed?.Invoke();
            ColorPickerPopUpElement colorPickerPopUpElement = new(_pickerParent);
            colorPickerPopUpElement.OnColorSelected += ColorSelected;
            colorPickerPopUpElement.Initialize(_allColors);
            colorPickerPopUpElement.OnHide += () => { OnColorPickerClosed?.Invoke(); };
        }

        public void SetColor(Color c)
        {
            ColorSelected(c);
        }

        void ColorSelected(Color c)
        {
            _colorContainer.style.backgroundColor = c;
            OnColorSelected?.Invoke(c);
        }
    }
}