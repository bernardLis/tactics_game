using System.Collections.Generic;
using Lis.Battle.Arena.Building;
using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/MapNode")]
    public class MapNode : BaseScriptableObject
    {
        public Sprite Icon;
        public string Name;
        public Nature Nature;

        public Vector3 NameFramePosition;

        public Vector2 MapPosition;
        public bool IsUnlocked;
        public List<MapNode> ConnectedNodes;

        public Arena Arena;

        // TODO: possibly cutscene


        public bool IsConnectedTo(MapNode ncNode)
        {
            if (ConnectedNodes.Contains(ncNode)) return true;

            foreach (MapNode n in ConnectedNodes)
                if (n.Id == ncNode.Id)
                    return true;

            return false;
        }
    }
}