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
    }
}