using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleWaveManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    Battle _selectedBattle;


    [SerializeField] BattleOpponentPortal[] _opponentPortals;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _selectedBattle = _gameManager.SelectedBattle;
    }

    public void Initialize()
    {
        Debug.Log($"handle waves {_selectedBattle.Waves.Count}");
        StartCoroutine(HandleWaves());
    }

    IEnumerator HandleWaves()
    {
        while (true)
        {
            foreach (BattleWave wave in _selectedBattle.Waves)
                CheckWave(wave);
            yield return new WaitForSeconds(1);
        }
    }

    void CheckWave(BattleWave wave)
    {
        if (wave.IsStarted) return;
        if (wave.StartTime <= Time.time)
            StartWave(wave);
    }

    void StartWave(BattleWave wave)
    {
        Debug.Log($"start wave {wave.Element}");
        wave.IsStarted = true;
        foreach (BattleOpponentPortal portal in _opponentPortals)
        {
            if (portal.Element == wave.Element)
            {
                portal.InitializeWave(wave);
                return;
            }
        }
    }
}
