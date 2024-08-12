using System.Collections;
using Lis.Units;
using UnityEngine;

namespace Lis
{
    public class UnitCampController : MonoBehaviour
    {
        UnitPathingController _unitPathingController;

        public void Initialize(Unit unit)
        {
            _unitPathingController = GetComponent<UnitPathingController>();
            _unitPathingController.Initialize(default);
            _unitPathingController.InitializeUnit(unit);

            StartCoroutine(CampCoroutine());
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
    }
}