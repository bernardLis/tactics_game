using System.Collections;
using UnityEngine;

namespace Lis.Units.Enemy
{
    public class EnemyController : UnitController
    {
        [Header("Enemy")]
        [SerializeField] GameObject _miniBossEffect;

        Enemy _enemy;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _enemy = unit as Enemy;


            HandleMiniBoss();
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