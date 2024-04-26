using System;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class RerollButton : MyButton
    {
        const string _ussCommonRerollButton = "common__reroll-button";
        const string _ussCommonRerollIcon = "common__reroll-icon";
        const string _ussCommonRerollIconHover = "common__reroll-icon-hover";

        public RerollButton(string buttonText = null, string className = _ussCommonRerollButton, Action callback = null)
            : base(buttonText, className, callback)
        {
            VisualElement rerollIcon = new();
            rerollIcon.AddToClassList(_ussCommonRerollIcon);
            Add(rerollIcon);

            Remove(_text);
            style.paddingBottom = 0;
            style.paddingTop = 0;
            style.paddingLeft = 0;
            style.paddingRight = 0;


            RegisterCallback<PointerEnterEvent>(evt => rerollIcon.AddToClassList(_ussCommonRerollIconHover));
            RegisterCallback<PointerLeaveEvent>(evt => rerollIcon.RemoveFromClassList(_ussCommonRerollIconHover));
        }
    }
}