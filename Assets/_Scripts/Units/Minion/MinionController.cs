using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Minion
{
    public class MinionController : UnitController
    {
        Minion _minion;

        [Header("Minion")]
        static readonly int AnimAttack = Animator.StringToHash("Attack");

        IEnumerator _currentCoroutine;

        public override void InitializeUnit(Unit unit, int team)
        {
            _minion = (Minion)unit;

            // minion pool
            if (Gfx != null)
            {
                Gfx.transform.localScale = Vector3.one;
                Gfx.SetActive(true);

                GameObject gfxInstance = Instantiate(_minion.GfxPrefab, Gfx.transform);
                MinionMaterialSetter ms = gfxInstance.GetComponent<MinionMaterialSetter>();
                if (ms != null) ms.SetMaterial(unit);
                Animator = gfxInstance.GetComponent<Animator>();
                UnitPathingController.SetAnimator(Animator);
            }

            base.InitializeUnit(unit, team);

            UnitPathingController.SetSpeed(_minion.Speed.GetValue() + _minion.Level.Value * Random.Range(0.1f, 0.2f));
            RunUnit();
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
                .OnComplete(() =>
                {
                    Debug.Log($"{Gfx.transform.GetChild(0)}");
                    Destroy(Gfx.transform.GetChild(0).gameObject);
                    Gfx.SetActive(false);
                });

            yield return new WaitForSeconds(5f);


            StopCoroutine(_currentCoroutine);
            gameObject.SetActive(false);
        }
    }
}