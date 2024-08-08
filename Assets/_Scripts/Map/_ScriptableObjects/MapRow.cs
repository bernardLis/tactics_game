using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    public class MapRow : BaseScriptableObject
    {
        public List<MapNode> Nodes = new();

        public void Initialize(int nodeCount, int rowNumber)
        {
            Debug.Log($"Initializing row {rowNumber} with {nodeCount} nodes");
            for (int i = 0; i < nodeCount; i++)
            {
                MapNode mn = CreateInstance<MapNode>();

                float x = GetNodeXPosition(i, nodeCount);

                int z = rowNumber * Random.Range(7, 12);
                mn.MapPosition = new(x, 0.5f, z);
                Nodes.Add(mn);
            }
        }

        float GetNodeXPosition(int currentNode, int nodeCount)
        {
            // TODO: there must be a better way xD
            if (nodeCount == 2 && currentNode == 0)
                return -2.5f + Random.Range(-0.5f, 0.5f);
            if (nodeCount == 2 && currentNode == 1)
                return 2.5f + Random.Range(-0.5f, 0.5f);

            if (nodeCount == 3 && currentNode == 0)
                return -5 + Random.Range(-0.5f, 0.5f);
            if (nodeCount == 3 && currentNode == 1)
                return 0 + Random.Range(-0.5f, 0.5f);
            if (nodeCount == 3 && currentNode == 2)
                return 5 + Random.Range(-0.5f, 0.5f);

            return 0;
        }
    }
}