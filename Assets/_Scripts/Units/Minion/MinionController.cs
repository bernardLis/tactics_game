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

        static readonly int AnimAttack = Animator.StringToHash("Attack");

        IEnumerator _currentCoroutine;

        public override void InitializeUnit(Unit unit, int team)
        {
            // minion pool
            if (Gfx != null)
            {
                Gfx.transform.localScale = Vector3.one;
                Gfx.SetActive(true);
                Gfx.GetComponent<MinionMaterialSetter>().SetMaterial(unit);
            }

            if (unit.Nature.NatureName == NatureName.Earth) InitializeElement(_earthGfx);
            if (unit.Nature.NatureName == NatureName.Fire) InitializeElement(_fireGfx);
            if (unit.Nature.NatureName == NatureName.Water) InitializeElement(_waterGfx);
            if (unit.Nature.NatureName == NatureName.Wind) InitializeElement(_windGfx);

            base.InitializeUnit(unit, team);
            _minion = (Minion)unit;

            UnitPathingController.SetSpeed(_minion.Speed.GetValue() + _minion.Level.Value * Random.Range(0.1f, 0.2f));
            RunUnit();
        }

        void InitializeElement(GameObject elementGfx)
        {
            elementGfx.SetActive(true);
            Animator = elementGfx.GetComponentInChildren<Animator>();
            UnitPathingController.SetAnimator(Animator);
        }

        protected override IEnumerator RunUnitCoroutine()
        {
            if (IsDead) yield break;

            Gfx.transform.localScale = Vector3.one;
            Gfx.transform.localPosition = Vector3.zero; // idk, gfx moves up for some reason

            _currentCoroutine = PathToHero();
            yield return _currentCoroutine;
        }

        IEnumerator PathToHero()
        {
            AddToLog("Pathing to hero");
            UnitPathingController.SetStoppingDistance(0.7f);
            yield return UnitPathingController.PathToTarget(HeroController.transform);

            ReachedHero();
        }

        void ReachedHero()
        {
            if (IsDead) return;
            AddToLog("Reached hero");
            _currentCoroutine = Attack();
            StartCoroutine(_currentCoroutine);
        }

        IEnumerator Attack()
        {
            if (IsDead) yield break;
            if (Vector3.Distance(transform.position, HeroController.transform.position) > 1f)
            {
                AddToLog("Hero is too far to attack");
                yield return PathToHero();
                yield break;
            }

            AddToLog("Attacking hero");
            Animator.SetTrigger(AnimAttack);

//            Gfx.transform.DOPunchScale(Vector3.one * 1.1f, 0.2f, 1, 0.5f);
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

            StopCoroutine(_currentCoroutine);
            gameObject.SetActive(false);
        }
    }
}