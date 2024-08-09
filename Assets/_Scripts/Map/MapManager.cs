using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Map
{
    public class MapManager : Singleton<MapManager>
    {
        Campaign _campaign;
        Map _map;

        [SerializeField] Transform _nodeParent;
        public Transform PathsParent;

        [SerializeField] NodeController _mapNodePrefab;
        PlayerController _playerController;

        readonly List<NodeController> _nodeControllers = new();
        NodeControllerGrid _mapGrid;

        public void Start()
        {
            _playerController = PlayerController.Instance;
            _campaign = GameManager.Instance.Campaign;
            _map = _campaign.Map;

            SetUpMap();

            VisualElement container =
                GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("campButtonContainer");
            container.Add(
                new MyButton("Go To Camp", "common__button", () => GameManager.Instance.LoadScene(Scenes.Camp)));
        }

        void SetUpMap()
        {
            GenerateNodes();
            CreateConnections();

            _playerController.transform.position = _campaign.CurrentHeroNode.MapPosition;
        }

        void GenerateNodes()
        {
            // also create a grid
            _mapGrid = new();
            _mapGrid.Rows = new();

            foreach (MapRow r in _map.MapRows)
            {
                NodeControllerRow currentRow = new();
                currentRow.Nodes = new();
                _mapGrid.Rows.Add(currentRow);
                foreach (MapNode mn in r.Nodes)
                {
                    NodeController nc = Instantiate(_mapNodePrefab, _nodeParent);
                    nc.Initialize(mn);
                    _nodeControllers.Add(nc);

                    // add node to map grid
                    currentRow.Nodes.Add(nc);

                    if (mn == _campaign.CurrentHeroNode)
                        _playerController.CurrentNode = nc;
                }
            }
        }

        void CreateConnections()
        {
            for (int i = _mapGrid.Rows.Count - 1; i > 0; i--)
            {
                for (int j = 0; j < _mapGrid.Rows[i].Nodes.Count; j++)
                {
                    for (int k = 0; k < _mapGrid.Rows[i - 1].Nodes.Count; k++)
                    {
                        if (CanConnect(_mapGrid.Rows[i].Nodes.Count, _mapGrid.Rows[i - 1].Nodes.Count,
                                j, k))
                            _mapGrid.Rows[i].Nodes[j].ConnectTo(_mapGrid.Rows[i - 1].Nodes[k]);
                    }
                }
            }
        }

        bool CanConnect(int thisRowCount, int nextRowCount, int thisNodePosition, int nextNodePosition)
        {
            // HERE: ask Jacek for help
            if (nextRowCount == 1 || thisRowCount == 1) return true;

            if (thisRowCount == 2 && nextRowCount == 2)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0;
                if (thisNodePosition == 1)
                    return nextNodePosition is 1;
            }

            if (thisRowCount == 2 && nextRowCount == 3)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0 or 1;
                if (thisNodePosition == 1)
                    return nextNodePosition is 1 or 2;
            }

            if (thisRowCount == 2 && nextRowCount == 4)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0 or 1;
                if (thisNodePosition == 1)
                    return nextNodePosition is 2 or 3;
            }

            if (thisRowCount == 3 && nextRowCount == 2)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0;
                if (thisNodePosition == 1)
                    return nextNodePosition is 0 or 1;
                if (thisNodePosition == 2)
                    return nextNodePosition is 1;
            }

            if (thisRowCount == 3 && nextRowCount == 3)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0 or 1;
                if (thisNodePosition == 1)
                    return nextNodePosition is 1;
                if (thisNodePosition == 2)
                    return nextNodePosition is 1 or 2;
            }

            if (thisRowCount == 3 && nextRowCount == 4)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0 or 1;
                if (thisNodePosition == 1)
                    return nextNodePosition is 1 or 2;
                if (thisNodePosition == 2)
                    return nextNodePosition is 2 or 3;
            }

            if (thisRowCount == 4 && nextRowCount == 2)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0;
                if (thisNodePosition == 1)
                    return nextNodePosition is 0;
                if (thisNodePosition == 2)
                    return nextNodePosition is 1;
                if (thisNodePosition == 3)
                    return nextNodePosition is 1;
            }

            if (thisRowCount == 4 && nextRowCount == 3)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0;
                if (thisNodePosition == 1)
                    return nextNodePosition is 0 or 1;
                if (thisNodePosition == 2)
                    return nextNodePosition is 1 or 2;
                if (thisNodePosition == 3)
                    return nextNodePosition is 2;
            }

            if (thisRowCount == 4 && nextRowCount == 4)
            {
                if (thisNodePosition == 0)
                    return nextNodePosition is 0;
                if (thisNodePosition == 1)
                    return nextNodePosition is 1;
                if (thisNodePosition == 2)
                    return nextNodePosition is 2;
                if (thisNodePosition == 3)
                    return nextNodePosition is 3;
            }

            return true;
        }
    }

    public struct NodeControllerGrid
    {
        public List<NodeControllerRow> Rows;
    }

    public struct NodeControllerRow
    {
        public List<NodeController> Nodes;
    }
}