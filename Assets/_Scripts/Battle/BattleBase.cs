using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BattleBase : MonoBehaviour
{
    MMF_Player _feelPlayer;

    public int Lives { get; private set; }

    void Start()
    {
        Lives = 10;
        _feelPlayer = GetComponent<MMF_Player>();

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BattleEntity>(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return;

            Lives--;
            DisplayFloatingText($"Lives: {Lives}", Color.white);

            StartCoroutine(battleEntity.Die(hasPickup: false));
        }
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
