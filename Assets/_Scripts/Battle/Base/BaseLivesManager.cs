using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BaseLivesManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    Base _base;

    MMF_Player _feelPlayer;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _feelPlayer = GetComponent<MMF_Player>();

        _base = _gameManager.SelectedBattle.Base;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BattleEntity>(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return;

            _base.LivesUpgrade.CurrentLives.ApplyChange(-1);
            DisplayFloatingText($"Lives: {_base.LivesUpgrade.CurrentLives.Value}", Color.white);
            StartCoroutine(battleEntity.Die(hasPickup: false));

            if (_base.LivesUpgrade.CurrentLives.Value <= 0)
                _battleManager.LoseBattle();
        }
    }

    public void DisplayFloatingText(string text, Color color)
    {
        if (_feelPlayer == null) return;
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        _feelPlayer.PlayFeedbacks(transform.position + Vector3.forward);
    }
}
