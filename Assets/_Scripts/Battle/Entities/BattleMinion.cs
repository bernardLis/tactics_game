using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleMinion : BattleEntity
    {
        public Minion Minion { get; private set; }

        [SerializeField] GameObject _deathEffect;

        BattleHero _targetHero;

        static readonly int Attack = Animator.StringToHash("Attack");

        public override void InitializeEntity(Entity entity, int team)
        {
            if (Gfx != null) Gfx.SetActive(true);

            base.InitializeEntity(entity, team);
            Minion = (Minion)entity;

            // minion pool
            IsDead = false;
            IsDeathCoroutineStarted = false;
            Collider.enabled = true;
        }

        public override void InitializeBattle(ref List<BattleEntity> opponents)
        {
            base.InitializeBattle(ref opponents);

            _targetHero = BattleManager.GetComponent<BattleHeroManager>().BattleHero;
            StartRunEntityCoroutine();
        }

        protected override IEnumerator RunEntity()
        {
            if (IsDead) yield break;

            yield return PathToHero();
        }

        IEnumerator PathToHero()
        {
            yield return new WaitForSeconds(0.5f);
            Gfx.transform.localPosition = Vector3.zero; // idk, gfx moves up for some reason

            Agent.stoppingDistance = 0.7f;
            yield return PathToTarget(_targetHero.transform);

            // something is blocking path, so just die...
            if (Vector3.Distance(transform.position, _targetHero.transform.position) > 2.5f)
            {
                StartCoroutine(Die(hasLoot: false));
                yield break;
            }

            ReachedHero();
        }

        void ReachedHero()
        {
            _targetHero.BaseGetHit(5, GameManager.GameDatabase.GetColorByName("Health").Primary);
            StartCoroutine(PathToHero());
        }

        public override IEnumerator Die(EntityFight attacker = null, bool hasLoot = true)
        {
            yield return base.Die(attacker, hasLoot);
            _deathEffect.SetActive(true);
            Gfx.SetActive(false);
            StopAllCoroutines();
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
    }
}