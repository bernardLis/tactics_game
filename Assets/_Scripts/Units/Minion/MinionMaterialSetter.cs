using System;
using System.Linq;
using Lis.Core;
using Lis.Units;
using UnityEngine;

namespace Lis
{
    public class MinionMaterialSetter : MonoBehaviour
    {
        [SerializeField] MeshRenderer[] _meshRenderers;
        [SerializeField] SkinnedMeshRenderer[] _skinnedMeshRenderers;

        [SerializeField] MinionMaterial[] _minionMaterials;

        public void SetMaterial(Unit unit)
        {
            Material material = GetMinionMaterialByNature(unit.Nature.NatureName);
            foreach (MeshRenderer rend in _meshRenderers)
                rend.material = material;
            foreach (SkinnedMeshRenderer sRend in _skinnedMeshRenderers)
                sRend.material = material;
        }

        Material GetMinionMaterialByNature(NatureName natureName)
        {
            return _minionMaterials.FirstOrDefault(x => x.NatureName == natureName).Material;
        }
    }
}

[Serializable]
public struct MinionMaterial
{
    public NatureName NatureName;
    public Material Material;
}