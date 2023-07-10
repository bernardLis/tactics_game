using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Minion")]
public class Minion : Entity
{
    public void InitializeMinion(int level)
    {
        Level = level;
        if (level >= 5) Speed = 2;
    }

}

