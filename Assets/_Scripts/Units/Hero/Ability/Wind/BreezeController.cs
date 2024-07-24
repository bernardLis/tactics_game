using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Arena.Pickup;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class BreezeController : Controller
    {
        [SerializeField] GameObject _gfx;
        [SerializeField] Collider _col;

        List<UnitController> _entitiesInCollider = new();

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BreakableVaseController bbv))
                bbv.TriggerBreak();

            if (other.gameObject.TryGetComponent(out UnitController uc))
            {
                if (uc.Team == 0) return; // TODO: hardcoded team number
                _entitiesInCollider.Add(uc);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out UnitController uc))
            {
                if (uc.Team == 0) return; // TODO: hardcoded team number
                if (_entitiesInCollider.Contains(uc))
                    _entitiesInCollider.Remove(uc);
            }
        }

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
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

            float duration = Ability.GetDuration();
            float cooldown = Ability.GetCooldown();
            if (duration < cooldown) duration = cooldown - 0.3f;

            ParticleSystem ps = _gfx.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule psMain = ps.main;
            psMain.duration = duration;
            psMain.startLifetime = duration;
            yield return new WaitForSeconds(0.1f);
            _gfx.SetActive(true);

            StartCoroutine(DealDamage(duration));

            _col.gameObject.SetActive(true);
            _col.transform.localScale = Vector3.zero;
            _col.transform.DOScale(Vector3.one * 2f, duration - 0.2f)
                .OnComplete(() => _col.gameObject.SetActive(false));

            yield return new WaitForSeconds(duration);

            _gfx.SetActive(false);
        }

        IEnumerator DealDamage(float duration)
        {
            float endTime = Time.time + duration;
            while (Time.time < endTime)
            {
                List<UnitController> currentEntities = new(_entitiesInCollider);
                foreach (UnitController entity in currentEntities)
                    StartCoroutine(entity.GetHit(Ability.GetCurrentLevel()));
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}