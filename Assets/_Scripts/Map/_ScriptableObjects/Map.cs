using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map")]
    public class Map : BaseScriptableObject
    {
        public List<MapNode> OriginalNodes;
        [HideInInspector] public List<MapNode> Nodes;

        public void Initialize()
        {
            Nodes = new();
            foreach (MapNode n in OriginalNodes)
            {
                MapNode mn = Instantiate(n);
                Nodes.Add(mn);
            }
        }
    }
}