using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Pawn
{
    using Attack;

    [CreateAssetMenu(menuName = "ScriptableObject/Units/Pawn/Pawn Upgrade")]
    public class PawnUpgrade : BaseScriptableObject
    {
        public string Name;
        public Sprite Icon;
        public int Price;
        public int LevelLimit;
        public List<Attack> Attacks = new();
        public PawnMission Mission;
    }
}