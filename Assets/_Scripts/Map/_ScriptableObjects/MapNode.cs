using System;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/MapNode")]
    public class MapNode : BaseScriptableObject
    {
        public Vector3 MapPosition;
        public bool IsVisited;

        public List<MapNodeConnection> Connections;

        public Arena.Arena Arena;

        // TODO: possibly cutscene

        public void Initialize()
        {
        }

        public bool IsConnectedTo(MapNode ncNode)
        {
            foreach (MapNodeConnection mnc in Connections)
                if (mnc.Node.Id == ncNode.Id)
                    return true;

            return false;
        }
    }

    [Serializable]
    public struct MapNodeConnection
    {
        public MapNode Node;
        public GameObject Path;
    }
}