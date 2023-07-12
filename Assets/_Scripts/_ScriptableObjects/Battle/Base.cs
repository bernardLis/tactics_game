using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : BaseScriptableObject
{
    public IntVariable TroopsLimit;


    public void Initialize()
    {
        TroopsLimit = ScriptableObject.CreateInstance<IntVariable>();
        TroopsLimit.SetValue(3);
    }

    public void AddTroopsLimit(int amount)
    {
        TroopsLimit.ApplyChange(amount);
    }

}
