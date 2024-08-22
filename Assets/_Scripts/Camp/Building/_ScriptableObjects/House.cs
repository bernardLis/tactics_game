using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/House")]
    public class House : Building
    {
        [HideInInspector] public int AvailablePeasantCount;

        protected override void NodeCompleted()
        {
            base.NodeCompleted();
            if (!IsUnlocked) return;
            if (GetAssignedWorkerCount() <= 1) return;
            AvailablePeasantCount++;
        }
    }
}