using System.Collections.Generic;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Map
{
    public class MapManager : Singleton<MapManager>
    {
        [SerializeField] Map _map;

        [SerializeField] Transform _nodeParent;
        [SerializeField] NodeController _mapNodePrefab;
        PlayerController _playerController;

        readonly List<NodeController> _nodeControllers = new();

        public void Start()
        {
            _playerController = PlayerController.Instance;
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

            _playerController.transform.position = new(_map.AllNodes[0].MapPosition.x, _map.AllNodes[0].MapPosition.y, -2);
            _playerController.CurrentNode = _nodeControllers[0];
            _nodeControllers[0].Visited();
        }

        public List<NodeController> GetConnectedNodes(MapNode node)
        {
            List<NodeController> connectedNodes = new();
            foreach (NodeController nc in _nodeControllers)
                if (node.IsConnectedTo(nc.Node))
                    connectedNodes.Add(nc);

            return connectedNodes;
        }
    }
}