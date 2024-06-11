using Lis.Core;
using UnityEngine;

namespace Lis.Upgrades
{
    public class UpgradeManager : MonoBehaviour
    {
        public void ShowUpgradeMenu()
        {
            new UpgradeScreen(GameManager.Instance.UpgradeBoard);
        }
    }
}