using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : BaseScriptableObject
{
    public IntVariable Lives;
    public IntVariable TroopsLimit;

    public void Initialize()
    {
        Lives = ScriptableObject.CreateInstance<IntVariable>();
        Lives.SetValue(10);

        TroopsLimit = ScriptableObject.CreateInstance<IntVariable>();
        TroopsLimit.SetValue(5);
    }
}
