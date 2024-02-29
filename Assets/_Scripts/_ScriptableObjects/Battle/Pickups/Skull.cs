
using Lis.Units.Hero;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Skull")]
    public class Skull : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.SkullCollected();
        }
    }
}
