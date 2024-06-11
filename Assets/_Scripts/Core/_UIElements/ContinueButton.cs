using System;
using DG.Tweening;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ContinueButton : MyButton
    {
        const string _ussCommonButton = "common__button";

        public ContinueButton(string buttonText = "Continue", string className = _ussCommonButton,
            Action callback = null)
            : base(buttonText, className, callback)
        {
            style.opacity = 0;
            DOTween.To(x => style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

            style.alignSelf = Align.Center;
            RegisterCallback<ClickEvent>(_ => { SetEnabled(false); });
        }
    }
}