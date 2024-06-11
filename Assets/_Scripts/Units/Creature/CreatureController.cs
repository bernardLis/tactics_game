using System.Collections;
using UnityEngine;

namespace Lis.Units.Creature
{
    public class CreatureController : UnitController
    {
        private Creature _creature;


        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            if (team == 0) ObjectShaders.LitShader();

            _creature = (Creature)unit;
        }

        protected override void OnLevelUp()
        {
            base.OnLevelUp();
            DisplayFloatingText("Level Up!", Color.white);
            _creature.CurrentHealth.SetValue(Mathf.FloorToInt(_creature.MaxHealth.GetValue()));

            if (_creature.SpecialAttack is null || _creature.Level.Value != 4) return;
            AddToLog("Unlocking special attack");
            DisplayFloatingText("Special Attack Unlocked!", Color.white);
            _creature.AddAttack(_creature.SpecialAttack);
        }

        protected override IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attack, hasLoot);
            StopUnit();
            UnitPathingController.DisableAgent();

            _creature.Die();
        }
    }
}