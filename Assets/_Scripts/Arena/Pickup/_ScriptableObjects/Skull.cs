using Lis.Arena.Fight;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Skull")]
    public class Skull : Pickup
    {
        public override void Collected(Hero hero)
        {
            FightManager.Instance.SkullCollected();
        }
    }
}