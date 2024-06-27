using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class MeteorsObjectController : ObjectControllerDmgOverTime
    {
        [SerializeField]
        GameObject _circle; // start lifetime determines how long the circle will be growing (4 seconds now)

        [SerializeField] GameObject _meteor;

        readonly List<SoundEmitter> _audioSources = new();

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0.1f;
            base.Execute(pos, Quaternion.identity);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            transform.localScale = Vector3.one * Ability.GetScale();

            _circle.transform.localScale = Vector3.one;
            _meteor.transform.localScale = Vector3.one;

            ManageCircles();
            yield return new WaitForSeconds(8f);
            ManageMeteors();
            StartCoroutine(PlaySound());

            StartCoroutine(DamageCoroutine(Time.time + Ability.GetDuration()));
            yield return new WaitForSeconds(Ability.GetDuration() - 0.5f);
            StopSound();
            yield return new WaitForSeconds(0.5f);

            yield return _circle.transform.DOScale(0, 1f).WaitForCompletion();
            _meteor.transform.DOScale(0, 0.5f).OnComplete(
                () =>
                {
                    _meteor.SetActive(false);
                    gameObject.SetActive(false);
                });
        }

        IEnumerator PlaySound()
        {
            if (Ability.ExecuteSound == null) yield break;
            for (int i = 0; i < 5; i++)
            {
                SoundEmitter ass = AudioManager.CreateSound()
                    .WithSound(Ability.ExecuteSound)
                    .WithPosition(transform.position)
                    .Play();

                _audioSources.Add(ass);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            }
        }

        protected override void OnGamePaused()
        {
            base.OnGamePaused();
            foreach (SoundEmitter ass in _audioSources)
                ass.Pause();
        }

        protected override void OnGameResumed()
        {
            base.OnGameResumed();
            foreach (SoundEmitter ass in _audioSources)
                ass.Resume();
        }

        void StopSound()
        {
            foreach (SoundEmitter ass in _audioSources)
            {
                if (ass == null) continue;
                ass.Stop();
            }

            _audioSources.Clear();
        }

        void ManageCircles()
        {
            _circle.SetActive(true);

            foreach (Transform child in _circle.transform)
            {
                ParticleSystem ps = child.GetComponent<ParticleSystem>();
                ps.Simulate(0.0f, true, true);
                ps.Play();
            }
        }

        void ManageMeteors()
        {
            ParticleSystem ps = _meteor.GetComponent<ParticleSystem>();
            ps.Simulate(0.0f, true, true);

            ParticleSystem.ShapeModule shape = ps.shape;
            shape.radius = Ability.GetScale();
            int burstCount = Mathf.FloorToInt(Ability.GetDuration());
            short burstCountShort = (short)burstCount;
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new(0f, burstCountShort, burstCountShort, 20, 0.1f)
            });

            _meteor.SetActive(true);

            ps.Play();
        }
    }
}