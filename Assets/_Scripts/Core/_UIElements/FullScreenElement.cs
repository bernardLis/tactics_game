using System;
using DG.Tweening;
using Lis.Battle;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class FullScreenElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";
        protected const string USSCommonButton = "common__button";
        protected const string USSCommonHorizontalSpacer = "common__horizontal-spacer";
        const string _ussCommonFullScreenMain = "common__full-screen-main";
        const string _ussCommonFullScreenTitle = "common__full-screen-title";
        const string _ussCommonFullScreenContent = "common__full-screen-content";
        const string _ussCommonFullScreenUtilityContainer = "common__full-screen-utility-container";

        readonly Label _titleLabel;
        protected readonly BattleManager BattleManager;
        protected readonly VisualElement Content;
        protected readonly VisualElement UtilityContainer;

        bool _isNavigationDisabled;

        VisualElement _root;
        protected ContinueButton ContinueButton;

        protected GameManager GameManager;

        protected FullScreenElement()
        {
            GameManager = GameManager.Instance;
            BattleManager = BattleManager.Instance;

            StyleSheet commonStyles = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null) styleSheets.Add(commonStyles);

            ResolveRoot();

            GameManager.OpenFullScreens.Add(this);
            if (BattleManager != null) BattleManager.PauseGame();

            AddToClassList(_ussCommonFullScreenMain);
            AddToClassList(_ussCommonTextPrimary);

            _titleLabel = new("");
            _titleLabel.AddToClassList(_ussCommonFullScreenTitle);
            Add(_titleLabel);

            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            Add(spacer);

            Content = new();
            Content.AddToClassList(_ussCommonFullScreenContent);
            Add(Content);

            UtilityContainer = new();
            UtilityContainer.AddToClassList(_ussCommonFullScreenUtilityContainer);
            Add(UtilityContainer);

            focusable = true;
            Focus();

            style.opacity = 0;
            DOTween.To(x => style.opacity = x, style.opacity.value, 1, 0.5f)
                .SetUpdate(true)
                .OnComplete(EnableNavigation);
        }

        public event Action OnHide;


        void ResolveRoot()
        {
            _root = GameManager.Root;
            if (BattleManager != null) _root = BattleManager.Root;

            _root.Add(this);
        }

        protected void SetTitle(string txt)
        {
            _titleLabel.text = txt;
            DOTween.To(x => _titleLabel.style.opacity = x, 0, 1, 0.5f)
                .SetUpdate(true);
        }

        void EnableNavigation()
        {
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<KeyDownEvent>(OnKeyDown); // TODO: full screen management vs menu opening and closing
        }

        protected void DisableNavigation()
        {
            _isNavigationDisabled = true;
            UnregisterCallback<PointerDownEvent>(OnPointerDown);
            UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (_isNavigationDisabled) return;
            if (evt.button != 1) return; // only right mouse click

            Hide();
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            if (_isNavigationDisabled) return;
            if (evt.keyCode != KeyCode.Escape) return;

            Hide();
        }

        protected void AddContinueButton()
        {
            ContinueButton = new(callback: Hide);
            UtilityContainer.Add(ContinueButton);
        }

        public void Hide()
        {
            VisualElement tt = _root.Q<VisualElement>("tooltipContainer");
            if (tt != null) tt.style.display = DisplayStyle.None;

            DOTween.To(x => style.opacity = x, style.opacity.value, 0, 0.5f)
                .SetUpdate(true);
            DOTween.To(x => Content.style.opacity = x, 1, 0, 0.5f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    OnHide?.Invoke();

                    GameManager.OpenFullScreens.Remove(this);
                    if (GameManager.OpenFullScreens.Count > 0) GameManager.OpenFullScreens[^1].Focus();
                    else if (BattleManager != null) BattleManager.ResumeGame();

                    SetEnabled(false);
                    RemoveFromHierarchy();
                });
        }
    }
}