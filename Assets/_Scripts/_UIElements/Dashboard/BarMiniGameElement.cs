using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.InputSystem;

public class BarMiniGameElement : VisualElement
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;

    VisualElement _root;

    VisualElement _bar;
    VisualElement _cursor;
    MyButton _hitButton;
    Label _spaceBarTooltip;

    bool _gameStarted;

    string _cursorMovementTween = "cursorMovementTween";

    const string _ussCommonTextSecondary = "common__text-secondary";

    const string _ussClassName = "bar-mini-game__";
    const string _ussBar = _ussClassName + "bar";
    const string _ussCursor = _ussClassName + "cursor";
    const string _ussGhostCursor = _ussClassName + "ghost-cursor";
    const string _ussHitButton = _ussClassName + "hit-button";
    const string _ussBestZone = _ussClassName + "best-zone";
    const string _ussWorseZone = _ussClassName + "worse-zone";
    const string _ussWorstZone = _ussClassName + "worst-zone";

    public event Action<int> OnHit;
    public BarMiniGameElement()
    {
        _gameManager = GameManager.Instance;
        _dashboardManager = DashboardManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BarMiniGameStyles);
        if (ss != null)
            styleSheets.Add(ss);

        if (_dashboardManager == null)
            Debug.LogError($"No desk manager in bar mini game element");
        _root = _dashboardManager.Root;

        _bar = new();
        _bar.AddToClassList(_ussBar);
        Add(_bar);

        VisualElement worstZone = new();
        VisualElement worseZone = new();
        VisualElement bestZone = new();
        worstZone.AddToClassList(_ussWorstZone);
        worseZone.AddToClassList(_ussWorseZone);
        bestZone.AddToClassList(_ussBestZone);
        _bar.Add(worstZone);
        _bar.Add(worseZone);
        _bar.Add(bestZone);

        _cursor = new();
        _cursor.AddToClassList(_ussCursor);
        _bar.Add(_cursor);

        _hitButton = new("Start negotiation", _ussHitButton, Hit);
        Add(_hitButton);
    }

    public void StartMiniGame()
    {
        _gameStarted = true;
        _hitButton.UpdateButtonText("Hit");
        _dashboardManager.PlayerInput.actions["Space"].performed += HitInput;
        _spaceBarTooltip = new Label("Space Bar works as well.");
        _spaceBarTooltip.AddToClassList(_ussCommonTextSecondary);
        Add(_spaceBarTooltip);

        // move cursor from side to side
        _cursor.style.left = 0;
        DOTween.To(() => _cursor.style.left.value.value,
                x => _cursor.style.left = Length.Percent(x), 96, 3f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Unset)
                .SetId(_cursorMovementTween);
    }

    public void StopGame()
    {
        _dashboardManager.PlayerInput.actions["Space"].performed -= HitInput;

        Remove(_spaceBarTooltip);
        DOTween.Kill(_cursorMovementTween);
        _hitButton.SetEnabled(false);
        _hitButton.UpdateButtonText("Negotiations are over");
    }

    void HitInput(InputAction.CallbackContext ctx) { Hit(); }

    void Hit()
    {
        if (!_gameStarted)
        {
            StartMiniGame();
            return;
        }
        float hitValue = _cursor.style.left.value.value;

        VisualElement ghostCursor = new();
        ghostCursor.AddToClassList(_ussGhostCursor);
        _bar.Add(ghostCursor);
        ghostCursor.style.left = Length.Percent(hitValue);

        if (hitValue > 47.5f && 52.5f > hitValue)
        {
            Helpers.DisplayTextOnElement(_root, _bar, "Bulls Eye!", new Color(0.63f, 0.11f, 0.11f));
            OnHit?.Invoke(3);
            return;
        }

        if (hitValue > 40 && 60 > hitValue)
        {
            Helpers.DisplayTextOnElement(_root, _bar, "Good!", new Color(1f, 0.53f, 0f));
            OnHit?.Invoke(2);
            return;
        }

        if (hitValue > 30 && 70 > hitValue)
        {
            Helpers.DisplayTextOnElement(_root, _bar, "Decent!", new Color(1f, 0.69f, 0f));
            OnHit?.Invoke(1);
            return;
        }

        Helpers.DisplayTextOnElement(_root, _bar, "Miss!", Color.white);
        OnHit?.Invoke(0);
    }

}
