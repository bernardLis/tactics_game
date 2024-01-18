using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Experience Orb")]
    public class ExperienceOrb : Pickup
    {
        public int Amount;
        public int OrbChance;

        public override void Initialize()
        {
            base.Initialize();
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
