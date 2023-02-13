using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/General/DateTime")]
public class DateTime : BaseScriptableObject
{
    public int Day;
    public float Seconds;

    public float GetTimeInSeconds() { return Seconds + Day * GameManager.SecondsInDay; }

    public void LoadFromData(DateTimeData d)
    {
        Day = d.Day;
        Seconds = d.Seconds;
    }

    public DateTimeData SerializeSelf()
    {
        DateTimeData d = new();

        d.Day = Day;
        d.Seconds = Seconds;

        return d;
    }
}

[Serializable]
public struct DateTimeData
{
    public int Day;
    public float Seconds;
}