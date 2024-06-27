using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ColorPickerPopUpElement : PopUpElement
    {
        public event Action<Color> OnColorSelected;

        public ColorPickerPopUpElement(VisualElement parent) : base(parent)
        {
            SetTitle("Select Color");
        }

        public void Initialize(List<Color> colors)
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;
            Content.Add(container);
            foreach (Color color in colors)
            {
                ColorSwatch swatch = new(color);
                container.Add(swatch);
                swatch.OnSelected += OnSwatchSelected;
            }
        }

        void OnSwatchSelected(Color c)
        {
            OnColorSelected?.Invoke(c);
        }
    }
}