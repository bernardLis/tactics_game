using UnityEngine;

public class DashboardBuilding : MonoBehaviour
{
    [SerializeField] DashboardBuildingType DashboardBuildingType;
    DashboardManager _dashboardManager;
    
    void Start() { _dashboardManager = DashboardManager.Instance; }
    void OnTriggerEnter2D(Collider2D other) { _dashboardManager.OpenDashboardBuilding(DashboardBuildingType); }

}
