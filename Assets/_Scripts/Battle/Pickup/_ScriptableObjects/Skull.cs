using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Skull")]
    public class Skull : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.SkullCollected();
        }
    }
}
