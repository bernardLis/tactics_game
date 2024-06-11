using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Boss;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class TooltipManager : Singleton<TooltipManager>
    {
        private readonly Queue<VisualElement> _gameInfoQueue = new();

        private readonly string _gameInfoTweenID = "_gameInfoContainer";
        private BattleManager _battleManager;

        private VisualElement _currentTooltip;
        private VisualElement _entityInfoContainer; // shows mouse hover info 
        private EntityInfoElement _entityInfoElement;
        private FightManager _fightManager;

        private VisualElement _gameInfoContainer;
        private GameManager _gameManager;

        private bool _isBossHealthBarShown;
        private PlayerInput _playerInput;

        private VisualElement _root;


        private ResourceBarElement _tileSecureBar;
        private VisualElement _tooltipCardContainer;
        public GameObject CurrentTooltipDisplayer { get; private set; }

        public UnitController CurrentEntityInfo { get; private set; }

        /* INPUT */
        private void OnEnable()
        {
            if (_gameManager == null)
                _gameManager = GameManager.Instance;

            _playerInput = _gameManager.GetComponent<PlayerInput>();
            _playerInput.SwitchCurrentActionMap("Battle");
            UnsubscribeInputActions();
            SubscribeInputActions();
        }

        private void OnDisable()
        {
            if (_playerInput == null) return;

            UnsubscribeInputActions();
        }

        private void OnDestroy()
        {
            if (_playerInput == null) return;

            UnsubscribeInputActions();
        }

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _battleManager.OnBattleFinalized += OnBattleFinalized;

            _fightManager = GetComponent<FightManager>();
            _fightManager.OnFightStarted += OnFightStarted;
            _fightManager.OnFightEnded += OnFightEnded;
            _root = GetComponent<UIDocument>().rootVisualElement;
            _tooltipCardContainer = _root.Q<VisualElement>("tooltipCardContainer");
            _entityInfoContainer = _root.Q<VisualElement>("entityInfoContainer");
            _gameInfoContainer = _root.Q<VisualElement>("gameInfoContainer");

            SetUpEntityInfo();
            StartCoroutine(ShowGameInfoCoroutine());
        }

        private void OnFightStarted()
        {
            DisplayGameInfo(new Label("Fight started!"));
            if (CurrentTooltipDisplayer == null) return;
            HideTooltip();
        }

        private void OnFightEnded()
        {
            VisualElement el = new();
            el.style.alignItems = Align.Center;
            el.Add(new Label("Fight won!"));
            if (_fightManager.LastFight != null)
            {
                VisualElement container = new();
                container.style.flexDirection = FlexDirection.Row;
                Label l = new("Reward: ");
                l.AddToClassList("common__text-primary");
                container.Add(l);
                container.Add(new GoldElement(_fightManager.LastFight.GetGoldReward()));
                el.Add(container);
            }

            DisplayGameInfo(el);
        }

        private void SubscribeInputActions()
        {
            _playerInput.actions["RightMouseClick"].performed += RightMouseClick;
        }

        private void UnsubscribeInputActions()
        {
            _playerInput.actions["RightMouseClick"].performed -= RightMouseClick;
        }

        private void RightMouseClick(InputAction.CallbackContext ctx)
        {
            if (CurrentTooltipDisplayer == null) return;
            HideTooltip();
        }

        private void OnBattleFinalized()
        {
            HideEntityInfo();
            HideTooltip();
        }

        private void SetUpEntityInfo()
        {
            _entityInfoElement = new(default); // placeholder unit
            _entityInfoElement.style.display = DisplayStyle.None;
            _entityInfoContainer.Add(_entityInfoElement);
        }

        public void ShowEntityInfo(UnitController unitController)
        {
            if (unitController.IsDead) return;
            if (_isBossHealthBarShown) return;

            CurrentEntityInfo = unitController;
            _entityInfoElement.UpdateEntityInfo(unitController);
            _entityInfoElement.style.display = DisplayStyle.Flex;
        }

        public void HideEntityInfo()
        {
            if (_isBossHealthBarShown) return;

            _entityInfoElement.style.display = DisplayStyle.None;
        }

        public void ShowBossHealthBar(BossController bossController)
        {
            HideEntityInfo();

            CurrentEntityInfo = bossController;
            BossInfoElement info = new(bossController);
            _entityInfoContainer.Add(info);
            _entityInfoContainer.style.display = DisplayStyle.Flex;

            _isBossHealthBarShown = true;
        }

        public void DisplayGameInfo(VisualElement el)
        {
            _gameInfoQueue.Enqueue(el);
        }

        private IEnumerator ShowGameInfoCoroutine()
        {
            while (this != null)
            {
                while (_gameInfoQueue.Count > 0)
                {
                    int waitTime = 3;
                    VisualElement el = _gameInfoQueue.Peek();
                    if (el is Label label)
                        if (label.text.Length < 3)
                            waitTime = 1;

                    ShowGameInfo(_gameInfoQueue.Dequeue());
                    yield return new WaitForSeconds(waitTime);
                    HideGameInfo();
                }

                yield return new WaitForSeconds(1);
            }
        }

        private void ShowGameInfo(VisualElement el)
        {
            if (_gameInfoContainer == null) return;
            _gameInfoContainer.Clear();
            _gameInfoContainer.Add(el);

            _gameInfoContainer.style.display = DisplayStyle.Flex;
            _gameInfoContainer.style.opacity = 0;
            DOTween.Kill(_gameInfoTweenID);
            DOTween.To(x => _gameInfoContainer.style.opacity = x, 0, 1, 0.5f)
                .SetEase(Ease.InOutSine);
        }

        private void HideGameInfo()
        {
            if (_gameInfoContainer == null) return;
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

        private IEnumerator ShowTooltipCoroutine(bool isAnimated)
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
                });
        }
    }
}