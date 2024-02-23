using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Boss")]
    public class Boss : EntityMovement
    {
        [Header("Attacks")]
        public List<BossAttack> Attacks = new();
    }
}