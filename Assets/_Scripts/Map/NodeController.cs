﻿using System.Collections.Generic;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using Shapes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace Lis.Map
{
    public class NodeController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        GameManager _gameManager;
        MapManager _mapManager;
        PlayerController _playerController;

        public MapNode Node;

        readonly List<MapNodePaths> _pathsToNodes = new();

        bool _isUnavailable;

        [SerializeField] GameObject _splinePrefab;
        [SerializeField] Image _icon;
        [SerializeField] Transform _gfx;
        [SerializeField] Disc _disc;

        public void Initialize(MapNode node)
        {
            _gameManager = GameManager.Instance;
            _mapManager = MapManager.Instance;
            _playerController = PlayerController.Instance;
            Node = node;

            name = node.name;
            transform.position = node.MapPosition;

            if (node.IsVisited)
            {
                _disc.gameObject.SetActive(true);
                SetUnavailable();
            }
        }

        public void ConnectTo(NodeController node)
        {
            GameObject splineGameObject = Instantiate(_splinePrefab, _mapManager.PathsParent);
            SplineContainer sc = splineGameObject.GetComponent<SplineContainer>();
            sc.Splines[0].Clear();
            sc.Splines[0].Add(node.transform.position);
            sc.Splines[0].Add(transform.position);
            SplinePath path = new(new[]
            {
                new SplineSlice<Spline>(sc.Splines[0], new(0, sc.Splines[0].Count),
                    sc.transform.localToWorldMatrix),
            });

            _pathsToNodes.Add(new(node, path));
        }

        public bool IsConnectedTo(NodeController nodeController)
        {
            foreach (MapNodePaths mnp in _pathsToNodes)
                if (nodeController == mnp.Node)
                    return true;

            return false;
        }

        public SplinePath GetPathTo(NodeController nodeController)
        {
            foreach (MapNodePaths mnp in _pathsToNodes)
                if (nodeController == mnp.Node)
                    return mnp.Path;

            Debug.LogError("Path not found");
            return default;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked on " + name);
            if (!_playerController.TryMovingPlayerToNode(this))
                _icon.transform.DOShakePosition(0.5f, Vector2.one * 0.1f);
        }

        public void SetCurrentNode()
        {
            _gameManager.Campaign.SetCurrentHeroNode(Node);

            _disc.gameObject.SetActive(true);
            DOTween.To(x => _disc.DashOffset = x, 0, 1, 0.5f)
                .SetLoops(-1, LoopType.Restart)
                .SetId("current node disc tween");

            if (Node.IsVisited) return;

            Node.IsVisited = true;
            ResolveNode();
        }

        void ResolveNode()
        {
            if (Node is MapNodeFight mapNodeFight)
            {
                _gameManager.Campaign.SetCurrentArena(mapNodeFight.Arena);
                _gameManager.LoadScene(Scenes.Arena);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Node.IsVisited) return;
            if (_isUnavailable) return;

            _icon.color = new(1, 1, 1, 1);
            _icon.transform.DOScale(1.1f, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Node.IsVisited) return;
            if (_isUnavailable) return;

            _icon.color = new(1, 1, 1, 0.8f);
            _icon.transform.DOScale(0.9f, 0.2f);
        }

        public void SetUnavailable()
        {
            _icon.transform.DOKill();

            _isUnavailable = true;

            _icon.transform.DOLocalMoveY(-94, 0.2f);
            _icon.transform.DOLocalRotate(new(90f, 0f, 0f), 0.2f);

            _icon.color = new(1, 1, 1, 0.5f);
            _icon.transform.DOScale(0.8f, 0.2f);
        }

        public void SetAvailable()
        {
            _icon.transform.DOLocalMoveY(50, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    public struct MapNodePaths
    {
        public MapNodePaths(NodeController node, SplinePath path)
        {
            Node = node;
            Path = path;
        }

        public readonly NodeController Node;
        public readonly SplinePath Path;
    }
}