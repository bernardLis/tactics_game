using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class ColorSwatch : VisualElement
    {
        readonly Color _color;

        public event Action<Color> OnSelected;

        public ColorSwatch(Color color)
        {
            _color = color;
            style.backgroundColor = color;
            AddToClassList("common__color-swatch");

            RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            OnSelected?.Invoke(_color);
        }
    }
}