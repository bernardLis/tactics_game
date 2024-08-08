using System.Collections.Generic;
using System.Linq;
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

        public MapNode Node { get; private set; }

        List<NodeController> _connectedNodes = new();
        readonly List<MapNodePaths> _pathsToNodes = new();

        [SerializeField] GameObject _visitedIcon;
        [SerializeField] RectTransform _nameFrame;
        [SerializeField] TMP_Text _nameText;
        [SerializeField] Image _natureIcon;

        SpriteRenderer _gfx;

        public void Initialize(MapNode node)
        {
            _mapManager = MapManager.Instance;
            _playerController = PlayerController.Instance;
            Node = node;

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

        public void ResolveConnections()
        {
            _connectedNodes = _mapManager.GetConnectedNodes(Node);

            foreach (MapNodeConnection mnc in Node.Connections)
            {
                foreach (NodeController n in _connectedNodes)
                {
                    if (n.Node.Id != mnc.Node.Id) continue;

                    SplineContainer sc = Instantiate(mnc.Path, transform).GetComponent<SplineContainer>();
                    SplinePath path = new(new[]
                    {
                        new SplineSlice<Spline>(sc.Splines[0], new(0, sc.Splines[0].Count),
                            sc.transform.localToWorldMatrix),
                    });

                    Spline reversedSpline = new() { Knots = sc.Splines[0].Reverse().ToArray() };
                    SplinePath reversedPath = new(new[]
                    {
                        new SplineSlice<Spline>(reversedSpline, new(0, reversedSpline.Count),
                            sc.transform.localToWorldMatrix),
                    });

                    _pathsToNodes.Add(new(n, path, reversedPath));
                    break;
                }
            }
        }

        public MapNodePaths GetPathTo(NodeController nodeController)
        {
            foreach (MapNodePaths mnp in _pathsToNodes)
                if (nodeController == mnp.Node)
                    return mnp;

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
            gm.Campaign.SetCurrentHeroNode(Node);

            if (Node.IsVisited) return;
            Node.IsVisited = true;
            _visitedIcon.SetActive(false);

            if (Node.Arena == null) return;
            gm.Campaign.SetCurrentArena(Node.Arena);
            gm.LoadScene(Scenes.Arena);
        }
    }

    public struct MapNodePaths
    {
        public MapNodePaths(NodeController node, SplinePath path, SplinePath reversedPath)
        {
            Node = node;
            Path = path;
            ReversedPath = reversedPath;
        }

        public readonly NodeController Node;
        public readonly SplinePath Path;
        public readonly SplinePath ReversedPath;
    }
}