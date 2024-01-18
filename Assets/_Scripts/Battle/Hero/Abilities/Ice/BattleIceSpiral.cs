using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleIceSpiral : BattleAbilityObjectDmgOverTime
    {
        [SerializeField] GameObject _gfx;
        [SerializeField] Collider _col;

        [SerializeField] ParticleSystem _iceSpikes;
        [SerializeField] ParticleSystem _snow;
        [SerializeField] ParticleSystem[] _delayedEffects; // -1f of duration
        [SerializeField] ParticleSystem _iceExplosion; // -0.2f of duration

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;
            base.Execute(pos, rot);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            _gfx.SetActive(true);
            _col.gameObject.SetActive(true);

            SetDurations();
            StartCoroutine(DamageCoroutine(_ability.GetDuration()));

            yield return new WaitForSeconds(_ability.GetDuration() + 0.5f);
            _col.gameObject.SetActive(false);

            yield return new WaitForSeconds(3f); // for ice to disappear
            _gfx.SetActive(false);
            gameObject.SetActive(false);
        }

        void SetDurations()
        {
            var iceSpikesMain = _iceSpikes.main;
            iceSpikesMain.startLifetime = _ability.GetDuration();

            var snowMain = _snow.main;
            snowMain.startLifetime = _ability.GetDuration();

            foreach (ParticleSystem ps in _delayedEffects)
            {
                var psMain = ps.main;
                psMain.startDelay = _ability.GetDuration() - 1f;
            }

            var iceExplosionMain = _iceExplosion.main;
            iceExplosionMain.startDelay = _ability.GetDuration() - 0.1f;
        }

    }
}
