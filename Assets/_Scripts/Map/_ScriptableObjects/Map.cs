using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map")]
    public class Map : BaseScriptableObject
    {
        public List<MapNode> AllNodes;
        
    }
}