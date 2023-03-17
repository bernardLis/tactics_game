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

    [SerializeField] Line _lineReachable;
    [SerializeField] Line _lineUnreachable;

    MapHero _selectedHero;

    Vector3Int _destinationPos;
    Vector3 _reachablePoint;

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
        Vector2 worldPos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Collider2D[] results = Physics2D.OverlapCircleAll(worldPos, 0.2f);
        foreach (Collider2D c in results)
            if (c.CompareTag(Tags.Player))
                SelectHero(c.gameObject);

        if (_selectedHero != null)
            ResolveMovement(worldPos);
    }

    void SelectHero(GameObject obj)
    {
        if (_selectedHero != null)
            _selectedHero.Unselect();

        _selectedHero = obj.GetComponent<MapHero>();
        _selectedHero.Select();
        _cameraSmoothFollow.SetTarget(_selectedHero.transform);
    }

    void ResolveMovement(Vector2 worldPos)
    {
        Vector3Int tilePos = _tilemap.WorldToCell(worldPos);

        if (_destinationPos == tilePos)
        {
            StartCoroutine(Path());
            return;
        }
        _destinationPos = tilePos;
        StartCoroutine(DrawPath());
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

            if (lengthCheckPath.GetTotalLength() < _selectedHero.RangeLeft)
                _reachablePoint = pos;
        }

        SetMovementIndicators();
    }

    void SetMovementIndicators()
    {
        _lineReachable.Start = _selectedHero.transform.position;
        _lineReachable.End = _reachablePoint;

        _lineUnreachable.Start = _reachablePoint;
        _lineUnreachable.End = new Vector2(_destinationPos.x + 0.5f, _destinationPos.y + 0.5f);

        Vector3Int tilePos = _tilemap.WorldToCell(_reachablePoint);
        _tilemap.SetTileFlags(tilePos, TileFlags.None);
        _tilemap.SetColor(tilePos, Color.red);

        _reachablePointMarker.position = _reachablePoint;
        _reachablePointMarker.gameObject.SetActive(true);

        _destinationMarker.position = new Vector3(_destinationPos.x + 0.5f, _destinationPos.y + 0.5f);
        _destinationMarker.gameObject.SetActive(true);
    }

    IEnumerator Path()
    {
        Path p = _selectedHero.GetComponent<Seeker>().StartPath(_selectedHero.transform.position, _reachablePoint);
        yield return StartCoroutine(p.WaitForPath());
        if (p.error)
            yield break;
        if (_selectedHero == null)
            yield break;

        _selectedHero.UpdateRangeLeft(p.GetTotalLength());

        AILerp ai = _selectedHero.GetComponent<AILerp>();
        ai.canMove = true;
        ai.OnTargetReached += OnTargetReached;

        while (!ai.reachedEndOfPath)
        {
            _lineReachable.Start = _selectedHero.transform.position;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnTargetReached()
    {
        ClearMovementIndicators();
        if (_selectedHero == null)
            return;

        AILerp ai = _selectedHero.GetComponent<AILerp>();
        ai.OnTargetReached -= OnTargetReached;

        _selectedHero.UpdateMapPosition();
        _selectedHero.Unselect();
        _cameraSmoothFollow.SetTarget(null);
        _selectedHero = null;

        _gameManager.SaveJsonData();
    }

    void ClearMovementIndicators()
    {
        _lineReachable.Start = Vector3.zero;
        _lineReachable.End = Vector3.zero;
        _lineUnreachable.Start = Vector3.zero;
        _lineUnreachable.End = Vector3.zero;

        Vector3Int tilePos = _tilemap.WorldToCell(_reachablePoint);
        _tilemap.SetColor(tilePos, Color.white);

        _reachablePointMarker.gameObject.SetActive(false);
        _destinationMarker.gameObject.SetActive(false);
    }
}
