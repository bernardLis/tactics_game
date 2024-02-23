

using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Boss Attack")]
    public class BossAttack : BaseScriptableObject
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


        [Header("Battle GameObjects")]
        public GameObject BossAttackManagerPrefab;
        [HideInInspector] public BattleBossAttack BattleBossAttack;

        public bool UseElementalProjectile;

        public void Initialize(BattleBoss bb)
        {
            BattleBossAttack = Instantiate(BossAttackManagerPrefab, bb.transform).GetComponent<BattleBossAttack>();
            BattleBossAttack.Initialize(this, bb);
        }
    }
}
