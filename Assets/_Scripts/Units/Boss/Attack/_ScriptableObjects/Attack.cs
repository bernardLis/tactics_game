using Lis.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Boss.Attack
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Boss/Attack")]
    public class Attack : BaseScriptableObject
    {
        public Sprite Icon;
        public string Description;

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

        [HideInInspector] public AttackController AttackController;

        public bool UseElementalProjectile;

        public void Initialize(BossController bb)
        {
            AttackController = Instantiate(Prefab, bb.transform).GetComponent<AttackController>();
            AttackController.Initialize(this, bb);
        }
    }
}