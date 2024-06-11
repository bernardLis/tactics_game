using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Pawn
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Pawn/Pawn Upgrade")]
    public class PawnUpgrade : BaseScriptableObject
    {
        public string Name;
        public Sprite Icon;
        public int Price;
        public int LevelLimit;
        public List<Attack.Attack> Attacks = new();
        public PawnMission Mission;

        public int BaseHealth;
        public int BaseArmor;
        public float BaseSpeed;
        public int BasePower;
    }
}