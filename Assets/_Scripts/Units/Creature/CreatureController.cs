using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Creature
{
    public class CreatureController : UnitController
    {
        Creature _creature;

        [SerializeField] GameObject _respawnEffect;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            if (team == 0) ObjectShaders.LitShader();

            _creature = (Creature)unit;
        }

        protected override IEnumerator OnFightEndedCoroutine()
        {
            if (IsDead) yield return Respawn();
            yield return base.OnFightEndedCoroutine();
        }

        protected override void OnLevelUp()
        {
            base.OnLevelUp();
            DisplayFloatingText("Level Up!", Color.white);
            _creature.CurrentHealth.SetValue(Mathf.FloorToInt(_creature.MaxHealth.GetValue()));

            if (_creature.SpecialAttack is null || _creature.Level.Value != 4) return;
            AddToLog("Unlocking special attack");
            DisplayFloatingText("Special Attack Unlocked!", Color.white);
            _creature.SpecialAttack.InitializeAttack(this);
            _creature.AddAttack(_creature.SpecialAttack);
        }

        protected override IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attack, hasLoot);
            StopUnit();
            UnitPathingController.DisableAgent();
            _respawnEffect.SetActive(false);

            _creature.Die();
        }

        IEnumerator Respawn()
        {
            AddToLog("Respawning...");
            Animator.Rebind();
            Animator.Update(0f);
            _respawnEffect.SetActive(true);
            EnableSelf();
            yield return new WaitForSeconds(1);
        }

#if UNITY_EDITOR
        [ContextMenu("Respawn")]
        public void DebugRespawn() => StartCoroutine(Respawn());
#endif
    }
}