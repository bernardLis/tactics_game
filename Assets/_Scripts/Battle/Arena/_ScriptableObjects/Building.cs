using Lis.Core;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Building")]
    public class Building : BaseScriptableObject
    {
        public int Level;
        public int MaxLevel;

        public void Upgrade()
        {
            if (Level < MaxLevel)
                Level++;
        }
    }
}