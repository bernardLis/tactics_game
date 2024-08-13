using Lis.Camp.Building;
using UnityEngine;

namespace Lis.Camp
{
    public class UnitDropZoneController : MonoBehaviour
    {
        BuildingController _buildingController;

        public void Initialize(BuildingController buildingController)
        {
            _buildingController = buildingController;
        }

        public void DropOff(UnitCampController ucc)
        {
            _buildingController.AssignUnit(ucc);
        }
    }
}