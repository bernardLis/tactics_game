using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Minion;

namespace Lis.Battle.Fight
{
    public class EnemyGroup : BaseScriptableObject
    {
        public ElementName ElementName;
        public List<Minion> Minions = new();
    }
}