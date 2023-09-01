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

    public int CurrentWaveIndex { get; private set; }

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

        Debug.Log($"Initializing battle wave manager");

        StartCoroutine(HandleWaves());
    }

    IEnumerator HandleWaves()
    {
        while (true)
        {
            Debug.Log($"Starting wave {CurrentWaveIndex}");

            StartWave(_selectedBattle.Waves[CurrentWaveIndex]);
            CurrentWaveIndex++;

            if (CurrentWaveIndex == _selectedBattle.Waves.Count - 3)
                _selectedBattle.CreateWaves();

            float nextWaveIn = _selectedBattle.Waves[CurrentWaveIndex].StartTime
                    - _battleManager.GetTime();
            yield return new WaitForSeconds(nextWaveIn);
        }
    }

    void StartWave(BattleWave wave)
    {
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
