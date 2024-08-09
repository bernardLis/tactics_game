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
            if (nodeCount == 2)
                return -2.5f + (5 * currentNode) + Random.Range(-0.5f, 0.5f);

            if (nodeCount == 3)
                return -5 + (5 * currentNode) + Random.Range(-0.5f, 0.5f);

            if (nodeCount == 4)
                return -7.5f + (5 * currentNode) + Random.Range(-0.5f, 0.5f);


            return 0;
        }
    }
}