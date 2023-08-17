using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleWave : BaseScriptableObject
{
    public Element Element;
    public int Difficulty;
    public float StartTime;
    public bool IsStarted;

    public List<OpponentGroup> OpponentGroups = new();
    public int CurrentGroupIndex;
    public float DelayBetweenGroups;

    GameManager _gameManager;

    public event Action OnGroupSpawned;
    public void CreateWave(Element element, int difficulty, float startTime)
    {
        _gameManager = GameManager.Instance;

        Element = element;
        Difficulty = difficulty;
        StartTime = startTime;
        /*
        params that I can use:
        - number of minions
        - number of creatures
        - minion level range
        - creature level range
        - delay between each group spawn
        */
        // TODO: math for wave difficulty

        DelayBetweenGroups = Random.Range(10, 20);
        int numberOfGroups = 5 + difficulty;
        for (int i = 0; i < numberOfGroups; i++)
        {
            int numberOfMinions = 1 + (difficulty * i);
            int numberOfCreatures = 0;
            if (i == numberOfGroups - 1) numberOfCreatures = 1;
            Vector2Int minionLevelRange = new Vector2Int(1, 2);
            Vector2Int creatureLevelRange = new Vector2Int(1, 2);

            //  Debug.Log($"group: difficulty {difficulty}, group number {i}, minions {numberOfMinions}, creatures {numberOfCreatures}, delay {DelayBetweenGroups}");

            OpponentGroup group = ScriptableObject.CreateInstance<OpponentGroup>();
            group.CreateGroup(Element, numberOfMinions, minionLevelRange, numberOfCreatures, creatureLevelRange);
            OpponentGroups.Add(group);
        }
    }

    public void SpawningGroupFinished()
    {
        CurrentGroupIndex++;
        OnGroupSpawned?.Invoke();
    }

    public OpponentGroup GetCurrentOpponentGroup()
    {
        return OpponentGroups[CurrentGroupIndex];
    }
}
