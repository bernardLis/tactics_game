using Lis.Core;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Gold Mine")]
    public class GoldMine : Building
    {
        [Header("Gold Mine")]
        public int GoldPerPeasantPerNode;

        [HideInInspector] public int GoldAvailable;

        protected override void NodeCompleted()
        {
            base.NodeCompleted();
            if (!IsUnlocked) return;

            GoldAvailable += GoldPerPeasantPerNode * AssignedWorkers.Count;
        }

        public void CollectGold()
        {
            GameManager.Instance.ChangeGoldValue(GoldAvailable);
            GoldAvailable = 0;
        }
    }
}