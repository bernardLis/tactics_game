using System;
using System.Collections;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Camp
{
    public class UnitCampController : MonoBehaviour
    {
        [HideInInspector] public Unit Unit;
        Hero _hero;

        Animator _animator;
        static readonly int AnimAttack = Animator.StringToHash("Attack");

        UnitPathingController _unitPathingController;
        UnitGrabController _unitGrabController;

        IEnumerator _campCoroutine;

        public event Action<UnitCampController> OnGrabbed;

        public void Initialize(Unit unit)
        {
            Unit = unit;
            _hero = GameManager.Instance.Campaign.Hero;

            _animator = GetComponentInChildren<Animator>();

            _unitPathingController = GetComponent<UnitPathingController>();
            _unitPathingController.Initialize(new(1, 99));
            _unitPathingController.InitializeUnit(unit);
            _unitPathingController.SetStoppingDistance(0.2f);

            if (!TryGetComponent(out _unitGrabController)) return;
            _unitGrabController.Initialize();
            _unitGrabController.OnGrabbed += Grabbed;
            _unitGrabController.OnReleased += Released;
        }

        void OnDestroy()
        {
            _unitGrabController.OnGrabbed -= Grabbed;
            _unitGrabController.OnReleased -= Released;
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

        void Released()
        {
            int maxColliders = 10;
            Collider[] results = new Collider[maxColliders];
            Physics.OverlapSphereNonAlloc(transform.position, 1.5f, results);
            foreach (Collider col in results)
            {
                if (col == null) continue;
                if (col.TryGetComponent(out UnitDropZoneController zone))
                    if (zone.DropOff(this))
                        return;
            }

            Debug.Log("No drop zone found");
            StartCampCoroutine();
        }

        void Grabbed()
        {
            OnGrabbed?.Invoke(this);
            if (_campCoroutine != null) StopCoroutine(_campCoroutine);
        }

        // TODO: building coroutines - probably a bad idea to handle it like that
        void BaseBuildingAssignment()
        {
            _unitPathingController.Stop();
            _hero.Army.Remove(Unit);
        }


        public void StartGoldMineCoroutine(Vector3 minePosition, Vector3 dropOffPosition)
        {
            BaseBuildingAssignment();
            if (_campCoroutine != null) StopCoroutine(_campCoroutine);
            _campCoroutine = GoldMineCoroutine(minePosition, dropOffPosition);
            StartCoroutine(_campCoroutine);
        }

        IEnumerator GoldMineCoroutine(Vector3 minePosition, Vector3 dropOffPosition)
        {
            while (true)
            {
                if (this == null) yield break;

                yield return new WaitForSeconds(Random.Range(1f, 3f));
                // mine
                yield return _unitPathingController.PathToPositionAndStop(minePosition);
                for (int i = 0; i < Random.Range(4, 8); i++)
                {
                    _animator.SetTrigger(AnimAttack);
                    yield return new WaitForSeconds(Random.Range(1f, 3f));
                }

                // drop off
                yield return _unitPathingController.PathToPositionAndStop(dropOffPosition);
            }
        }

        public void StartBlacksmithCoroutine(Vector3 anvilPosition)
        {
            BaseBuildingAssignment();
            if (_campCoroutine != null) StopCoroutine(_campCoroutine);
            _campCoroutine = BlacksmithCoroutine(anvilPosition);
            StartCoroutine(_campCoroutine);
        }

        IEnumerator BlacksmithCoroutine(Vector3 anvilPosition)
        {
            yield return _unitPathingController.PathToPositionAndStop(anvilPosition);

            while (true)
            {
                if (this == null) yield break;

                _animator.SetTrigger(AnimAttack);
                yield return new WaitForSeconds(Random.Range(1f, 3f));
            }
        }
    }
}