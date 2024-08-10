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

        PlayerController _playerController;

        readonly List<NodeController> _nodeControllers = new();
        NodeControllerGrid _mapGrid;

        public VisualElement ButtonContainer;

        public void Start()
        {
            _playerController = PlayerController.Instance;
            _campaign = GameManager.Instance.Campaign;
            _map = _campaign.Map;

            // HERE: testing
            GameManager.Instance.ChangeGoldValue(1000);

            ButtonContainer =
                GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("campButtonContainer");
            ButtonContainer.Add(
                new MyButton("Go To Camp", "common__button", () => GameManager.Instance.LoadScene(Scenes.Camp)));

            SetUpMap();
        }

        void SetUpMap()
        {
            GenerateNodes();
            CreateConnections();

            _playerController.transform.position = _campaign.CurrentHeroNode.MapPosition;
            ResolveNodes(_playerController.CurrentNode);
            _playerController.CurrentNode.SetCurrentNode();
        }

        void GenerateNodes()
        {
            _mapGrid = new();
            _mapGrid.Rows = new();

            foreach (MapRow r in _map.MapRows)
            {
                NodeControllerRow currentRow = new();
                currentRow.Nodes = new();
                _mapGrid.Rows.Add(currentRow);
                foreach (MapNode mn in r.Nodes)
                {
                    NodeController nc = Instantiate(mn.NodePrefab, _nodeParent).GetComponent<NodeController>();
                    nc.Initialize(mn);
                    _nodeControllers.Add(nc);

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
            if (thisRowCount == 1 || nextRowCount == 1) return true;
            if (thisRowCount % 2 == 0 && thisRowCount == nextRowCount)
                return thisNodePosition == nextNodePosition;

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

            return true;
        }

        public void ResolveNodes(NodeController currentNode)
        {
            for (int i = 0; i < _mapGrid.Rows.Count; i++)
                foreach (NodeController nc in _mapGrid.Rows[i].Nodes)
                    if (i <= currentNode.Node.Row)
                        nc.SetUnavailable();

            foreach (NodeController nc in _nodeControllers)
                if (nc.IsConnectedTo(currentNode))
                    nc.SetAvailable();
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