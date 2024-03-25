using Lis.Core;
using Lis.Units;
using UnityEngine;

namespace Lis
{
    public class MinionMaterialSetter : MonoBehaviour
    {
        [SerializeField] MeshRenderer[] _meshRenderers;
        [SerializeField] SkinnedMeshRenderer[] _skinnedMeshRenderers;

        [SerializeField] Material _neutralMaterial;
        [SerializeField] Material _earthMaterial;
        [SerializeField] Material _fireMaterial;
        [SerializeField] Material _waterMaterial;
        [SerializeField] Material _windMaterial;


        public void SetMaterial(Unit unit)
        {
            Material material = GetMaterialByNatureName(unit.Nature.NatureName);
            foreach (MeshRenderer rend in _meshRenderers)
                rend.material = material;
            foreach (SkinnedMeshRenderer sRend in _skinnedMeshRenderers)
                sRend.material = material;
        }

        Material GetMaterialByNatureName(NatureName natureName)
        {
            switch (natureName)
            {
                case NatureName.Earth:
                    return _earthMaterial;
                case NatureName.Fire:
                    return _fireMaterial;
                case NatureName.Water:
                    return _waterMaterial;
                case NatureName.Wind:
                    return _windMaterial;
                default:
                    return _neutralMaterial;
            }
        }
    }
}