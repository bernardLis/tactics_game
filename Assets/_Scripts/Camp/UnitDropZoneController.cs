using Lis.Camp.Building;
using Shapes;
using UnityEngine;
using DG.Tweening;

namespace Lis.Camp
{
    public class UnitDropZoneController : MonoBehaviour
    {
        [SerializeField] Disc _disc;
        BuildingController _buildingController;

        string _tweenId = "bla";

        public void Initialize(BuildingController buildingController)
        {
            _buildingController = buildingController;

            GrabManager gm = GrabManager.Instance;
            gm.OnGrabbed += OnGrabbed;
            gm.OnReleased += OnReleased;
        }

        public bool DropOff(UnitCampController ucc)
        {
            return _buildingController.AssignUnit(ucc);
        }

        void OnGrabbed()
        {
            if (_buildingController.Building.GetAssignedWorkerCount() >= _buildingController.Building.MaxWorkers)
                return;

            DOTween.To(x => _disc.DashOffset = x, 0, 1, 0.5f)
                .SetLoops(-1, LoopType.Restart).SetId(_tweenId);
        }

        void OnReleased()
        {
            DOTween.Kill(_tweenId);
        }
    }
}