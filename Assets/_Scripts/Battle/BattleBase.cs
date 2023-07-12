using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MoreMountains.Feedbacks;
using UnityEngine.EventSystems;

public class BattleBase : Singleton<BattleBase>, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Base _base;

    MMF_Player _feelPlayer;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _tooltipManager = BattleTooltipManager.Instance;
        _feelPlayer = GetComponent<MMF_Player>();

        _base = _gameManager.SelectedBattle.Base;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BattleEntity>(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return;

            _base.Lives.ApplyChange(-1);
            DisplayFloatingText($"Lives: {_base.Lives.Value}", Color.white);
            StartCoroutine(battleEntity.Die(hasPickup: false));

            if (_base.Lives.Value <= 0)
                _battleManager.LoseBattle();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"click click");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"{_base.Lives.Value} lives left");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void DisplayFloatingText(string text, Color color)
    {
        if (_feelPlayer == null) return;
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        _feelPlayer.PlayFeedbacks(transform.position);
    }
}
