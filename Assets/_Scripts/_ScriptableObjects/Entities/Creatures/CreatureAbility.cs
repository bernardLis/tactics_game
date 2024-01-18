using System;

using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Creature Ability")]
    public class CreatureAbility : BaseScriptableObject
    {
        public string Description;

        public Sprite Icon;
        public int Cooldown;
        public Sound Sound;

        public int UnlockLevel;

        public event Action OnAbilityUsed;
        public void Used() { OnAbilityUsed?.Invoke(); }

        public event Action OnAbilityUnlocked;
        public void Unlock() { OnAbilityUnlocked?.Invoke(); }
    }
}
