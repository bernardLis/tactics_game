using System.Collections;
using System.Collections.Generic;
using Lis;
using Lis.Core;
using Lis.Units.Minion;
using UnityEngine;

public class EnemyGroup : BaseScriptableObject
{
    public ElementName ElementName;
    public List<Minion> Minions = new();
}