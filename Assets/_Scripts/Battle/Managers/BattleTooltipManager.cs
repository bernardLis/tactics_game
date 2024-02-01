using System;
using System.Collections;






using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Lis
{
    public class BattleTooltipManager : Singleton<BattleTooltipManager>
    {
        GameManager _gameManager;
        PlayerInput _playerInput;
        BattleManager _battleManager;

        VisualElement _root;
        VisualElement _tooltipCardContainer;

        VisualElement _tileSecureBarContainer;
        VisualElement _gameInfoContainer;
        VisualElement _entityInfoContainer; // shows mouse hover info 
        EntityInfoElement _entityInfoElement;

        readonly string _gameInfoTweenID = "_gameInfoContainer";

        VisualElement _currentTooltip;
        public GameObject CurrentTooltipDisplayer { get; private set; }

        public BattleEntity CurrentEntityInfo { get; private set; }


        ResourceBarElement _tileSecureBar;

        bool _isBossHealthBarShown;

        public event Action OnTooltipHidden;
        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _battleManager.OnBattleFinalized += OnBattleFinalized;

            _root = GetComponent<UIDocument>().rootVisualElement;
            _tooltipCardContainer = _root.Q<VisualElement>("tooltipCardContainer");
            _entityInfoContainer = _root.Q<VisualElement>("entityInfoContainer");
            _gameInfoContainer = _root.Q<VisualElement>("gameInfoContainer");
            _tileSecureBarContainer = _root.Q<VisualElement>("tileSecureBarContainer");

            SetUpEntityInfo();
            SetUpTileSecureBar();
        }

        /* INPUT */
        void OnEnable()
        {
            if (_gameManager == null)
                _gameManager = GameManager.Instance;

            _playerInput = _gameManager.GetComponent<PlayerInput>();
            _playerInput.SwitchCurrentActionMap("Battle");
            UnsubscribeInputActions();
            SubscribeInputActions();
        }

        void OnDisable()
        {
            if (_playerInput == null) return;

            UnsubscribeInputActions();
        }

        void OnDestroy()
        {
            if (_playerInput == null) return;

            UnsubscribeInputActions();
        }

        void SubscribeInputActions()
        {
            _playerInput.actions["RightMouseClick"].performed += RightMouseClick;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["RightMouseClick"].performed -= RightMouseClick;
        }

        void RightMouseClick(InputAction.CallbackContext ctx)
        {
            if (CurrentTooltipDisplayer == null) return;
            HideTooltip();
        }

        void OnBattleFinalized()
        {
            HideEntityInfo();
            HideTooltip();
        }

        void SetUpEntityInfo()
        {
            _entityInfoElement = new EntityInfoElement(_battleManager.BattleHero); // placeholder entity
            _entityInfoElement.style.display = DisplayStyle.None;
            _entityInfoContainer.Add(_entityInfoElement);
        }

        void SetUpTileSecureBar()
        {
            IntVariable current = ScriptableObject.CreateInstance<IntVariable>();
            current.SetValue(0);
            IntVariable total = ScriptableObject.CreateInstance<IntVariable>();
            total.SetValue(0);

            _tileSecureBar = new ResourceBarElement(Color.white, "Securing", current, total);
            _tileSecureBar.HideText();
            Label label = new("Securing");
            label.style.fontSize = 24;
            label.style.color = Color.magenta;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.position = Position.Absolute;
            _tileSecureBar.Add(label);

            _tileSecureBar.style.minWidth = 500;
            _tileSecureBarContainer.Add(_tileSecureBar);
            _tileSecureBarContainer.style.display = DisplayStyle.None;
        }

        public void ShowTileSecureBar(IntVariable current, IntVariable total)
        {
            _tileSecureBar.UpdateTrackedVariables(current, total);
            _tileSecureBarContainer.style.display = DisplayStyle.Flex;
        }

        public void HideTileSecureBar()
        {
            if (_tileSecureBarContainer == null) return;
            _tileSecureBarContainer.style.display = DisplayStyle.None;
        }

        public void ShowEntityInfo(BattleEntity entity)
        {
            if (entity.IsDead) return;
            if (_isBossHealthBarShown) return;

            CurrentEntityInfo = entity;
            _entityInfoElement.UpdateEntityInfo(entity);
            _entityInfoElement.style.display = DisplayStyle.Flex;
        }

        public void HideEntityInfo()
        {
            if (_isBossHealthBarShown) return;

            _entityInfoElement.style.display = DisplayStyle.None;
        }

        public void ShowBossHealthBar(BattleBoss boss)
        {
            HideEntityInfo();

            CurrentEntityInfo = boss;
            BossInfoElement info = new(boss);
            _entityInfoContainer.Add(info);
            _entityInfoContainer.style.display = DisplayStyle.Flex;

            _isBossHealthBarShown = true;
        }

        public void ShowGameInfo(string text, float duration)
        {
            ShowGameInfo(text);
            Invoke(nameof(HideGameInfo), duration);
        }

        public void ShowGameInfo(string text)
        {
            _gameInfoContainer.Clear();
            Label txt = new(text);
            _gameInfoContainer.Add(txt);

            _gameInfoContainer.style.display = DisplayStyle.Flex;
            _gameInfoContainer.style.opacity = 0;
            DOTween.Kill(_gameInfoTweenID);
            DOTween.To(x => _gameInfoContainer.style.opacity = x, 0, 1, 0.5f)
                .SetEase(Ease.InOutSine);
        }

        public void HideGameInfo()
        {
            DOTween.To(x => _gameInfoContainer.style.opacity = x, 1, 0, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetId(_gameInfoTweenID)
                .OnComplete(() =>
                {
                    _gameInfoContainer.style.display = DisplayStyle.None;
                    _gameInfoContainer.Clear();
                });

        }

        /* TOOLTIP CARD */
        public void ShowTooltip(VisualElement el, GameObject go)
        {
            if (CurrentTooltipDisplayer == go) return;
            if (_currentTooltip != null) _tooltipCardContainer.Remove(_currentTooltip);

            bool tooltipAnimation = _currentTooltip == null;
            CurrentTooltipDisplayer = go;
            _currentTooltip = el;

            StartCoroutine(ShowTooltipCoroutine(tooltipAnimation));
        }

        IEnumerator ShowTooltipCoroutine(bool isAnimated)
        {
            yield return new WaitForSeconds(0.1f);
            _tooltipCardContainer.Add(_currentTooltip);

            if (!isAnimated) yield break;

            _tooltipCardContainer.style.left = -_currentTooltip.resolvedStyle.width;
            _tooltipCardContainer.style.visibility = Visibility.Visible;
            DOTween.To(x => _tooltipCardContainer.style.left = x, -_currentTooltip.resolvedStyle.width, 0, 0.5f)
                .SetEase(Ease.InOutSine);
        }

        public void HideTooltip()
        {
            if (_currentTooltip == null) return;
            CurrentTooltipDisplayer = null;

            DOTween.To(x => _tooltipCardContainer.style.left = x, 0, -_currentTooltip.worldBound.width, 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if (_currentTooltip == null) return;
                    _currentTooltip.RemoveFromHierarchy();
                    _currentTooltip = null;

                    OnTooltipHidden?.Invoke();
                });
        }
    }
}
