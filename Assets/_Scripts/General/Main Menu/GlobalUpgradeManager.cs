using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUpgradeManager : MonoBehaviour
{
    [SerializeField] GlobalUpgradeBoard _globalUpgradeBoard;

    public void ShowGlobalUpgradesMenu()
    {
        new GlobalUpgradesScreen(_globalUpgradeBoard);
    }
}
