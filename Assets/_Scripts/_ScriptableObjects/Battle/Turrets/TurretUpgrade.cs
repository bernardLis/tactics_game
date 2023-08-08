using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Turret Upgrade")]
public class TurretUpgrade : BaseScriptableObject
{
    public int Cost;

    public float Range;
    public float Power;
    public float RateOfFire;
    public GameObject GFXPrefab;
}


