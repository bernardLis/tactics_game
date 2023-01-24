using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    GameManager _gameManager;

    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _gameManager.OnDayPassed += OnDayPassed;
    }

    void OnDayPassed(int day)
    {
        if (Random.value > 0.5f)
            AddRandomQuest();
    }

    public void AddRandomQuest()
    {
        Quest q = ScriptableObject.CreateInstance<Quest>();
        q.CreateRandom();
        _gameManager.OnDayPassed += q.OnDayPassed;

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Quest, q);
        _gameManager.AddNewReport(r);
    }

}
