using System.Collections;
using DG.Tweening;
using Lis.Battle;
using UnityEngine;

namespace Lis.Units.Enemy
{
    public class EnemyController : UnitController
    {
        [Header("Enemy")]
        [SerializeField]
        GameObject _miniBossEffect;

        Enemy _enemy;

        EnemyPool _pool;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _enemy = unit as Enemy;

            HandleMiniBoss();
        }

        protected override void OnFightEnded()
        {
            if (this == null) return;
            if (Team != 1 || !IsDead) return;

            transform.DOMoveY(0f, 5f)
                .SetDelay(1f)
                .OnComplete(ReturnToPool);
        }

        public void SetPool(EnemyPool pool)
        {
            _pool = pool;
        }

        void ReturnToPool()
        {
            if (_pool != null) _pool.Return(this);
        }

        void HandleMiniBoss()
        {
            if (_enemy == null) return;
            if (!_enemy.IsMiniBoss) return;

            transform.localScale = new(transform.localScale.x + 0.5f,
                transform.localScale.y + 0.5f,
                transform.localScale.z + 0.5f);

            _miniBossEffect.SetActive(true);

            foreach (Attack.Attack a in _enemy.Attacks) // TODO: this is a hacky way to make sure mini boss can attack
                a.Range += 1;
        }

        protected override IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            if (_enemy != null && _enemy.IsMiniBoss) _miniBossEffect.SetActive(false);
            return base.DieCoroutine(attack, hasLoot);
        }
    }
}