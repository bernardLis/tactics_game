using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    public void ShowUpgradeMenu()
    {
        new UpgradeScreen(GameManager.Instance.UpgradeBoard);
    }
}
