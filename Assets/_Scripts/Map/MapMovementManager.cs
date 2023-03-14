using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Pathfinding;

public class MapMovement : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    [SerializeField] Tilemap _tilemap;
    [SerializeField] GameObject _hero;
    [SerializeField] LineRenderer _lineRenderer;


    Vector3Int _destinationPos;
    GameObject _destination;
    Path _currentPath;
    List<Vector3> _lineRendererPoints = new();

    // Start is called before the first frame update
    void Start() { _gameManager = GameManager.Instance; }

    // Update is called once per frame
    void Update()
    {

    }

    /* INPUT */
    void SubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].performed += LeftMouseClick;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].performed -= LeftMouseClick;
    }

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

    void LeftMouseClick(InputAction.CallbackContext ctx)
    {
        if (_destinationPos != null)
            _tilemap.SetColor(_destinationPos, Color.white);

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
        _hero.GetComponent<AILerp>().canMove = false;
        Vector3 middleOfTheTile = new Vector3(_destinationPos.x + 0.5f, _destinationPos.y + 0.5f);
        Path p = _hero.GetComponent<Seeker>().StartPath(_hero.transform.position, middleOfTheTile);
        yield return StartCoroutine(p.WaitForPath());
        _currentPath = p;

        _lineRendererPoints = new();
        for (int i = 0; i < p.vectorPath.Count; i++) // 1 to start in front of character
        {
            Vector3 pos = new Vector3(p.vectorPath[i].x, p.vectorPath[i].y, -1); // -1 why shows the line, why?!
            _lineRendererPoints.Add(pos);
        }

        _lineRenderer.positionCount = _lineRendererPoints.Count;
        _lineRenderer.SetPositions(_lineRendererPoints.ToArray());
    }

    IEnumerator Path()
    {
        AILerp ai = _hero.GetComponent<AILerp>();
        ai.canMove = true;
        ai.OnTargetReached += OnTargetReached;

        int count = _lineRenderer.positionCount;
        while (!ai.reachedEndOfPath)
        {
            List<Vector3> temp = new();
            ai.GetRemainingPath(temp, out bool asd);
           // temp.RemoveAt(0);// 1 to start in front of character
            _lineRendererPoints = new();
            foreach (Vector3 v in temp)
            {
                Vector3 pos = new Vector3(v.x, v.y, -1); // -1 why shows the line, why?!
                _lineRendererPoints.Add(pos);
            }
            _lineRenderer.positionCount = _lineRendererPoints.Count;
            _lineRenderer.SetPositions(_lineRendererPoints.ToArray());
            yield return new WaitForSeconds(0.1f);
        }
        /*
        for (int i = 0; i < count; i++)
        {
            _lineRendererPoints.RemoveAt(0);
        }
        */
    }

    void OnTargetReached()
    {
        Debug.Log($"target reached");
        _lineRenderer.positionCount = 0;
    }
}
