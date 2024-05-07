using UnityEngine;

namespace Lis.Units.Attack
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Attacks/Attack Hero Ability")]
    public class AttackHeroAbility : Attack
    {
        [Header("Ability")]
        public float Scale;

        public int Amount;
        public float Duration;
        public int Price;
    }
}