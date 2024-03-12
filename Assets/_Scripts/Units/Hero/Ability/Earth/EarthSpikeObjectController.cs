using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Hero.Ability
{
    public class EarthSpikeObjectController : ObjectControllerDmgOverTime
    {
        [FormerlySerializedAs("_GFX")] [SerializeField] GameObject _gfx;
        [SerializeField] GameObject _col;
        [SerializeField] ParticleSystem _spikes;

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localScale = Vector3.one * Ability.GetScale();
        }

        protected override void OnAbilityLevelUp()
        {
            transform.localScale = Vector3.one * Ability.GetScale();
        }

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;
            base.Execute(pos, rot);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            _gfx.SetActive(true);
            _col.SetActive(true);
            if (Ability.ExecuteSound != null) AudioManager.PlaySfx(Ability.ExecuteSound, transform.position);

            ParticleSystem.MainModule main = _spikes.main;
            main.startLifetime = Ability.GetDuration();

            yield return DamageCoroutine(Time.time + Ability.GetDuration());
            yield return new WaitForSeconds(0.5f);

            _col.SetActive(false);
            _gfx.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}