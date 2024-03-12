using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class WaterTornadoObjectController : ObjectControllerDmgOverTime
    {
        [SerializeField] ParticleSystem[] _psDurationChange;

        readonly float _speed = 2f;
        Vector3 _dir;

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;
            transform.localScale = Vector3.one * Ability.GetScale();

            foreach (ParticleSystem ps in _psDurationChange)
            {
                var main = ps.main;
                main.startLifetime = Ability.GetDuration();
            }

            base.Execute(pos, rot);
        }

        AudioSource _audioSource;

        protected override IEnumerator ExecuteCoroutine()
        {
            StartCoroutine(DamageCoroutine(Time.time + Ability.GetDuration()));
            if (Ability.ExecuteSound != null)
                _audioSource = AudioManager.PlaySfx(Ability.ExecuteSound, transform, true);

            // I would like tornado to follow a circular path
            // - fuck it, just move it in random direction
            Vector3 rand = Random.insideUnitCircle;
            _dir = new(rand.x, 0, rand.y);
            _dir.Normalize();
            float endTime = Time.time + Ability.GetDuration();
            while (Time.time < endTime)
            {
                transform.position += _dir * (_speed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }

            if (_audioSource != null)
            {
                _audioSource.Stop();
                _audioSource.transform.parent = AudioManager.transform;
                _audioSource = null;
            }

            transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }

        protected override void UnpassableHit()
        {
            _dir *= -1;
        }
    }
}