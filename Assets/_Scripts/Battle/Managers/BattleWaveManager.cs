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

    bool _isInitialized;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _selectedBattle = _gameManager.CurrentBattle;

        if (BattleIntroManager.Instance != null)
            BattleIntroManager.Instance.OnIntroFinished += Initialize;
    }

    public void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        StartCoroutine(HandleWaves());
    }

    IEnumerator HandleWaves()
    {
        Debug.Log($"handle waves");
        while (true)
        {
            StartWave(_selectedBattle.Waves[_currentWaveIndex]);
            _currentWaveIndex++;
            Debug.Log($"current wave index {_currentWaveIndex}");

            if (_currentWaveIndex == _selectedBattle.Waves.Count - 3)
                _selectedBattle.CreateWaves();

            Debug.Log($"start time: {_selectedBattle.Waves[_currentWaveIndex].StartTime}");
            Debug.Log($"current time: {_battleManager.GetTime()}");
            Debug.Log($"wait for: {_selectedBattle.Waves[_currentWaveIndex].StartTime - _battleManager.GetTime()}");
            // how long do you need to wait for next wave
            yield return new WaitForSeconds(_selectedBattle.Waves[_currentWaveIndex].StartTime - _battleManager.GetTime());
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
