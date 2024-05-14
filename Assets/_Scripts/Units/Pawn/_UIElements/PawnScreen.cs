using Lis.Core;
using UnityEngine.UIElements;


namespace Lis.Units.Pawn
{
    public class PawnScreen : UnitScreen
    {
        Pawn _pawn;

        public PawnScreen(Pawn pawn)
            : base(pawn)
        {
            _pawn = pawn;
        }

        public override void Initialize()
        {
            base.Initialize();

            AddPawnMission();
        }

        void AddPawnMission()
        {
            OtherContainer.Add(new HorizontalSpacerElement());

            OtherContainer.Add(new Label($"Mission to upgrade:"));

            if (_pawn.CurrentMission is PawnMissionKill)
            {
                PawnMissionKill mission = (PawnMissionKill)_pawn.CurrentMission;
                OtherContainer.Add(new Label($"Kills: {mission.Kills} / {mission.KillsToMake}"));
            }

            if (_pawn.CurrentMission is PawnMissionDealDamage)
            {
                PawnMissionDealDamage mission = (PawnMissionDealDamage)_pawn.CurrentMission;
                OtherContainer.Add(new Label($"Damage dealt: {mission.DamageDealt} / {mission.DamageToDeal}"));
            }

            if (_pawn.CurrentMission is PawnMissionTakeDamage)
            {
                PawnMissionTakeDamage mission = (PawnMissionTakeDamage)_pawn.CurrentMission;
                OtherContainer.Add(new Label($"Damage taken: {mission.DamageTaken} / {mission.DamageToTake}"));
            }

            if (_pawn.CurrentMission is PawnMissionSurviveFights)
            {
                PawnMissionSurviveFights mission = (PawnMissionSurviveFights)_pawn.CurrentMission;
                OtherContainer.Add(new Label($"Fights survived: {mission.FightsSurvived} / {mission.FightsToSurvive}"));
            }
        }
    }
}