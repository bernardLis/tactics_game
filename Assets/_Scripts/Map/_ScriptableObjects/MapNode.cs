using System;
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

        public List<MapNodeConnection> Connections;

        public Arena Arena;

        // TODO: possibly cutscene


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