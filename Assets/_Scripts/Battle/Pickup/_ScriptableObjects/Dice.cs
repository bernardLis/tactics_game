using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Dice")]
    public class Dice : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.GetComponent<HeroManager>().DiceCollected();
        }
    }
}