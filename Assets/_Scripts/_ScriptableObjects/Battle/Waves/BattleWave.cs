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

    public event Action OnGroupSpawned;
    public void CreateWave(Element element, int difficulty, float startTime)
    {
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

        // HERE: waves
        DelayBetweenGroups = Random.Range(7, 15) - difficulty;
        int numberOfGroups = Random.Range(3, 6);
        for (int i = 0; i < numberOfGroups; i++)
        {
            int numberOfMinions = 2 + Mathf.FloorToInt(difficulty * i * 1.5f);
            int numberOfCreatures = GetNumberOfCreatures(i, numberOfGroups, difficulty);
            Vector2Int minionLevelRange = new Vector2Int(difficulty, difficulty + 5);
            Vector2Int creatureLevelRange = new Vector2Int(1, difficulty - 1);

            //  Debug.Log($"group: difficulty {difficulty}, group number {i}, minions {numberOfMinions}, creatures {numberOfCreatures}, delay {DelayBetweenGroups}");

            OpponentGroup group = CreateInstance<OpponentGroup>();
            group.CreateGroup(Element, numberOfMinions, minionLevelRange, numberOfCreatures, creatureLevelRange);
            OpponentGroups.Add(group);
        }
    }

    int GetNumberOfCreatures(int i, int numberOfGroups, int difficulty)
    {
        if (i != numberOfGroups - 1) return 0; // only last group has a creature
        return Mathf.FloorToInt(difficulty * 0.5f);
    }

    public float GetPlannedEndTime()
    {
        return StartTime + DelayBetweenGroups * OpponentGroups.Count;
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
