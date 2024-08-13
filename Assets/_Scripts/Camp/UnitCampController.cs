using System.Collections;
using Lis.Units;
using UnityEngine;

namespace Lis.Camp
{
    public class UnitCampController : MonoBehaviour
    {
        UnitPathingController _unitPathingController;

        IEnumerator _campCoroutine;

        public void Initialize(Unit unit)
        {
            _unitPathingController = GetComponent<UnitPathingController>();
            _unitPathingController.Initialize(default);
            _unitPathingController.InitializeUnit(unit);

            StartCampCoroutine();

            if (!TryGetComponent(out UnitGrabController grab)) return;
            grab.Initialize();
            grab.OnGrabbed += OnGrabbed;
            grab.OnReleased += OnReleased;
        }

        void StartCampCoroutine()
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
            StartCampCoroutine();
        }

        void OnGrabbed()
        {
            if (_campCoroutine != null) StopCoroutine(_campCoroutine);
        }
    }
}