using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Ability/Level")]
    public class Level : BaseScriptableObject
    {
        public float Power;
        public float Cooldown;
        public float Scale;
        public int Amount;
        public float Duration;
        public int Price;

    }
}