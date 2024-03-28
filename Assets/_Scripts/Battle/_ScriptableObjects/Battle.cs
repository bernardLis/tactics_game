using Lis.Core;
using Lis.Units.Boss;
using UnityEngine;

namespace Lis.Battle
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle")]
    public class Battle : BaseScriptableObject
    {
        public int Duration = 900;
        public Stats Stats;

        public Boss Boss;

        public void Initialize(int level)
        {
            Stats = CreateInstance<Stats>();

            Boss = Instantiate(GameManager.Instance.UnitDatabase.GetRandomBoss());
            Boss.InitializeBattle(1);
        }
    }
}