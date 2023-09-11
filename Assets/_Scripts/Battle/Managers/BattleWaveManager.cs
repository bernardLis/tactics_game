using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class BattleWaveManager : Singleton<BattleWaveManager>
{
    GameManager _gameManager;
    BattleManager _battleManager;

    Battle _selectedBattle;

    [SerializeField] List<BattleWave> _waves = new();

    public int CurrentDifficulty { get; private set; }
    public int CurrentWaveIndex { get; private set; }

    public List<BattleOpponentPortal> OpponentPortals = new();

    bool _isInitialized;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _selectedBattle = _gameManager.CurrentBattle;

        if (BattleIntroManager.Instance != null)
            BattleIntroManager.Instance.OnIntroFinished += Initialize;
    }

    public void AddPortal(BattleOpponentPortal portal)
    {
        if (OpponentPortals.Contains(portal)) return;
        OpponentPortals.Add(portal);

        portal.OnPortalClosed += HandleWaveSpawned;
    }

    void HandleWaveSpawned(BattleOpponentPortal portal)
    {
        // portal receives new wave right after the previous one is spawned
        BattleWave bw = CreateWave(portal.Element);
        portal.GetWave(bw);
    }

    public void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        Debug.Log($"Initializing battle wave manager");

        if (OpponentPortals.Count == 0)
            foreach (var item in FindObjectsOfType<BattleOpponentPortal>())
                AddPortal(item);

        CurrentDifficulty = 1;
        CurrentWaveIndex = 0;

        // so, the first wave is always the opposite element of the player's element
        Element firstElement = _gameManager.PlayerHero.Element.StrongAgainst;
        CreateWave(firstElement);

        Element lastElement = _gameManager.PlayerHero.Element.WeakAgainst;
        List<Element> availableElements = new(_gameManager.EntityDatabase.GetAllElements());
        availableElements.Remove(firstElement);
        availableElements.Remove(lastElement);

        foreach (Element el in availableElements)
            CreateWave(el);

        CreateWave(lastElement);

        foreach (BattleWave wave in _waves)
            foreach (BattleOpponentPortal portal in OpponentPortals)
                if (portal.Element == wave.Element)
                    portal.GetWave(wave);
    }

    BattleWave CreateWave(Element element)
    {
        BattleWave bw = ScriptableObject.CreateInstance<BattleWave>();
        float startTime = GetWaveStartTime(element);
        bw.CreateWave(element, CurrentDifficulty, startTime);

        _waves.Add(bw);
        CurrentWaveIndex++;
        if (CurrentWaveIndex % 4 == 0)
            CurrentDifficulty++;

        return bw;
    }


    public float GetWaveStartTime(Element element)
    {
        if (CurrentWaveIndex == 0) return _battleManager.GetTime(); // first wave should spawn right away

        // wave starts in the "middle" of the previous wave
        BattleWave previousWave = _waves[CurrentWaveIndex - 1];
        float waveSpawnFactor = 1f - CurrentWaveIndex * 0.08f; // how quickly waves spawn after each other

        waveSpawnFactor = Mathf.Clamp(waveSpawnFactor, 0.1f, 1f);
        float startTime = previousWave.StartTime + (previousWave.GetPlannedEndTime() - previousWave.StartTime) * waveSpawnFactor;

        // the wave can't start before the previous one of the same element ends
        for (int i = _waves.Count - 1; i >= 0; i--)
            if (_waves[i].Element == element && _waves[i].GetPlannedEndTime() > startTime)
            {
                Debug.LogError($"Wave creation gone wrong, I should not be seeing this...");
                return _waves[i].GetPlannedEndTime() + 5;
            }

        return startTime;
    }


}
