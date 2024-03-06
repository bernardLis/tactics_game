using Lis.Battle.Tiles;
using Lis.Upgrades;
using UnityEngine;

namespace Lis
{
    public class GfxController : MonoBehaviour
    {
        [SerializeField] protected GameObject GfxHolder;


        void Awake()
        {
            GetComponent<Controller>().OnTileEnabled += OnTileEnabled;
            GetComponent<Controller>().OnTileUnlocked += OnTileUnlocked;
        }

        protected virtual void OnTileEnabled(UpgradeTile upgrade)
        {
        }

        protected virtual void OnTileUnlocked(Controller tile)
        {
        }
    }
}