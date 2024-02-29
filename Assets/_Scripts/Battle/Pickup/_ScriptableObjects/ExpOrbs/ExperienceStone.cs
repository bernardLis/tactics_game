using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Experience Stone")]
    public class ExperienceStone : Pickup
    {
        public int Amount;
        public int OrbChance;

        public override void Collected(Hero hero)
        {
            hero.AddExp(Amount);
        }

        public override string GetCollectedText()
        {
            return $"+{Amount} EXP";
        }

    }
}
