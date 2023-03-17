using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Pathfinding;

public class MapMovementManager : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;
    CameraSmoothFollow _cameraSmoothFollow;

    [SerializeField] Tilemap _tilemap;
    [SerializeField] LineRenderer _lineRendererReachable;
    [SerializeField] LineRenderer _lineRendererUnreachable;

    MapHero _selectedHero;

    Vector3Int _destinationPos;
    List<Vector3> _lineRendererReachablePoints = new();
    List<Vector3> _lineRendererUnreachablePoints = new();
    Vector3 _reachablePoint;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _cameraSmoothFollow = Camera.main.GetComponent<CameraSmoothFollow>();
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
        if (_tilemap == null)
            return;
        _tilemap.SetColor(_destinationPos, Color.white);

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

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
        _tilemap.SetTileFlags(tilePos, TileFlags.None);
        _tilemap.SetColor(tilePos, Color.red);

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
        Vector3 middleOfTheTile = new Vector3(_destinationPos.x + 0.5f, _destinationPos.y + 0.5f);
        
        Path fullPath = Pathfinding.ABPath.Construct(_selectedHero.transform.position, middleOfTheTile);
        AstarPath.StartPath(fullPath);
        yield return StartCoroutine(fullPath.WaitForPath());

        _lineRendererReachablePoints = new();
        _lineRendererUnreachablePoints = new();

        for (int i = 0; i < fullPath.vectorPath.Count; i++) // 1 to start in front of character
        {
            Vector3 pos = new Vector3(fullPath.vectorPath[i].x, fullPath.vectorPath[i].y, -1); // -1 why shows the line, why?!
            if (_selectedHero == null)
                yield break;
            Path lengthCheckPath = Pathfinding.ABPath.Construct(_selectedHero.transform.position, fullPath.vectorPath[i]);

            AstarPath.StartPath(lengthCheckPath);
            yield return StartCoroutine(lengthCheckPath.WaitForPath());
            if (lengthCheckPath.error)
                yield break;

            if (lengthCheckPath.GetTotalLength() <= _selectedHero.RangeLeft)
            {
                _lineRendererReachablePoints.Add(pos);
                _reachablePoint = pos;
            }
            else
                _lineRendererUnreachablePoints.Add(pos);
        }

        _lineRendererReachable.positionCount = _lineRendererReachablePoints.Count;
        _lineRendererReachable.SetPositions(_lineRendererReachablePoints.ToArray());

        _lineRendererUnreachable.positionCount = _lineRendererUnreachablePoints.Count;
        _lineRendererUnreachable.SetPositions(_lineRendererUnreachablePoints.ToArray());
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

        int count = _lineRendererReachable.positionCount;
        while (!ai.reachedEndOfPath)
        {
            List<Vector3> temp = new();
            ai.GetRemainingPath(temp, out bool asd);
            _lineRendererReachablePoints = new();
            foreach (Vector3 v in temp)
            {
                Vector3 pos = new Vector3(v.x, v.y, -1); // TODO: -1 why shows the line, why?!
                _lineRendererReachablePoints.Add(pos);
            }
            _lineRendererReachable.positionCount = _lineRendererReachablePoints.Count;
            _lineRendererReachable.SetPositions(_lineRendererReachablePoints.ToArray());
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
        _lineRendererReachable.positionCount = 0;
        _lineRendererUnreachable.positionCount = 0;
        _tilemap.SetColor(_destinationPos, Color.white);
    }
}
