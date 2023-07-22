using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurretsManager : MonoBehaviour
{
    [Header("Turrets")]
    [SerializeField] Turret[] _allTurretsOriginal;

    BattleManager _battleManager;

    [SerializeField] List<BattleTurret> _turretInstances = new();


    void Start()
    {
        _battleManager = BattleManager.Instance;
    }

    public void InstantiateTurret(Element element)
    {
        foreach (Turret t in _allTurretsOriginal)
        {
            if (t.Element != element)
                continue;

            Turret scriptableObjectInstance = Instantiate(t);
            GameObject gameObjectInstance = Instantiate(scriptableObjectInstance.Prefab, _battleManager.EntityHolder);
            BattleTurret instance = gameObjectInstance.GetComponent<BattleTurret>();
            instance.Initialize(scriptableObjectInstance);
            _turretInstances.Add(instance);

            gameObjectInstance.transform.position = new Vector3(0, 0, 2);
        }
    }

    public Turret GetTurret(Element element)
    {
        foreach (BattleTurret t in _turretInstances)
        {
            if (t.Turret.Element != element)
                continue;

            return t.Turret;
        }

        return null;
    }

    public void UpgradeTurret(Element el)
    {
        foreach (BattleTurret t in _turretInstances)
        {
            if (t.Turret.Element != el)
                continue;

            t.Turret.PurchaseUpgrade();
        }
    }

    public void SpecialUpgradePurchased(Element el)
    {
        foreach (BattleTurret t in _turretInstances)
        {
            if (t.Turret.Element != el)
                continue;

            t.Turret.PurchaseSpecialUpgrade();
        }
    }
}
