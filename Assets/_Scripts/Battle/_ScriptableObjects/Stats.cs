using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Stats")]
public class Stats : BaseScriptableObject
{
    public Material Material;
    public float Health;

    public float Power;
    public float AttackRange;
    public float AttackCooldown;


}
