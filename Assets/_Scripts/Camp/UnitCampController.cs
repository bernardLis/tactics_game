using System.Collections;
using Lis.Camp.Building;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp
{
    public class UnitCampController : MonoBehaviour
    {
        [HideInInspector] public Unit Unit;
        Hero _hero;

        Animator _animator;
        static readonly int AnimAttack = Animator.StringToHash("Attack");

        UnitPathingController _unitPathingController;

        IEnumerator _campCoroutine;

        public void Initialize(Unit unit)
        {
            Unit = unit;
            _hero = GameManager.Instance.Campaign.Hero;

            _animator = GetComponentInChildren<Animator>();

            _unitPathingController = GetComponent<UnitPathingController>();
            _unitPathingController.Initialize(default);
            _unitPathingController.InitializeUnit(unit);

            if (!TryGetComponent(out UnitGrabController grab)) return;
            grab.Initialize();
            grab.OnGrabbed += OnGrabbed;
            grab.OnReleased += OnReleased;
        }

        public void StartCampCoroutine()
        {
            if (_campCoroutine != null) StopCoroutine(_campCoroutine);
            _campCoroutine = CampCoroutine();
            StartCoroutine(_campCoroutine);
        }

        IEnumerator CampCoroutine()
        {
            while (true)
            {
                if (this == null) yield break;

                yield return new WaitForSeconds(Random.Range(5f, 12f));
                Vector3 pos = new(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
                yield return _unitPathingController.PathToPositionAndStop(pos);
            }
        }

        void OnReleased()
        {
            int maxColliders = 10;
            Collider[] results = new Collider[maxColliders];
            Physics.OverlapSphereNonAlloc(transform.position, 1.5f, results);
            foreach (Collider col in results)
            {
                if (!col.TryGetComponent(out UnitDropZoneController zone))
                    continue;
                zone.DropOff(this);
                return;
            }

            StartCampCoroutine();
        }

        void OnGrabbed()
        {
            if (_campCoroutine != null) StopCoroutine(_campCoroutine);
        }

        // TODO: building coroutines - probably a bad idea to handle it like that
        void BaseBuildingAssignment()
        {
            _hero.Army.Remove(Unit);
        }

        public void ReleaseFromBuildingAssignment()
        {
            _hero.Army.Add(Unit);
            StartCampCoroutine();
        }

        public void StartGoldMineCoroutine(BuildingController goldMine)
        {
            BaseBuildingAssignment();
            if (_campCoroutine != null) StopCoroutine(_campCoroutine);
            _campCoroutine = GoldMineCoroutine(goldMine);
            StartCoroutine(_campCoroutine);
        }

        IEnumerator GoldMineCoroutine(BuildingController goldMine)
        {
            while (true)
            {
                if (this == null) yield break;

                yield return new WaitForSeconds(Random.Range(1f, 3f));
                Vector3 pos = goldMine.transform.position +
                              new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                yield return _unitPathingController.PathToPositionAndStop(pos);
                _animator.SetTrigger(AnimAttack);
            }
        }
    }
}