using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Minion;
using UnityEngine.Serialization;

namespace Lis.Battle.Fight
{
    public class EnemyGroup : BaseScriptableObject
    {
        [FormerlySerializedAs("ElementName")] public NatureName NatureName;
        public List<Minion> Minions = new();
    }
}