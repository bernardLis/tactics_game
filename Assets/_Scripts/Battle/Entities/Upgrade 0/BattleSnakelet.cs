using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class Snakelet : BattleCreatureMelee
    {
        [SerializeField] GameObject _abilityHit;

        protected override IEnumerator Attack()
        {
            yield return ManageCreatureAbility();
            yield return base.Attack();
        }

        protected override IEnumerator CreatureAbility()
        {
            if (!IsOpponentInRange()) yield break;

            yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
            yield return base.CreatureAbility();
            CurrentAttackCooldown = Creature.AttackCooldown.GetValue();

            _abilityHit.transform.parent = Opponent.transform;
            _abilityHit.transform.localPosition = Vector3.one;
            _abilityHit.SetActive(true);
            StartCoroutine(Opponent.GetPoisoned(this));

            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            _abilityHit.transform.parent = transform;
            _abilityHit.SetActive(false);
        }
    }
}