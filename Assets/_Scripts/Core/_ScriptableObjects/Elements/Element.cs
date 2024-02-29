using UnityEngine;

namespace Lis.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Element")]
    public class Element : BaseScriptableObject
    {
        public ElementName ElementName;
        public Sprite Icon;
        public ColorVariable Color;
        public string Description;
        public Element StrongAgainst;
        public Element WeakAgainst;

        public bool IsAdvanced;
    }
}
