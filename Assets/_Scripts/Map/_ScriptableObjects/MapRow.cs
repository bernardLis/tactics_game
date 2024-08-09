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
            for (int i = 0; i < nodeCount; i++)
            {
                MapNode mn = CreateInstance<MapNode>();
                mn.name = $"MapNode_{rowNumber}_{i}";

                float x = GetNodeXPosition(i, nodeCount);

                float z = rowNumber * 10 + Random.Range(-1, 1);
                mn.Initialize(new(x, 0, z), rowNumber);
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