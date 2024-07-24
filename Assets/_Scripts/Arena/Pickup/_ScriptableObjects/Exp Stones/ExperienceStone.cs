using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Experience Stone")]
    public class ExperienceStone : Pickup
    {
        public int Amount;
        public int OrbChance;
        public int MinScariness;

        public override void HandleHeroBonuses(Hero hero)
        {
            base.HandleHeroBonuses(hero);
            Amount += Mathf.FloorToInt(Amount *
                                       hero.GetStatByType(StatType.ExpBonus).GetValue()
                                       * 0.01f);
        }

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