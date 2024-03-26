using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Minion
{
    public class MinionController : UnitController
    {
        Minion _minion;

        static readonly int AnimAttack = Animator.StringToHash("Attack");

        IEnumerator _currentCoroutine;

        [Header("Minion")]
        [SerializeField] MinionBody[] _minionBodies;
        GameObject _currentActiveBody;

        public override void InitializeUnit(Unit unit, int team)
        {
            _minion = (Minion)unit;

            // minion pool
            if (Gfx != null)
            {
                Gfx.transform.localScale = Vector3.one;
                Gfx.SetActive(true);

                foreach (MinionBody mb in _minionBodies)
                {
                    if (mb.Name != unit.UnitName) continue;
                    _currentActiveBody = mb.Gfx;
                }

                _currentActiveBody.SetActive(true);
                Animator = _currentActiveBody.GetComponent<Animator>();
                // _currentActiveBody.GetComponentInChildren<MinionMaterialSetter>().SetMaterial(unit);
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
                    _currentActiveBody.SetActive(false);
                    Gfx.SetActive(false);
                });

            yield return new WaitForSeconds(5f);


            StopCoroutine(_currentCoroutine);
            gameObject.SetActive(false);
        }
    }
}

[Serializable]
public struct MinionBody
{
    public string Name;
    public GameObject Gfx;
}