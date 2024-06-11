using UnityEngine;

namespace Lis.Units.Pawn
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Pawn/Mission/Kill")]
    public class PawnMissionKill : PawnMission
    {
        public int KillsToMake;
        [HideInInspector] public int Kills;

        public override void Initialize(Pawn pawn)
        {
            base.Initialize(pawn);
            pawn.OnKilled += OnKilled;
        }

        void OnKilled()
        {
            Kills++;
            if (Kills >= KillsToMake)
                IsCompleted = true;
        }
    }
}