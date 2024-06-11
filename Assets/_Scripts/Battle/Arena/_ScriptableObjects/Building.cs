using System;
using Lis.Core;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Building")]
    public class Building : BaseScriptableObject
    {
        public bool IsUnlocked;
        public int UnlockCost;

        public int Level;
        public int MaxLevel;

        public event Action OnUnlocked;

        public virtual void Initialize()
        {
        }

        public void Upgrade()
        {
            if (Level < MaxLevel)
                Level++;
        }

        public void Unlock()
        {
            IsUnlocked = true;
            OnUnlocked?.Invoke();
        }
    }
}