using UnityEngine;

namespace Lis.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Nature")]
    public class Nature : BaseScriptableObject
    {
        public NatureName NatureName;
        public Sprite Icon;
        public ColorVariable Color;
        public string Description;
        public Nature StrongAgainst;
        public Nature WeakAgainst;

        public bool IsAdvanced;
    }
}