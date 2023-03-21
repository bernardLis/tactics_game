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

    Vector3 _middleOfDestinationTile;
    Vector3 _reachableDestination;

    bool _interactionResolved;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _cam = Camera.main;
        _cameraSmoothFollow = _cam.GetComponent<CameraSmoothFollow>();
    }

    void Update()
    {
        if (_reachablePoints.Count > 0)
            _lineRendererReachable.material.SetTextureOffset("_MainTex", Vector2.left * Time.time);
        if (_unreachablePoints.Count > 0)
            _lineRendererUnreachable.material.SetTextureOffset("_MainTex", Vector2.left * Time.time);
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
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void OnDestroy()
    {
        if (_playerInput == null) return;

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
        UnselectHero();
    }

    void LeftMouseClick(InputAction.CallbackContext ctx)
    {
        if (this == null) return;

        ResetDestinationCollider();

        Vector2 worldPos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (_selectedHero != null)
        {
            ResolveMovement(worldPos);
            return;
        }

        Collider2D[] results = Physics2D.OverlapCircleAll(worldPos, 0.2f);
        foreach (Collider2D c in results)
            if (c.gameObject.TryGetComponent<MapHero>(out MapHero hero))
                SelectHero(hero);
    }

    void SelectHero(MapHero hero)
    {
        UnselectHero();
        _selectedHero = hero;
        _selectedHero.Select();

        _cameraSmoothFollow.MoveTo(_selectedHero.transform.position);

        _interactionResolved = false;

        if (_selectedHero.GetLastDestination() == Vector3.zero)
            return;

        _middleOfDestinationTile = _selectedHero.GetLastDestination();
        StartCoroutine(DrawPath());
    }

    void ResolveMovement(Vector2 worldPos)
    {
        Vector3Int tilePos = _tilemap.WorldToCell(worldPos);
        Vector3 middleOfTile = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f);
        // if you want to move on top of an object you need to disable the collider 
        ResolveDestinationCollider(middleOfTile);

        _selectedHero.SetLastDestination(middleOfTile);

        // click twice at the same location to move
        if (_middleOfDestinationTile != middleOfTile)
        {
            _middleOfDestinationTile = middleOfTile;
            StartCoroutine(DrawPath());
            return;
        }

        StartCoroutine(Path());
    }

    void ResolveDestinationCollider(Vector3 pos)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in results)
        {
            if (c.gameObject == _selectedHero.gameObject)
                continue;

            if (c.gameObject.TryGetComponent<MapCollectable>(out MapCollectable collectable))
                DisableDestinationCollider(c);

            if (c.gameObject.TryGetComponent<MapHero>(out MapHero hero))
                DisableDestinationCollider(c);

            if (c.gameObject.TryGetComponent<MapBattle>(out MapBattle battle))
                DisableDestinationCollider(c);
        }
    }

    IEnumerator DrawPath()
    {
        ClearMovementIndicators();

        Path fullPath = Pathfinding.ABPath.Construct(_selectedHero.transform.position, _middleOfDestinationTile);
        AstarPath.StartPath(fullPath);
        yield return StartCoroutine(fullPath.WaitForPath());

        for (int i = 0; i < fullPath.vectorPath.Count; i++)
        {
            if (_selectedHero == null) yield break;

            Vector3 pos = new Vector3(fullPath.vectorPath[i].x, fullPath.vectorPath[i].y, 0);
            Path lengthCheckPath = Pathfinding.ABPath.Construct(_selectedHero.transform.position, pos);
            AstarPath.StartPath(lengthCheckPath);
            yield return StartCoroutine(lengthCheckPath.WaitForPath());
            if (lengthCheckPath.error) yield break;

            if (lengthCheckPath.GetTotalLength() <= _selectedHero.RangeLeft)
            {
                _reachablePoints.Add(pos);
                _reachableDestination = pos;
            }
            else
            {
                _unreachablePoints.Add(pos);
            }
        }

        SetMovementIndicators();
    }

    void SetMovementIndicators()
    {
        _lineRendererReachable.positionCount = _reachablePoints.Count;
        _lineRendererReachable.SetPositions(_reachablePoints.ToArray());

        _unreachablePoints.Insert(0, _reachableDestination);
        _lineRendererUnreachable.positionCount = _unreachablePoints.Count;
        _lineRendererUnreachable.SetPositions(_unreachablePoints.ToArray());

        Vector3Int tilePos = _tilemap.WorldToCell(_reachableDestination);
        _tilemap.SetTileFlags(tilePos, TileFlags.None);
        _tilemap.SetColor(tilePos, Color.red);

        _reachablePointMarker.position = _reachableDestination;
        _reachablePointMarker.gameObject.SetActive(true);

        _destinationMarker.position = _middleOfDestinationTile;
        _destinationMarker.gameObject.SetActive(true);
    }

    IEnumerator Path()
    {
        Path p = _selectedHero.GetComponent<Seeker>().StartPath(_selectedHero.transform.position, _reachableDestination);
        yield return StartCoroutine(p.WaitForPath());
        if (p.error) yield break;
        if (_selectedHero == null) yield break;

        _selectedHero.UpdateRangeLeft(p.GetTotalLength());

        _ai = _selectedHero.GetComponent<AILerp>();
        _ai.canMove = true;

        _cameraSmoothFollow.SetTarget(_selectedHero.transform);

        while (!_ai.reachedEndOfPath)
        {
            if (_selectedHero == null) yield break;

            if (Vector3.Distance(_selectedHero.transform.position, _middleOfDestinationTile) < 0.8f
                && !_interactionResolved)
                ResolveInteraction();

            yield return UpdatePathIndicator();
            yield return new WaitForSeconds(0.05f);
        }
        OnTargetReached();
    }

    IEnumerator UpdatePathIndicator()
    {
        Path pathLeft = Pathfinding.ABPath.Construct(_selectedHero.transform.position, _reachableDestination);
        AstarPath.StartPath(pathLeft);
        yield return StartCoroutine(pathLeft.WaitForPath());
        if (pathLeft.error) yield break;
        if (pathLeft.vectorPath.Count == 0) yield break;

        _lineRendererReachable.positionCount = pathLeft.vectorPath.Count;
        _lineRendererReachable.SetPositions(pathLeft.vectorPath.ToArray());
    }


    void ResolveInteraction()
    {
        _interactionResolved = true;

        Collider2D[] results = Physics2D.OverlapCircleAll(_middleOfDestinationTile, 0.2f);
        foreach (Collider2D c in results)
        {
            if (c.gameObject == _selectedHero.gameObject)
                continue;

            // collectible => move into the tile and collect
            if (c.gameObject.TryGetComponent<MapCollectable>(out MapCollectable collectable))
                collectable.Collect(_selectedHero);

            // another hero => stay on the previous tile and "interact"
            if (c.gameObject.TryGetComponent<MapHero>(out MapHero hero))
                MeetHero(hero);

            // battle => load scene battle
            if (c.gameObject.TryGetComponent<MapBattle>(out MapBattle b))
                b.TakeBattle(_selectedHero);

        }
    }
    void MeetHero(MapHero hero)
    {
        _ai.canMove = false;
        Debug.Log($"Selected hero: {_selectedHero.name} is meeting a hero: {hero.name}");
        ResetDestinationCollider();
    }


    void OnTargetReached()
    {
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

    void UnselectHero()
    {
        if (_selectedHero == null) return;

        _selectedHero.Unselect();
        _selectedHero = null;
    }

    void DisableDestinationCollider(Collider2D c)
    {
        c.isTrigger = true;
        _disabledCollider = c;
        Bounds b = new(c.gameObject.transform.position, Vector3.one * 2);
        AstarPath.active.UpdateGraphs(b);
    }

    void ResetDestinationCollider()
    {
        if (_disabledCollider == null)
            return;

        _disabledCollider.isTrigger = false;
        Bounds b = new(_disabledCollider.transform.position, Vector3.one * 2);
        AstarPath.active.UpdateGraphs(b);
    }
}
