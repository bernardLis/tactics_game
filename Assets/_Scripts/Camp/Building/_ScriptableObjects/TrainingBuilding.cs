using Lis.Units;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Training Building")]
    public class TrainingBuilding : Building
    {
        public Unit UnitToTrain;
        [HideInInspector] public int AvailablePawnCount;


        protected override void NodeCompleted()
        {
            base.NodeCompleted();
            if (!IsUnlocked) return;
            if (GetAssignedWorkerCount() <= 0) return;
            AvailablePawnCount++;
        }
    }
}