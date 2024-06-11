using Lis.Battle.Fight;
using UnityEngine;

namespace Lis.Units.Pawn
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Pawn/Mission/Survive Fights")]
    public class PawnMissionSurviveFights : PawnMission
    {
        public int FightsToSurvive;
        [HideInInspector] public int FightsSurvived;

        public override void Initialize(Pawn pawn)
        {
            base.Initialize(pawn);
            FightManager.Instance.OnFightEnded += OnFightEnded;
        }

        private void OnFightEnded()
        {
            FightsSurvived++;
            if (FightsSurvived >= FightsToSurvive)
                IsCompleted = true;
        }
    }
}