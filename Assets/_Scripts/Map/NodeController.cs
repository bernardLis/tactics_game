﻿using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using UnityEngine.UI;
using Unity.Mathematics;

namespace Lis.Map
{
    public class NodeController : MonoBehaviour, IPointerClickHandler
    {
        MapManager _mapManager;
        PlayerController _playerController;

        [SerializeField] MapPathDrawer _pathDrawer;

        public MapNode Node { get; private set; }
        Dictionary<NodeController, SplinePath> _pathsToNodes = new();
        List<NodeController> _connectedNodes = new();
        List<MapPathDrawer> _paths = new();

        [SerializeField] GameObject _visitedIcon;
        [SerializeField] RectTransform _nameFrame;
        [SerializeField] TMP_Text _nameText;
        [SerializeField] Image _natureIcon;

        public void Initialize(MapNode node)
        {
            _mapManager = MapManager.Instance;
            _playerController = PlayerController.Instance;
            Node = node;

            name = node.name;
            transform.position = new(node.MapPosition.x, node.MapPosition.y, -1);

            _nameFrame.transform.localPosition = Node.NameFramePosition;
            GetComponent<SpriteRenderer>().sprite = node.Icon;
            _nameText.text = node.Name;
            _natureIcon.sprite = node.Nature.Icon;

            if (!node.IsUnlocked)
            {
                _visitedIcon.transform.DOLocalMoveY(0, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
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

            foreach (MapNodeConnection mnc in Node.Connections)
            {
                foreach (NodeController n in _connectedNodes)
                {
                    if (n.Node.Id == mnc.Node.Id)
                    {
                        SplineContainer sc = Instantiate(mnc.Path, transform).GetComponent<SplineContainer>();
                        SplinePath path = new(new[]
                        {
                            new SplineSlice<Spline>(sc.Splines[0], new(0, 99),
                                sc.transform.localToWorldMatrix),
                        });


                        _pathsToNodes.Add(n, path);
                        break;
                    }
                }
            }
        }


        public SplinePath GetPathTo(NodeController nodeController)
        {
            return _pathsToNodes[nodeController];
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked on " + name);
            if (!_playerController.TryMovingPlayerToNode(this))
                transform.DOShakePosition(0.5f, Vector3.one * 0.1f);
        }

        void Visited()
        {
            Node.IsUnlocked = true;
            _visitedIcon.SetActive(false);
        }

        public void Activate()
        {
            Visited();
            // foreach (MapPathDrawer mpd in _paths)
            //     mpd.Activate();
        }


        public void Deactivate()
        {
            // foreach (MapPathDrawer mpd in _paths)
            //     mpd.Deactivate();
        }
    }
}