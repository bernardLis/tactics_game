using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogManager : MonoBehaviour
{
    public List<BattleLog> Logs = new();

    public void AddLog(BattleLog log) { Logs.Add(log); }

}
