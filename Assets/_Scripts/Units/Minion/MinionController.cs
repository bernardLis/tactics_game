using System.Collections;
using DG.Tweening;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Minion
{
    public class MinionController : UnitController
    {
        Minion _minion;

        [Header("Minion")] [SerializeField] GameObject _earthGfx;
        [SerializeField] GameObject _fireGfx;
        [SerializeField] GameObject _waterGfx;
        [SerializeField] GameObject _windGfx;

        public override void InitializeEntity(Unit unit, int team)
        {
            if (Gfx != null) Gfx.SetActive(true);
            if (unit.Element.ElementName == ElementName.Earth) _earthGfx.SetActive(true);
            if (unit.Element.ElementName == ElementName.Fire) _fireGfx.SetActive(true);
            if (unit.Element.ElementName == ElementName.Water) _waterGfx.SetActive(true);
            if (unit.Element.ElementName == ElementName.Wind) _windGfx.SetActive(true);

            base.InitializeEntity(unit, team);
            _minion = (Minion)unit;

            // minion pool
            IsDead = false;
            IsDeathCoroutineStarted = false;
            Collider.enabled = true;

            UnitPathingController.SetSpeed(_minion.Speed.GetValue() + _minion.Level.Value * Random.Range(0.1f, 0.2f));
            StartRunEntityCoroutine();
        }

        protected override IEnumerator RunEntity()
        {
            if (IsDead) yield break;

            Gfx.transform.localScale = Vector3.one;
            Gfx.transform.localPosition = Vector3.zero; // idk, gfx moves up for some reason

            yield return PathToHero();
        }

        IEnumerator PathToHero()
        {
            UnitPathingController.SetStoppingDistance(0.7f);
            yield return UnitPathingController.PathToTarget(HeroController.transform);

            // something is blocking path, so just die...
            if (Vector3.Distance(transform.position, HeroController.transform.position) > 2.5f)
            {
                StartCoroutine(Die(hasLoot: false));
                yield break;
            }

            ReachedHero();
        }

        void ReachedHero()
        {
            StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            Gfx.transform.DOPunchScale(Vector3.one * 1.1f, 0.2f, 1, 0.5f);
            StartCoroutine(HeroController.GetHit(_minion));
            yield return new WaitForSeconds(0.5f);
            StartRunEntityCoroutine();
        }

        public override IEnumerator Die(UnitController attacker = null, bool hasLoot = true)
        {
            yield return base.Die(attacker, hasLoot);
            Gfx.SetActive(false);
            StopAllCoroutines();
            yield return new WaitForSeconds(5f);

            _earthGfx.SetActive(false);
            _fireGfx.SetActive(false);
            _waterGfx.SetActive(false);
            _windGfx.SetActive(false);

            gameObject.SetActive(false);
        }
    }
}