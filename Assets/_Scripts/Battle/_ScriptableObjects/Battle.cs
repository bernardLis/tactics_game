using Lis.Core;
using Lis.Units.Boss;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Battle
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle")]
    public class Battle : BaseScriptableObject
    {
        public int Duration = 1200;
        public Stats Stats;

        public Boss Boss;


        public void Initialize(int level)
        {
            Stats = CreateInstance<Stats>();
            Stats.Initialize();

            Boss = Instantiate(GameManager.Instance.EntityDatabase.GetRandomBoss());
            Boss.InitializeBattle(1);
        }
    }
}