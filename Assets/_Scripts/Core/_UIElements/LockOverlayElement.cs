using DG.Tweening;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class LockOverlayElement : ElementWithTooltip
    {
        private const string _ussCommonTextPrimary = "common__text-primary";
        private const string _ussMain = "common__lock-overlay-main";
        private const string _ussLockIcon = "common__lock-overlay-icon";
        private const string _ussLockIconUnlocked = "common__lock-overlay-icon-unlocked";
        private readonly AudioManager _audioManager;

        private readonly GameManager _gameManager;

        private readonly VisualElement _localTooltip;
        private readonly Label _lockIcon;

        public LockOverlayElement(VisualElement tooltip)
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            StyleSheet commonStyles = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null) styleSheets.Add(commonStyles);
            AddToClassList(_ussCommonTextPrimary);
            AddToClassList(_ussMain);

            _localTooltip = tooltip;

            _lockIcon = new();
            _lockIcon.AddToClassList(_ussLockIcon);
            Add(_lockIcon);

            RegisterCallback<PointerEnterEvent>(ShakeIcon);
        }

        private void ShakeIcon(PointerEnterEvent evt)
        {
            _audioManager.PlayUI("Lock OnHover");
            DOTween.Shake(() => _lockIcon.transform.position, x => _lockIcon.transform.position = x,
                0.5f, 10f).SetUpdate(true);
        }

        public void Unlock()
        {
            _audioManager.PlayUI("Lock Unlock");

            DOTween.Shake(
                    () => _lockIcon.transform.position, x => _lockIcon.transform.position = x, 1f, 10f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _lockIcon.RemoveFromClassList(_ussLockIcon);
                    _lockIcon.AddToClassList(_ussLockIconUnlocked);

                    DOTween.To(x => style.opacity = x, 0, 1, 0.5f)
                        .SetDelay(0.5f)
                        .SetUpdate(true)
                        .OnComplete(() => RemoveFromHierarchy());
                });
        }

        protected override void DisplayTooltip()
        {
            _tooltip = new(this, _localTooltip);
            base.DisplayTooltip();
        }
    }
}