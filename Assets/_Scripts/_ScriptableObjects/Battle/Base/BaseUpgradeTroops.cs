using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Base/Base Upgrade Troops")]
public class BaseUpgradeTroops : BaseUpgrade
{
    public IntVariable CurrentLimit;

    public List<BaseUpgradeLevel> MaxTroopsTree = new();
    public int CurrentMaxTroopsLevel;

    public List<Creature> DeadTroops = new();

    public override void Initialize()
    {
        CurrentLimit = ScriptableObject.CreateInstance<IntVariable>();
        CurrentLimit.SetValue(MaxTroopsTree[CurrentMaxTroopsLevel].Value);

        base.Initialize();
    }

}
