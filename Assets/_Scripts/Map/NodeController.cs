using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lis.Map
{
    public class NodeController : MonoBehaviour, IPointerClickHandler
    {
        MapManager _mapManager;

        [SerializeField] MapPathDrawer _pathDrawer;

        public MapNode Node { get; private set; }
        List<NodeController> _connectedNodes = new();
        List<MapPathDrawer> _paths = new();

        public void Initialize(MapNode node)
        {
            _mapManager = MapManager.Instance;
            Node = node;

            name = node.name;
            transform.position = new(node.MapPosition.x, node.MapPosition.y, -1);
        }

        public void ResolveConnections()
        {
            _connectedNodes = _mapManager.GetConnectedNodes(Node);
            foreach (NodeController n in _connectedNodes)
            {
                MapPathDrawer path = Instantiate(_pathDrawer);
                path.gameObject.SetActive(true);
                path.Initialize(this, n);
                _paths.Add(path);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked on " + name);
            if (!_mapManager.TryMovingPlayerToNode(this))
                transform.DOShakePosition(0.5f, Vector3.one * 0.1f);
        }

        public void Activate()
        {
            foreach (MapPathDrawer mpd in _paths)
                mpd.Activate();
        }

        public void Deactivate()
        {
            foreach (MapPathDrawer mpd in _paths)
                mpd.Deactivate();
        }
    }
}