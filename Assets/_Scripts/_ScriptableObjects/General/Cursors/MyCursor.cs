using Lis.Core;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(fileName = "New Cursor", menuName = "ScriptableObject/Cursor")]
    public class MyCursor : BaseScriptableObject
    {
        public Texture2D Texture;
    }
}
