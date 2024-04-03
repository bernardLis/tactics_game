using UnityEngine;

namespace Lis.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Core/Nature Advanced")]
    public class NatureAdvanced : Nature
    {
        public Nature FirstNature;
        public Nature SecondNature;
    }
}