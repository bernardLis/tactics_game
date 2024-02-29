using Lis.Core;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Ability Level")]
    public class AbilityLevel : BaseScriptableObject
    {
        public int Level;

        public float Power;
        public float Cooldown;
        public float Scale;
        public int Amount;
        public float Duration;


    }
}
