using DG.Tweening;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class LockOverlayElement : ElementWithTooltip
    {
        const string _ussCommonTextPrimary = "common__text-primary";
        const string _ussMain = "common__lock-overlay-main";
        const string _ussLockIcon = "common__lock-overlay-icon";
        const string _ussLockIconUnlocked = "common__lock-overlay-icon-unlocked";
        readonly AudioManager _audioManager;

        readonly GameManager _gameManager;

        readonly VisualElement _localTooltip;
        readonly Label _lockIcon;

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

        void ShakeIcon(PointerEnterEvent evt)
        {
            _audioManager.PlaySound("Lock OnHover");
            DOTween.Shake(() => _lockIcon.transform.position, x => _lockIcon.transform.position = x,
                0.5f, 10f).SetUpdate(true);
        }

        public void Unlock()
        {
            _audioManager.PlaySound("Lock Unlock");

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