using UnityEngine;

namespace Lis.Units.Pawn
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Pawn/Mission/Deal Damage")]
    public class PawnMissionDealDamage : PawnMission
    {
        public int DamageToDeal;
        [HideInInspector] public int DamageDealt;

        public override void Initialize(Pawn pawn)
        {
            base.Initialize(pawn);
            foreach (Attack.Attack a in pawn.Attacks)
                a.OnDamageDealt += OnDamageDealt;
        }

        void OnDamageDealt(int dmg)
        {
            DamageDealt += dmg;
            if (DamageDealt >= DamageToDeal)
                IsCompleted = true;
        }
    }
}