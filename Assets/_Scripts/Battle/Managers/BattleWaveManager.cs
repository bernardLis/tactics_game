using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;

public class BattleWaveManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    Battle _selectedBattle;

    int _currentWaveIndex;

    [SerializeField] BattleOpponentPortal[] _opponentPortals;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _selectedBattle = _gameManager.CurrentBattle;
        BattleIntroCameraManager.Instance.OnIntroFinished += Initialize;
    }

    public void Initialize()
    {
        StartCoroutine(HandleWaves());
    }

    IEnumerator HandleWaves()
    {
        while (true)
        {
            StartWave(_selectedBattle.Waves[_currentWaveIndex]);
            _currentWaveIndex++;

            if (_currentWaveIndex == _selectedBattle.Waves.Count - 3)
                _selectedBattle.CreateWaves();

            // how long do you need to wait for next wave
            yield return new WaitForSeconds(_selectedBattle.Waves[_currentWaveIndex].StartTime - Time.time);
        }
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
