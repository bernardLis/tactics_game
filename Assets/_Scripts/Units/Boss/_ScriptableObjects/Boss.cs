using System.Collections.Generic;
using Lis.Units.Boss.Attack;
using UnityEngine;

namespace Lis.Units.Boss
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Boss")]
    public class Boss : UnitMovement
    {
        [Header("Attacks")]
        public List<Attack.Attack> Attacks = new();
    }
}