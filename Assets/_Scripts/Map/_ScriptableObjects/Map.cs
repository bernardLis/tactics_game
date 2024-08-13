using System.Collections.Generic;
using Lis.Core;
using Lis.Map.MapNodes;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map")]
    public class Map : BaseScriptableObject
    {
        [HideInInspector] public List<MapRow> MapRows = new();

        public void Initialize()
        {
            MapRows.Clear();

            int rowCount = Random.Range(8, 11);
            for (int i = 0; i < rowCount; i++)
            {
                MapRow mr = CreateInstance<MapRow>();

                int count = Random.Range(2, 5);
                if (i == 0) count = 1;
                if (i == rowCount - 1) count = 1;

                mr.Initialize(count, i);
                MapRows.Add(mr);
            }
        }

        public List<MapNode> GetAllNodes()
        {
            List<MapNode> nodes = new();

            foreach (MapRow row in MapRows)
            {
                foreach (MapNode node in row.Nodes)
                    nodes.Add(node);
            }

            return nodes;
        }
    }
}