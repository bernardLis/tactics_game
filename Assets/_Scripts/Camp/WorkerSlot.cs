using Lis.Units;
using UnityEngine;
using UnityEngine.UI;

namespace Lis.Camp
{
    public class WorkerSlot : MonoBehaviour
    {
        [HideInInspector] public Unit Unit;
        [SerializeField] Image _workerImage;

        public void AssignWorker(Unit unit)
        {
            Unit = unit;
            _workerImage.gameObject.SetActive(true);
            _workerImage.sprite = unit.Icon;
        }

        public void UnassignWorker()
        {
            _workerImage.gameObject.SetActive(false);
            Unit = null;
        }
    }
}