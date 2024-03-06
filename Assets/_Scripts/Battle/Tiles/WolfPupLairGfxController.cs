using Lis.Upgrades;
using UnityEngine;

namespace Lis
{
    public class WolfPupLairGfxController : GfxController
    {
        [SerializeField] GameObject _branchPrefab;

        protected override void OnTileEnabled(UpgradeTile upgrade)
        {
            base.OnTileEnabled(upgrade);
            PlaceBranches();
        }

        void PlaceBranches()
        {
            int numberOfBranches = Random.Range(3, 7);
            for (int i = 0; i < numberOfBranches; i++)
            {
                Vector3 position = new(Random.Range(-20f, 20f), Random.Range(0.1f, 0.2f),
                    Random.Range(-20f, 20f));
                GameObject branch = Instantiate(_branchPrefab, GfxHolder.transform);
                branch.transform.localPosition = position;
                branch.transform.localRotation =
                    Quaternion.Euler(Random.Range(-10, 10), Random.Range(0, 360), Random.Range(0, 360));
                branch.SetActive(true);
            }
        }
    }
}