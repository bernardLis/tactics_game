using System.Collections.Generic;
using Lis.Core;
using Lis.Map.MapNodes;
using UnityEngine;

namespace Lis.Map
{
    public class MapRow : BaseScriptableObject
    {
        GameDatabase _gameDatabase;
        public List<MapNode> Nodes = new();

        public void Initialize(int nodeCount, int rowNumber)
        {
            _gameDatabase = GameManager.Instance.GameDatabase;

            for (int i = 0; i < nodeCount; i++)
            {
                MapNode mn = SelectNodeType(rowNumber);
                mn.name = $"MapNode_{rowNumber}_{i}";

                float x = GetNodeXPosition(i, nodeCount);

                float z = rowNumber * 10 + Random.Range(-1, 1);
                mn.Initialize(new(x, 0, z), rowNumber);
                Nodes.Add(mn);
            }
        }

        MapNode SelectNodeType(int rowNumber)
        {
            if (rowNumber == 0)
                return Instantiate(_gameDatabase.MapNode);
            if (rowNumber == 1)
                return Instantiate(_gameDatabase.MapNodeChest); // HERE: testing

            // TODO: last row is a boss fight

            int value = Random.Range(0, 3);
            if (value == 0) return Instantiate(_gameDatabase.MapNodeFight);
            if (value == 1) return Instantiate(_gameDatabase.MapNodeShop);
            return Instantiate(_gameDatabase.MapNodeChest);
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