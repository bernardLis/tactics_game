

using UnityEngine;

namespace Lis
{
    public class UpgradeManager : MonoBehaviour
    {

        public void ShowUpgradeMenu()
        {
            new UpgradeScreen(GameManager.Instance.UpgradeBoard);
        }
    }
}
