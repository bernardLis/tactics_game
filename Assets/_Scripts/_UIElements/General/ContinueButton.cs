using System;
using DG.Tweening;
using UnityEngine.UIElements;

namespace Lis
{
    public class ContinueButton : MyButton
    {
        const string _ussCommonContinueButton = "common__continue-button";

        public ContinueButton(string buttonText = "Continue", string className = _ussCommonContinueButton, Action callback = null)
            : base(buttonText, className, callback)
        {
            style.opacity = 0;
            DOTween.To(x => style.opacity = x, 0, 1, 0.5f).SetUpdate(true);
        

            RegisterCallback<ClickEvent>(evt =>
            {
                SetEnabled(false);
            });

        }
    }
}
