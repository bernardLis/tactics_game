using System.Collections;


using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleWaterTornado : BattleAbilityObjectDmgOverTime
    {
        [SerializeField] ParticleSystem[] _psDurationChange;

        float _angularSpeed = 1f;
        float _circleRad = 3f;
        float _currentAngle;

        BattleHero _hero;

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            _hero = BattleManager.Instance.BattleHero;
        }

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;

            _angularSpeed = Random.Range(0.1f, 0.4f);
            _circleRad = Vector3.Distance(pos, _hero.transform.position);
            transform.localScale = Vector3.one * _ability.GetScale();

            foreach (ParticleSystem ps in _psDurationChange)
            {
                var main = ps.main;
                main.startLifetime = _ability.GetDuration();
            }

            base.Execute(pos, rot);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            StartCoroutine(DamageCoroutine(Time.time + _ability.GetDuration()));

            // I would like tornado to follow a circular path
            Vector3 fixedPos = _hero.transform.position +
                               new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            float endTime = Time.time + _ability.GetDuration();
            while (Time.time < endTime)
            {
                _currentAngle += _angularSpeed * Time.deltaTime;
                Vector3 offset = new Vector3(Mathf.Sin(_currentAngle), 0, Mathf.Cos(_currentAngle)) * _circleRad;
                transform.position = fixedPos + offset;

                yield return new WaitForFixedUpdate();
            }

            transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
