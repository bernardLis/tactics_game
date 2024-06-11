using UnityEngine;

namespace Lis.Core
{
    [CreateAssetMenu(fileName = "New Cursor", menuName = "ScriptableObject/Cursor")]
    public class MyCursor : BaseScriptableObject
    {
        public Texture2D Texture;
    }
}