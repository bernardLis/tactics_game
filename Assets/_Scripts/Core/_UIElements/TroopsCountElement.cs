using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class TroopsCountElement : ElementWithTooltip
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        readonly VisualElement _animationContainer;
        readonly VisualElement _countContainer;

        readonly int _fontSize;
        readonly GameManager _gameManager;

        public TroopsCountElement(string text, int fontSize = 0)
        {
            _gameManager = GameManager.Instance;
            StyleSheet commonStyles = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null)
                styleSheets.Add(commonStyles);

            style.flexDirection = FlexDirection.Row;
            AddToClassList(_ussCommonTextPrimary);

            if (fontSize != 0)
                _fontSize = fontSize;

            _animationContainer = new();
            _animationContainer.style.width = 30;
            _animationContainer.style.height = 30;

            var animationSprites = _gameManager.GameDatabase.TroopsElementAnimationSprites;
            AnimationElement el = new(animationSprites, 100, true);
            el.PlayAnimation();
            _animationContainer.Add(el);
            Add(_animationContainer);

            _countContainer = new();
            Add(_countContainer);
            UpdateCountContainer(text, Color.white);
        }

        public void UpdateCountContainer(string text, Color color)
        {
            _countContainer.Clear();
            Label l = new();
            l.style.color = color;
            _countContainer.Add(l);
            _countContainer.style.justifyContent = Justify.Center;
            l.text = text;
            l.style.marginLeft = 10;
            l.style.marginBottom = 0;
            l.style.paddingBottom = 0;

            if (_fontSize != 0)
                l.style.fontSize = _fontSize;
        }
    }
}