using Lis.Units.Boss;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Attack
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Attacks/Attack Boss")]
    public class AttackBoss : Attack
    {
        [Header("Attack Params")]
        public int CooldownSeconds = 5;

        public Vector2Int TotalShotCount = new(20, 50);
        public int GroupCount = 5;
        public float TotalAttackDuration = 5;
        public float Spread = 15;
        public float ProjectileDuration = 10;
        public float ProjectileSpeed = 5;
        public int ProjectilePower = 5;


        [FormerlySerializedAs("BossAttackManagerPrefab")] [Header("Battle GameObjects")]
        public GameObject Prefab;

        public bool UseElementalProjectile;

        public void Initialize(BossController bb)
        {
            // AttackController = Instantiate(Prefab, bb.transform).GetComponent<Boss.AttackController>();
            // AttackController.Initialize(this, bb);
        }
    }
}