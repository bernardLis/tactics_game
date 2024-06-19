using System;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle.Arena.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Building")]
    public class Building : BaseScriptableObject
    {
        public Sprite Icon;
        
        public bool IsUnlocked;
        public int UnlockCost;

        public int Level;
        public int MaxLevel;

        public event Action OnUnlocked;

        public virtual void Initialize(Battle battle)
        {
            IsUnlocked = false;
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