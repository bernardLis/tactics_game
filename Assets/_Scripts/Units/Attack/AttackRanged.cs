using UnityEngine;

namespace Lis.Units.Attack
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Attacks/Attack Ranged")]
    public class AttackRanged : Attack
    {
        public GameObject ProjectilePrefab;
    }
}