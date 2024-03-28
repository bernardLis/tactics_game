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

        [SerializeField] GameObject _miniBossEffect;

        GameObject _currentActiveBody;

        public override void InitializeUnit(Unit unit, int team)
        {
            _minion = (Minion)unit;

            // minion pool
            if (Gfx != null) InitializeMinion();
            base.InitializeUnit(unit, team);

            if (_minion.IsMiniBoss) InitializeMiniBoss();

            UnitPathingController.SetSpeed(_minion.Speed.GetValue() + Random.Range(0.1f, 0.2f));
            RunUnit();
        }

        void InitializeMinion()
        {
            Gfx.transform.localScale = Vector3.one;
            Gfx.SetActive(true);

            foreach (MinionBody mb in _minionBodies)
            {
                if (mb.Name != _minion.UnitName) continue;
                _currentActiveBody = mb.Gfx;
            }

            _currentActiveBody.SetActive(true);
            Animator = _currentActiveBody.GetComponent<Animator>();
            _currentActiveBody.GetComponentInChildren<MinionMaterialSetter>().SetMaterial(_minion);
            UnitPathingController.SetAnimator(Animator);
        }

        void InitializeMiniBoss()
        {
            transform.localScale = Vector3.one * 2;
            _miniBossEffect.SetActive(true);
            UnitPathingController.SetStoppingDistance(1.3f);
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
            if (Vector3.Distance(transform.position, HeroController.transform.position) > 1.5f)
            {
                AddToLog("Hero is too far to attack");
                yield return PathToHero();
                yield break;
            }

            AddToLog("Attacking hero");
            Animator.SetTrigger(AnimAttack);

            StartCoroutine(HeroController.GetHit(_minion));
            yield return new WaitForSeconds(1.6f);
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
                    if (_minion.IsMiniBoss) ResolveMiniBossDeath();
                });

            yield return new WaitForSeconds(5f);

            StopCoroutine(_currentCoroutine);
            gameObject.SetActive(false);
        }

        void ResolveMiniBossDeath()
        {
            int v = Random.Range(3, 8);
            Debug.Log($"Spawning {v} exp orbs");
            for (int i = 0; i < v; i++)
                PickupManager.SpawnExpStone(transform.position +
                                          new Vector3(Random.Range(-1f, 1f), 2, Random.Range(-1f, 1f)));
            transform.localScale = Vector3.one * 0.5f;
            _miniBossEffect.SetActive(false);
            UnitPathingController.SetStoppingDistance(0.7f);
        }
    }
}

[Serializable]
public struct MinionBody
{
    public string Name;
    public GameObject Gfx;
}