using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Building Upgrade")]
public class BuildingUpgrade : BaseScriptableObject
{
    public int Level;
    public int Cost;

    public Creature ProducedCreature;
    public float ProductionDelay;
    public int ProductionLimit;
}
