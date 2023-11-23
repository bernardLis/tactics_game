using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUpgradeManager : MonoBehaviour
{

    public void ShowGlobalUpgradesMenu()
    {
        new GlobalUpgradesScreen(GameManager.Instance.GlobalUpgradeBoard);
    }
}
