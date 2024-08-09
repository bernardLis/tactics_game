using System.Collections.Generic;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace Lis.Map
{
    public class NodeController : MonoBehaviour, IPointerClickHandler
    {
        MapManager _mapManager;
        PlayerController _playerController;

        MapNode _node;

        readonly List<MapNodePaths> _pathsToNodes = new();

        [SerializeField] GameObject _splinePrefab;

        [SerializeField] GameObject _visitedIcon;
        [SerializeField] RectTransform _nameFrame;
        [SerializeField] TMP_Text _nameText;
        [SerializeField] Image _natureIcon;

        [SerializeField] Transform _gfx;

        public void Initialize(MapNode node)
        {
            _mapManager = MapManager.Instance;
            _playerController = PlayerController.Instance;
            _node = node;

            name = node.name;
            transform.position = node.MapPosition;

            if (!node.IsVisited)
            {
                _visitedIcon.transform.DOLocalMoveY(0, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                _visitedIcon.SetActive(false);
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
                _gfx.transform.DOShakePosition(0.5f, Vector2.one * 0.1f);
        }

        public void Visited()
        {
            GameManager gm = GameManager.Instance;
            gm.Campaign.SetCurrentHeroNode(_node);

            if (_node.IsVisited) return;
            _node.IsVisited = true;
            _visitedIcon.SetActive(false);

            if (_node.Arena == null) return;
            gm.Campaign.SetCurrentArena(_node.Arena);
            gm.LoadScene(Scenes.Arena);
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