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

        public override void InitializeUnit(Unit unit, int team)
        {
            // minion pool

            if (Gfx != null)
            {
                Gfx.transform.localScale = Vector3.one;
                Gfx.SetActive(true);
            }

            if (unit.Nature.NatureName == NatureName.Earth) _earthGfx.SetActive(true);
            if (unit.Nature.NatureName == NatureName.Fire) _fireGfx.SetActive(true);
            if (unit.Nature.NatureName == NatureName.Water) _waterGfx.SetActive(true);
            if (unit.Nature.NatureName == NatureName.Wind) _windGfx.SetActive(true);

            base.InitializeUnit(unit, team);
            _minion = (Minion)unit;

            UnitPathingController.SetSpeed(_minion.Speed.GetValue() + _minion.Level.Value * Random.Range(0.1f, 0.2f));
            RunUnit();
        }

        protected override IEnumerator RunUnitCoroutine()
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
                Die();
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
            RunUnit();
        }

        public override IEnumerator DieCoroutine(UnitController attacker = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attacker, hasLoot);
            Gfx.transform.DOScale(0, 0.5f)
                .OnComplete(() => Gfx.SetActive(false));

            yield return new WaitForSeconds(5f);

            _earthGfx.SetActive(false);
            _fireGfx.SetActive(false);
            _waterGfx.SetActive(false);
            _windGfx.SetActive(false);

            gameObject.SetActive(false);
        }
    }
}