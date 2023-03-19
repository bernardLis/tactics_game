using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Pathfinding;
using Shapes;
using DG.Tweening;

public class MapMovementManager : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;
    CameraSmoothFollow _cameraSmoothFollow;
    Camera _cam;

    [SerializeField] Tilemap _tilemap;
    [SerializeField] Transform _reachablePointMarker;
    [SerializeField] Transform _destinationMarker;

    [SerializeField] LineRenderer _lineRendererReachable;
    [SerializeField] LineRenderer _lineRendererUnreachable;

    List<Vector3> _reachablePoints = new();
    List<Vector3> _unreachablePoints = new();

    MapHero _selectedHero;
    AILerp _ai;

    Collider2D _disabledCollider;

    Vector3Int _destinationPos;
    Vector3 _reachableDestination;
    Vector3 _desiredDestination;
    bool _interactionResolved;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _cam = Camera.main;
        _cameraSmoothFollow = _cam.GetComponent<CameraSmoothFollow>();
    }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Dashboard");
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;

        UnsubscribeInputActions();
    }

    void OnDestroy()
    {
        if (_playerInput == null)
            return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].performed += LeftMouseClick;
        _playerInput.actions["RightMouseClick"].performed += RightMouseClick;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].performed -= LeftMouseClick;
        _playerInput.actions["RightMouseClick"].performed -= RightMouseClick;
    }

    void RightMouseClick(InputAction.CallbackContext ctx)
    {
        ClearMovementIndicators();
        if (_selectedHero != null)
        {
            _selectedHero.Unselect();
            _selectedHero = null;
        }
    }

    void LeftMouseClick(InputAction.CallbackContext ctx)
    {
        if (_disabledCollider != null)
        {
            _disabledCollider.isTrigger = false;
            Bounds b = new(_disabledCollider.transform.position, Vector3.one * 2);
            AstarPath.active.UpdateGraphs(b);
        }


        Vector2 worldPos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (_selectedHero != null && this != null)
        {
            ResolveMovement(worldPos);
            return;
        }

        Collider2D[] results = Physics2D.OverlapCircleAll(worldPos, 0.2f);
        foreach (Collider2D c in results)
            if (c.CompareTag(Tags.Player))
                HeroClick(c.gameObject);
    }

    void HeroClick(GameObject obj)
    {
        _selectedHero = obj.GetComponent<MapHero>();
        _selectedHero.Select();

        Vector3 camPos = _selectedHero.transform.position;
        camPos.z = -10;
        _cameraSmoothFollow.transform.position = camPos;

        _interactionResolved = false;
    }

    void ResolveMovement(Vector2 worldPos)
    {
        _desiredDestination = worldPos;

        DisableCollider();

        Vector3Int tilePos = _tilemap.WorldToCell(worldPos);
        // click twice at the same location to move
        if (_destinationPos == tilePos)
        {
            StartCoroutine(Path());
            return;
        }

        _destinationPos = tilePos;
        StartCoroutine(DrawPath());
    }

    void DisableCollider()
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(_desiredDestination, 0.2f);
        foreach (Collider2D c in results)
        {
            if (c.gameObject == _selectedHero.gameObject)
                continue;

            // collectible => move into the tile and collect
            if (c.gameObject.TryGetComponent<MapCollectable>(out MapCollectable collectable))
            {
                Debug.Log($"disabling the collider");
                c.isTrigger = true;
                _disabledCollider = c;
                Bounds b = new(c.gameObject.transform.position, Vector3.one * 2);
                AstarPath.active.UpdateGraphs(b);
            }

            // another hero => stay on the previous tile and "interact"
            if (c.gameObject.TryGetComponent<MapHero>(out MapHero hero))
            {
                c.isTrigger = true;
                _disabledCollider = c;

                Debug.Log($"meeting a hero");
                break;
            }
        }

    }

    IEnumerator DrawPath()
    {
        ClearMovementIndicators();

        Vector3 middleOfTheTile = new Vector3(_destinationPos.x + 0.5f, _destinationPos.y + 0.5f);

        Path fullPath = Pathfinding.ABPath.Construct(_selectedHero.transform.position, middleOfTheTile);
        AstarPath.StartPath(fullPath);
        yield return StartCoroutine(fullPath.WaitForPath());

        for (int i = 0; i < fullPath.vectorPath.Count; i++)
        {
            Vector3 pos = new Vector3(fullPath.vectorPath[i].x, fullPath.vectorPath[i].y, 0);
            if (_selectedHero == null)
                yield break;
            Path lengthCheckPath = Pathfinding.ABPath.Construct(_selectedHero.transform.position, fullPath.vectorPath[i]);
            AstarPath.StartPath(lengthCheckPath);
            yield return StartCoroutine(lengthCheckPath.WaitForPath());
            if (lengthCheckPath.error)
                yield break;

            if (lengthCheckPath.GetTotalLength() <= _selectedHero.RangeLeft)
            {
                _reachablePoints.Add(fullPath.vectorPath[i]);
                _reachableDestination = pos;
            }
            else
            {
                _unreachablePoints.Add(fullPath.vectorPath[i]);
            }
        }

        SetMovementIndicators();
    }

    void SetMovementIndicators()
    {
        _unreachablePoints.Insert(0, _reachableDestination);

        _lineRendererReachable.positionCount = _reachablePoints.Count;
        _lineRendererReachable.SetPositions(_reachablePoints.ToArray());

        _lineRendererUnreachable.positionCount = _unreachablePoints.Count;
        _lineRendererUnreachable.SetPositions(_unreachablePoints.ToArray());

        Vector3Int tilePos = _tilemap.WorldToCell(_reachableDestination);
        _tilemap.SetTileFlags(tilePos, TileFlags.None);
        _tilemap.SetColor(tilePos, Color.red);

        _reachablePointMarker.position = _reachableDestination;
        _reachablePointMarker.gameObject.SetActive(true);

        _destinationMarker.position = new Vector3(_destinationPos.x + 0.5f, _destinationPos.y + 0.5f);
        _destinationMarker.gameObject.SetActive(true);
    }

    IEnumerator Path()
    {
        Path p = _selectedHero.GetComponent<Seeker>().StartPath(_selectedHero.transform.position, _reachableDestination);
        yield return StartCoroutine(p.WaitForPath());
        if (p.error)
            yield break;
        if (_selectedHero == null)
            yield break;

        _selectedHero.UpdateRangeLeft(p.GetTotalLength());

        _ai = _selectedHero.GetComponent<AILerp>();
        _ai.canMove = true;

        _cameraSmoothFollow.SetTarget(_selectedHero.transform);

        while (!_ai.reachedEndOfPath)
        {
            if (_selectedHero == null)
                yield break;
            Debug.Log($"desired dest dist: {Vector3.Distance(_selectedHero.transform.position, _desiredDestination)}");
            if (Vector3.Distance(_selectedHero.transform.position, _desiredDestination) < 0.5f && !_interactionResolved)
                yield return ResolveInteraction();

            if (_reachablePoints.Count > 0)
            {
                _reachablePoints.RemoveAt(0);
                _lineRendererReachable.SetPositions(_reachablePoints.ToArray());
            }
            yield return new WaitForSeconds(0.05f);
        }
        OnTargetReached();

    }

    IEnumerator ResolveInteraction()
    {
        _interactionResolved = true;
        Debug.Log($"resolving interaction");

        Collider2D[] results = Physics2D.OverlapCircleAll(_desiredDestination, 0.2f);
        foreach (Collider2D c in results)
        {
            if (c.gameObject == _selectedHero.gameObject)
                continue;

            // collectible => move into the tile and collect
            if (c.gameObject.TryGetComponent<MapCollectable>(out MapCollectable collectable))
            {
                collectable.Collect(_selectedHero);
                _ai.canMove = true;
                Path p = _selectedHero.GetComponent<Seeker>().StartPath(_selectedHero.transform.position, _desiredDestination);
                yield return StartCoroutine(p.WaitForPath());
                if (p.error)
                    yield break;

                while (!_ai.reachedEndOfPath)
                    yield return null;

                break;
            }

            // another hero => stay on the previous tile and "interact"
            if (c.gameObject.TryGetComponent<MapHero>(out MapHero hero))
            {
                Debug.Log($"meeting a hero");
                break;
            }
        }

        yield return null;

    }

    void OnTargetReached()
    {
        Debug.Log($"on target reached");
        ClearMovementIndicators();

        _selectedHero.UpdateMapPosition();
        _selectedHero.Unselect();
        _cameraSmoothFollow.SetTarget(null);
        _selectedHero = null;
        _ai = null;

        _gameManager.SaveJsonData();
    }

    void ClearMovementIndicators()
    {
        _reachablePoints = new();
        _unreachablePoints = new();

        _lineRendererReachable.positionCount = 0;
        _lineRendererUnreachable.positionCount = 0;

        Vector3Int tilePos = _tilemap.WorldToCell(_reachableDestination);
        _tilemap.SetColor(tilePos, Color.white);

        _reachablePointMarker.gameObject.SetActive(false);
        _destinationMarker.gameObject.SetActive(false);
    }
}
