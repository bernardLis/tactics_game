using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BattleLivesManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    Spire _spire;

    MMF_Player _feelPlayer;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _feelPlayer = GetComponent<MMF_Player>();

        _spire = _gameManager.SelectedBattle.Spire;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision: {collision.gameObject.name}");
        if (collision.gameObject.TryGetComponent<BattleEntity>(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return;
            if (_spire == null) _spire = _gameManager.SelectedBattle.Spire;

            _spire.StoreyLives.CurrentLives.ApplyChange(-1);
            DisplayFloatingText($"Lives: {_spire.StoreyLives.CurrentLives.Value}", Color.white);
            StartCoroutine(battleEntity.Die(hasPickup: false));

            if (_spire.StoreyLives.CurrentLives.Value <= 0)
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
