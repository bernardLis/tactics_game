using DG.Tweening;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class GoldElement : ChangingValueElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "gold-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIcon = _ussClassName + "icon";
        const string _ussValue = _ussClassName + "value";

        readonly GameManager _gameManager;
        readonly VisualElement _icon;

        Tween _shakeTween;

        public GoldElement(int amount)
        {
            _gameManager = GameManager.Instance;

            StyleSheet commonStyles = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null) styleSheets.Add(commonStyles);
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.GoldElementStyles);
            if (ss != null) styleSheets.Add(ss);

            Amount = 0;

            AddToClassList(_ussMain);

            _icon = new();
            _icon.AddToClassList(_ussIcon);
            _icon.style.backgroundImage = new(_gameManager.GameDatabase.GetCoinSprite(amount));
            Add(_icon);

            _text = new();
            _text.AddToClassList(_ussCommonTextPrimary);
            _text.AddToClassList(_ussValue);
            _text.text = Amount.ToString();
            Add(_text);

            ChangeAmount(amount);
        }

        public void MakeItBig()
        {
            _icon.style.width = 50;
            _icon.style.height = 50;
            _text.style.fontSize = 32;
        }

        public override void ChangeAmount(int newValue)
        {
            base.ChangeAmount(newValue);
            if (_shakeTween.IsActive()) return;

            _shakeTween = DOTween.Shake(() => _icon.transform.position, x => _icon.transform.position = x, 0.5f, 5f)
                .SetUpdate(true);
        }

        protected override void NumberAnimation()
        {
            base.NumberAnimation();
            _icon.style.backgroundImage =
                new StyleBackground(_gameManager.GameDatabase.GetCoinSprite(_currentlyDisplayedAmount));
        }
    }
}