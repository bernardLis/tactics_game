using System.Collections;
using System.Collections.Generic;


using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleAbilityBreeze : BattleAbility
    {
        [SerializeField] GameObject _gfx;
        [SerializeField] Collider _col;

        List<BattleEntity> _entitiesInCollider = new();

        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            Ability.OnLevelUp += OnAbilityLevelUp;

            transform.localScale = Vector3.one * Ability.GetScale();
        }

        void OnAbilityLevelUp()
        {
            transform.localScale = Vector3.one * Ability.GetScale();
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            _entitiesInCollider = new();

            ParticleSystem ps = _gfx.GetComponent<ParticleSystem>();
            var psMain = ps.main;
            psMain.duration = Ability.GetDuration();
            psMain.startLifetime = Ability.GetDuration();
            yield return new WaitForSeconds(0.1f);
            _gfx.SetActive(true);

            StartCoroutine(DealDamage());

            _col.gameObject.SetActive(true);
            _col.transform.localScale = Vector3.zero;
            _col.transform.DOScale(Vector3.one * 2f, Ability.GetDuration() - 0.2f)
                .OnComplete(() => _col.gameObject.SetActive(false));

            yield return new WaitForSeconds(Ability.GetDuration());

            _gfx.SetActive(false);
        }

        IEnumerator DealDamage()
        {
            float endTime = Time.time + Ability.GetDuration();
            while (Time.time < endTime)
            {
                List<BattleEntity> currentEntities = new(_entitiesInCollider);
                foreach (BattleEntity entity in currentEntities)
                    StartCoroutine(entity.GetHit(Ability));
                yield return new WaitForSeconds(0.5f);
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                if (_entitiesInCollider.Contains(battleEntity))
                    _entitiesInCollider.Remove(battleEntity);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
                bbv.TriggerBreak();

            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                _entitiesInCollider.Add(battleEntity);
            }
        }
    }
}
