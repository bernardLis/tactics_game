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
        DelayBetweenGroups = Random.Range(10, 20);
        int numberOfGroups = Random.Range(4, 7);
        for (int i = 0; i < numberOfGroups; i++)
        {
            int numberOfMinions = 2 + Mathf.FloorToInt(difficulty * i * 1.25f);
            numberOfMinions = Mathf.Clamp(numberOfMinions, 2, 50);
            int numberOfCreatures = GetNumberOfCreatures(i, numberOfGroups, difficulty);
            Vector2Int minionLevelRange = new Vector2Int(1, difficulty + 1);
            Vector2Int creatureLevelRange = new Vector2Int(1, 2);

            //     Debug.Log($"group: difficulty {difficulty}, group number {i}, minions {numberOfMinions}, creatures {numberOfCreatures}, delay {DelayBetweenGroups}");

            OpponentGroup group = CreateInstance<OpponentGroup>();
            group.CreateGroup(Element, numberOfMinions, minionLevelRange, numberOfCreatures, creatureLevelRange);
            OpponentGroups.Add(group);
        }
    }

    int GetNumberOfCreatures(int i, int numberOfGroups, int difficulty)
    {
        if (i != numberOfGroups - 1) return 0; // only last group has a creature

        if (difficulty == 1) return 0;

        if (difficulty == 2 || difficulty == 3 || difficulty == 4 || difficulty == 5) return 1;
        if (difficulty == 6 || difficulty == 7) return 2;
        return 3;
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
