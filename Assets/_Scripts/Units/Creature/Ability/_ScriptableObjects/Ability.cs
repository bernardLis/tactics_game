using System;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Creature/Ability")]
    public class Ability : BaseScriptableObject
    {
        public string Description;

        public Sprite Icon;
        public int Cooldown;
        public Sound Sound;

        public int UnlockLevel;

        [Header("Execution Conditions")]
        public bool ExecuteOnCooldown;

        public bool ExecuteOnMove;
        public bool ExecuteOnAttack;
        public bool ExecuteOnDeath;

        public GameObject Prefab;

        public event Action OnAbilityUsed;

        public void Used()
        {
            OnAbilityUsed?.Invoke();
        }

        public event Action OnAbilityUnlocked;

        public void Unlock()
        {
            OnAbilityUnlocked?.Invoke();
        }
    }
}