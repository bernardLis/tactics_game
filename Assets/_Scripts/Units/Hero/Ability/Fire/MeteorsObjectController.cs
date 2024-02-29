using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class MeteorsObjectController : ObjectControllerDmgOverTime
    {
        [SerializeField] GameObject _circle;    // start lifetime determines how long the circle will be growing (4 seconds now)
        [SerializeField] GameObject _meteor;

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;
            base.Execute(pos, Quaternion.identity);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            transform.localScale = Vector3.one * Ability.GetScale();

            _circle.transform.localScale = Vector3.one;
            _meteor.transform.localScale = Vector3.one;

            ManageCircles();
            yield return new WaitForSeconds(2f);
            ManageMeteors();
            StartCoroutine(DamageCoroutine(Time.time + Ability.GetDuration()));
            yield return new WaitForSeconds(Ability.GetDuration());
            yield return _circle.transform.DOScale(0, 1f).WaitForCompletion();
            _meteor.transform.DOScale(0, 0.5f).OnComplete(
                () =>
                {
                    _meteor.SetActive(false);
                    gameObject.SetActive(false);
                });
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
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, burstCountShort, burstCountShort, 20, 0.1f)
            });

            _meteor.SetActive(true);

            ps.Play();
        }
    }
}
