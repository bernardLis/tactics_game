using Lis.Core;
using Lis.Units;
using UnityEngine;

namespace Lis
{
    public class MinionMaterialSetter : MonoBehaviour
    {
        [SerializeField] MeshRenderer[] _meshRenderers;
        [SerializeField] SkinnedMeshRenderer[] _skinnedMeshRenderers;

        public void SetMaterial(Unit unit)
        {
            Material material = GameManager.Instance.UnitDatabase.GetMinionMaterialByNature(unit.Nature.NatureName);
            foreach (MeshRenderer rend in _meshRenderers)
                rend.material = material;
            foreach (SkinnedMeshRenderer sRend in _skinnedMeshRenderers)
                sRend.material = material;
        }
    }
}