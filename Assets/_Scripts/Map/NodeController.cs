using System.Collections.Generic;
using DG.Tweening;
using Lis.Core;
using Lis.Map.MapNodes;
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
        protected MapManager MapManager;
        protected PlayerController PlayerController;

        [HideInInspector] public MapNode Node;

        readonly List<MapNodePaths> _pathsToNodes = new();

        bool _isUnavailable;

        [SerializeField] GameObject _splinePrefab;
        [SerializeField] protected Image Icon;
        [SerializeField] Disc _disc;
        [SerializeField] protected Transform Gfx;

        public void Initialize(MapNode node)
        {
            _gameManager = GameManager.Instance;
            MapManager = MapManager.Instance;
            PlayerController = PlayerController.Instance;
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
            GameObject splineGameObject = Instantiate(_splinePrefab, MapManager.PathsParent);
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

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (this == PlayerController.CurrentNode) return;

            if (!PlayerController.TryMovingPlayerToNode(this))
                Icon.transform.DOShakePosition(0.5f, Vector2.one * 0.1f);
        }

        public void SetCurrentNode()
        {
            _gameManager.Campaign.SetCurrentHeroNode(Node);

            _disc.gameObject.SetActive(true);
            DOTween.To(x => _disc.DashOffset = x, 0, 1, 0.5f)
                .SetLoops(-1, LoopType.Restart)
                .SetId("current node disc tween");

            ResolveNode();
            Node.IsVisited = true;
        }

        public virtual void LeaveNode()
        {
        }

        protected virtual void ResolveNode()
        {
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (Node.IsVisited) return;
            if (_isUnavailable) return;
            Gfx.DOScale(1.2f, 0.5f)
                .SetEase(Ease.InOutBack);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (Node.IsVisited) return;
            if (_isUnavailable) return;

            Gfx.DOScale(1f, 0.5f)
                .SetEase(Ease.InOutBack);
        }

        public virtual void SetAvailable()
        {
            Icon.transform.DOLocalMoveY(50, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public virtual void SetUnavailable()
        {
            Icon.transform.DOKill();

            _isUnavailable = true;

            Icon.transform.DOLocalMoveY(-94, 0.2f);
            Icon.transform.DOLocalRotate(new(90f, 0f, 0f), 0.2f);

            Icon.color = new(1, 1, 1, 0.5f);
            Icon.transform.DOScale(0.8f, 0.2f);

            if (this == PlayerController.CurrentNode) return;
            Gfx.DOScale(0f, 0.5f).SetEase(Ease.InOutBack);
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