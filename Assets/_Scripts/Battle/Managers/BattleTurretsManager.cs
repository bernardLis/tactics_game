using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurretsManager : MonoBehaviour
{
    [Header("Turrets")]
    [SerializeField] Turret _earthTurretOriginal;
    [SerializeField] Turret _fireTurretOriginal;
    [SerializeField] Turret _waterTurretOriginal;
    [SerializeField] Turret _windTurretOriginal;

    BattleManager _battleManager;

    BattleTurret _earthTurretInstance;
    BattleTurret _fireTurretInstance;
    BattleTurret _waterTurretInstance;
    BattleTurret _windTurretInstance;

    void Start()
    {
        _battleManager = BattleManager.Instance;
    }

    public void InstantiateEarthTurret()
    {
        Turret scriptableObjectInstance = Instantiate(_earthTurretOriginal);
        GameObject gameObjectInstance = Instantiate(scriptableObjectInstance.Prefab, _battleManager.EntityHolder);

        _earthTurretInstance = gameObjectInstance.GetComponent<BattleTurret>();
        _earthTurretInstance.Initialize(scriptableObjectInstance);
        gameObjectInstance.transform.position = new Vector3(0, 0, 2);
    }

    public void UpgradeEarthTurret()
    {
        _earthTurretInstance.Turret.PurchaseUpgrade();
    }

    public void InstantiateFireTurret()
    {
        Turret scriptableObjectInstance = Instantiate(_fireTurretOriginal);
        GameObject gameObjectInstance = Instantiate(scriptableObjectInstance.Prefab, _battleManager.EntityHolder);
        _fireTurretInstance = gameObjectInstance.GetComponent<BattleTurret>();
        _fireTurretInstance.Initialize(scriptableObjectInstance);
        gameObjectInstance.transform.position = new Vector3(0, 0, -2);
    }

    public void UpgradeFireTurret()
    {
        _fireTurretInstance.Turret.PurchaseUpgrade();
    }

    public void InstantiateWaterTurret()
    {
        Turret scriptableObjectInstance = Instantiate(_waterTurretOriginal);
        GameObject gameObjectInstance = Instantiate(scriptableObjectInstance.Prefab, _battleManager.EntityHolder);
        _waterTurretInstance = gameObjectInstance.GetComponent<BattleTurret>();
        _waterTurretInstance.Initialize(scriptableObjectInstance);
        gameObjectInstance.transform.position = new Vector3(2, 0, 0);
    }

    public void UpgradeWaterTurret()
    {
        _waterTurretInstance.Turret.PurchaseUpgrade();
    }

    public void InstantiateWindTurret()
    {
        Turret scriptableObjectInstance = Instantiate(_windTurretOriginal);
        GameObject gameObjectInstance = Instantiate(scriptableObjectInstance.Prefab, _battleManager.EntityHolder);
        _windTurretInstance = gameObjectInstance.GetComponent<BattleTurret>();
        _windTurretInstance.Initialize(scriptableObjectInstance);
        gameObjectInstance.transform.position = new Vector3(-2, 0, 0);
    }

    public void UpgradeWindTurret()
    {
        _windTurretInstance.Turret.PurchaseUpgrade();
    }


}
