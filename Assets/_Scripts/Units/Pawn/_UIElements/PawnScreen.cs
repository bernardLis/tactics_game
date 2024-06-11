using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units.Pawn
{
    public class PawnScreen : UnitScreen
    {
        private readonly Pawn _pawn;

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

        private void AddPawnMission()
        {
            OtherContainer.Add(new HorizontalSpacerElement());

            OtherContainer.Add(new Label("Mission to upgrade:"));

            if (_pawn.CurrentMission is PawnMissionKill kill)
                OtherContainer.Add(new Label($"Kills: {kill.Kills} / {kill.KillsToMake}"));

            if (_pawn.CurrentMission is PawnMissionDealDamage damage)
                OtherContainer.Add(new Label($"Damage dealt: {damage.DamageDealt} / {damage.DamageToDeal}"));

            if (_pawn.CurrentMission is PawnMissionTakeDamage takeDamage)
                OtherContainer.Add(new Label($"Damage taken: {takeDamage.DamageTaken} / {takeDamage.DamageToTake}"));

            if (_pawn.CurrentMission is PawnMissionSurviveFights survive)
                OtherContainer.Add(new Label($"Fights survived: {survive.FightsSurvived} / {survive.FightsToSurvive}"));
        }
    }
}