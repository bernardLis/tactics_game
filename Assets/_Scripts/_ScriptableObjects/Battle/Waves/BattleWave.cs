using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleWave : BaseScriptableObject
{
    public int Difficulty;
    public bool IsStarted;
    public float LastGroupSpawnTime;

    public List<OpponentGroup> OpponentGroups = new();
    public int CurrentGroupIndex;
    public float DelayBetweenGroups;

    public event Action OnGroupSpawned;
    public void CreateWave(int difficulty)
    {
        Difficulty = difficulty;
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
        int numberOfGroups = Random.Range(1, 3);
        for (int i = 0; i < numberOfGroups; i++)
        {
            int numberOfMinions = 2 + Mathf.FloorToInt(difficulty * i * 1.1f);
            numberOfMinions = Mathf.Clamp(numberOfMinions, 2, 50);
            int numberOfCreatures = GetNumberOfCreatures(i, numberOfGroups, difficulty);
            Vector2Int minionLevelRange = new Vector2Int(1, difficulty + 1);
            Vector2Int creatureLevelRange = new Vector2Int(1, 2);

            OpponentGroup group = CreateInstance<OpponentGroup>();
            group.CreateGroup(numberOfMinions, minionLevelRange, numberOfCreatures, creatureLevelRange);
            OpponentGroups.Add(group);
        }
    }

    public bool IsFinished()
    {
        return CurrentGroupIndex >= OpponentGroups.Count;
    }

    int GetNumberOfCreatures(int i, int numberOfGroups, int difficulty)
    {
        if (i != numberOfGroups - 1) return 0; // only last group has a creature

        if (difficulty == 1) return 0;

        if (difficulty == 2 || difficulty == 3 || difficulty == 4 || difficulty == 5) return 1;
        return 2;
    }

    public void SpawningGroupFinished()
    {
        CurrentGroupIndex++;
        LastGroupSpawnTime = BattleManager.Instance.GetTime();
        OnGroupSpawned?.Invoke();
    }

    public OpponentGroup GetCurrentOpponentGroup()
    {
        return OpponentGroups[CurrentGroupIndex];
    }
}
