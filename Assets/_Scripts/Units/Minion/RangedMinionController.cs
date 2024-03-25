using System.Collections;
using Lis.Units.Creature;
using UnityEngine;

namespace Lis.Units.Minion
{
    public class RangedMinionController : CreatureControllerRanged
    {
        MinionMaterialSetter _minionMaterialSetter;

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _minionMaterialSetter = GetComponentInChildren<MinionMaterialSetter>();
        }

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            if (Gfx != null)
                Gfx.SetActive(true);
            _minionMaterialSetter.SetMaterial(unit);

            // pool
            IsDead = false;
            IsDeathCoroutineStarted = false;
            Collider.enabled = true;

            Opponent = BattleManager.HeroController;
        }

        protected override IEnumerator RunUnitCoroutine()
        {
            while (true)
            {
                if (IsDead) yield break;
                yield return ManagePathing();
                yield return ManageAttackCoroutine();
            }
        }

        public override IEnumerator DieCoroutine(UnitController attacker = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attacker, hasLoot);
            Gfx.SetActive(false);
            StopAllCoroutines();
            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }
    }
}