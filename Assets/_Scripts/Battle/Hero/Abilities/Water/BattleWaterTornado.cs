using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleWaterTornado : BattleAbilityObjectDmgOverTime
    {
        [SerializeField] ParticleSystem[] _psDurationChange;

        float _currentAngle;
        
        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;
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
            // - fuck it, just move it in random direction
            Vector3 rand = Random.insideUnitCircle;
            Vector3 dir = new(rand.x, 0, rand.y);
            dir.Normalize();
            Vector3 endPos = transform.position + dir * _ability.GetDuration();
            yield return transform.DOMove(endPos, _ability.GetDuration()).WaitForCompletion();

            transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}