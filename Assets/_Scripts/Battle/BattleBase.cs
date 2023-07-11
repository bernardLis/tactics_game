using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.EventSystems;

public class BattleBase : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    MMF_Player _feelPlayer;


    public int Lives { get; private set; }

    void Start()
    {
        _tooltipManager = BattleTooltipManager.Instance;
        _feelPlayer = GetComponent<MMF_Player>();
        Lives = 10;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BattleEntity>(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return;

            Lives--;
            DisplayFloatingText($"Lives: {Lives}", Color.white);

            StartCoroutine(battleEntity.Die(hasPickup: false));

            if (Lives <= 0)
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
        _tooltipManager.ShowInfo($"{Lives} lives left");
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
