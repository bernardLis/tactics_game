using System;
using Lis.Core;
using Lis.Map.MapNodes;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Building")]
    public class Building : BaseScriptableObject
    {
        public Sprite Icon;

        public bool IsUnlocked;
        public int UnlockCost;

        public int Level;
        public int MaxLevel;

        public event Action OnUnlocked;

        public virtual void Initialize(Campaign campaign)
        {
            IsUnlocked = false;

            foreach (MapNode node in campaign.Map.GetAllNodes())
                node.OnNodeCompleted += NodeCompleted;
        }

        protected virtual void NodeCompleted()
        {
            Debug.Log("Node completed in building");
        }

        public void Upgrade()
        {
            if (Level < MaxLevel)
                Level++;
        }

        public void Unlock()
        {
            Debug.Log("Unlocking building");
            IsUnlocked = true;
            OnUnlocked?.Invoke();
        }
    }
}