using System.Collections.Generic;
using Lis.Units.Boss.Attack;
using UnityEngine;

namespace Lis.Units.Boss
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Boss")]
    public class Boss : Unit
    {
        [Header("Attacks")]
        public List<Units.Attack.Attack> Attacks = new();
    }
}