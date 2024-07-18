using System.Collections.Generic;
using Lis.Core.Utilities;
using NaughtyAttributes;
using UnityEngine;

namespace Lis.Map
{
    public class MapManager : Singleton<MapManager>
    {
        [SerializeField] Map _map;

        [SerializeField] Transform _nodeParent;
        [SerializeField] NodeController _mapNodePrefab;
        [SerializeField] PlayerController _player;

        readonly List<NodeController> _nodeControllers = new();

        public void Start()
        {
            SetUpMap();
        }

        void SetUpMap()
        {
            foreach (MapNode mn in _map.AllNodes)
            {
                NodeController nc = Instantiate(_mapNodePrefab, _nodeParent);
                nc.Initialize(Instantiate(mn));
                _nodeControllers.Add(nc);
            }

            foreach (NodeController nc in _nodeControllers)
                nc.ResolveConnections();

            _player.transform.position = new(_map.AllNodes[0].MapPosition.x, _map.AllNodes[0].MapPosition.y, -1);
            _player.CurrentNode = _nodeControllers[0];
            _nodeControllers[0].Activate();
        }

        public List<NodeController> GetConnectedNodes(MapNode node)
        {
            List<NodeController> connectedNodes = new();
            foreach (NodeController nc in _nodeControllers)
                if (node.IsConnectedTo(nc.Node))
                    connectedNodes.Add(nc);

            return connectedNodes;
        }

        public bool TryMovingPlayerToNode(NodeController nodeController)
        {
            if (_player.CurrentNode == nodeController) return false;
            if (!_player.CurrentNode.Node.IsConnectedTo(nodeController.Node)) return false;

            _player.MoveTo(nodeController);
            return true;
        }
    }
}