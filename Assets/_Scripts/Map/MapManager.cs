﻿using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Map
{
    public class MapManager : Singleton<MapManager>
    {
        Campaign _campaign;
        Map _map;

        [SerializeField] Transform _nodeParent;
        [SerializeField] NodeController _mapNodePrefab;
        PlayerController _playerController;

        readonly List<NodeController> _nodeControllers = new();

        public void Start()
        {
            _playerController = PlayerController.Instance;

            _campaign = GameManager.Instance.CurrentCampaign;
            _map = _campaign.Map;

            SetUpMap();
        }

        void SetUpMap()
        {
            foreach (MapNode mn in _map.Nodes)
            {
                NodeController nc = Instantiate(_mapNodePrefab, _nodeParent);
                nc.Initialize(mn);
                _nodeControllers.Add(nc);

                if (mn == _campaign.CurrentHeroNode)
                    _playerController.CurrentNode = nc;
            }

            foreach (NodeController nc in _nodeControllers)
                nc.ResolveConnections();

            _playerController.transform.position =
                new(_campaign.CurrentHeroNode.MapPosition.x, _campaign.CurrentHeroNode.MapPosition.y, -2);
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