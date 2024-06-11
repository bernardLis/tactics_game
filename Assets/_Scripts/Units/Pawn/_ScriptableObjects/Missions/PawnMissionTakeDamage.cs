using UnityEngine;

namespace Lis.Units.Pawn
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Pawn/Mission/Take Damage")]
    public class PawnMissionTakeDamage : PawnMission
    {
        public int DamageToTake;
        [HideInInspector] public int DamageTaken;

        public override void Initialize(Pawn pawn)
        {
            base.Initialize(pawn);
            pawn.OnDamageTaken += OnDamageTaken;
        }

        void OnDamageTaken(int dmg)
        {
            DamageTaken += dmg;
            if (DamageTaken >= DamageToTake)
                IsCompleted = true;
        }
    }
}