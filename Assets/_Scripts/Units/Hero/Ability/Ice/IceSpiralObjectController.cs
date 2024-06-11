using System.Collections;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class IceSpiralObjectController : ObjectControllerDmgOverTime
    {
        [SerializeField] private GameObject _gfx;
        [SerializeField] private Collider _col;

        [SerializeField] private ParticleSystem _iceSpikes;
        [SerializeField] private ParticleSystem _snow;
        [SerializeField] private ParticleSystem[] _delayedEffects; // -1f of duration
        [SerializeField] private ParticleSystem _iceExplosion; // -0.2f of duration

        [SerializeField] private Sound _startSound;
        [SerializeField] private Sound _explodeSound;

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;
            base.Execute(pos, rot);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            _gfx.SetActive(true);
            _col.gameObject.SetActive(true);
            Vector3 pos = transform.position;
            AudioManager.PlaySfx(_startSound, pos);

            SetDurations();
            StartCoroutine(DamageCoroutine(Time.time + Ability.GetDuration()));

            yield return new WaitForSeconds(Ability.GetDuration());
            AudioManager.PlaySfx(_explodeSound, pos);
            yield return new WaitForSeconds(0.5f);
            _col.gameObject.SetActive(false);

            yield return new WaitForSeconds(3f); // for ice to disappear
            _gfx.SetActive(false);
            gameObject.SetActive(false);
        }

        private void SetDurations()
        {
            ParticleSystem.MainModule iceSpikesMain = _iceSpikes.main;
            iceSpikesMain.startLifetime = Ability.GetDuration();

            ParticleSystem.MainModule snowMain = _snow.main;
            snowMain.startLifetime = Ability.GetDuration();

            foreach (ParticleSystem ps in _delayedEffects)
            {
                ParticleSystem.MainModule psMain = ps.main;
                psMain.startDelay = Ability.GetDuration() - 1f;
            }

            ParticleSystem.MainModule iceExplosionMain = _iceExplosion.main;
            iceExplosionMain.startDelay = Ability.GetDuration() - 0.1f;
        }
    }
}