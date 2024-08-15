using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Map.MapNodes;
using Lis.Units;
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

        protected readonly List<Unit> AssignedWorkers = new();
        public int MaxWorkers = 2;

        protected Campaign Campaign;

        public event Action OnUnlocked;

        public virtual void Initialize(Campaign campaign)
        {
            IsUnlocked = false;
            Campaign = campaign;

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

        public void AssignWorker(Unit unit)
        {
            AssignedWorkers.Add(unit);
        }

        public List<Unit> GetAssignedWorkers()
        {
            return AssignedWorkers;
        }

        public int GetAssignedWorkerCount()
        {
            return AssignedWorkers.Count;
        }

        public void ReleaseWorker(Unit unit)
        {
            AssignedWorkers.Remove(unit);
        }
    }
}