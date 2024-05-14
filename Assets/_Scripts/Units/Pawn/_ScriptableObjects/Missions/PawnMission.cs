using Lis.Core;
using UnityEngine;

namespace Lis.Units.Pawn
{
    public class PawnMission : BaseScriptableObject
    {
        [HideInInspector] public bool IsCompleted;

        public virtual void Initialize(Pawn pawn)
        {
        }
    }
}