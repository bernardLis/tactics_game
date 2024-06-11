using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class WaterTornadoObjectController : ObjectControllerDmgOverTime
    {
        [SerializeField] private ParticleSystem[] _psDurationChange;

        private readonly float _speed = 2f;
        private Vector3 _dir;

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;
            transform.localScale = Vector3.one * Ability.GetScale();

            foreach (ParticleSystem ps in _psDurationChange)
            {
                ParticleSystem.MainModule main = ps.main;
                main.startLifetime = Ability.GetDuration();
            }

            base.Execute(pos, rot);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            StartCoroutine(DamageCoroutine(Time.time + Ability.GetDuration()));
            if (Ability.ExecuteSound != null)
                AudioSource = AudioManager.PlaySfx(Ability.ExecuteSound, transform, true);

            BattleManager.OnGamePaused += () =>
            {
                if (AudioSource != null)
                    AudioSource.Pause();
            };

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

            if (AudioSource != null)
            {
                AudioSource.Stop();
                AudioSource.transform.parent = AudioManager.transform;
                AudioSource = null;
            }

            transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }

        public override void DisableSelf()
        {
            if (AudioSource != null)
            {
                AudioSource.Stop();
                AudioSource.transform.parent = AudioManager.transform;
                AudioSource = null;
            }

            base.DisableSelf();
        }

        protected override void UnpassableHit()
        {
            _dir *= -1;
        }
    }
}