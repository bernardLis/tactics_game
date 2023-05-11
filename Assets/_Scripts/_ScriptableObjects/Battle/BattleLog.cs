using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLog : BaseScriptableObject
{
    public float Time;

    public void SetTime()
    {
        Time = BattleManager.Instance.BattleTime;
    }

}
