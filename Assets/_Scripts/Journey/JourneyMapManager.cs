using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.InputSystem;
using DG.Tweening;

public class JourneyMapManager : Singleton<JourneyMapManager>
{

    GameManager _gameManager;
    RunManager _runManager;
    JourneyMapUI _journeyMapUI;
    PlayerInput _playerInput;

    [SerializeField] int _numberOfPaths = 5;
    [SerializeField] int _numberOfRows = 7;
    [SerializeField] int _numberOfBridges = 5;

    int _seed;

    [Header("Config")]
    [SerializeField] JourneyPathConfig[] _basicConfigs;


    [Header("Unity Setup")]
    [SerializeField] Sprite[] _backgrounds;
    [SerializeField] JourneyNode[] _journeyNodes;
    [SerializeField] JourneyNode _startNodeScriptableObject;
    [SerializeField] JourneyNode _endNodeScriptableObject;
    [SerializeField] GameObject _journeyHolder;
    [SerializeField] GameObject _journeyNodePrefab;
    [SerializeField] Material _journeyLine;
    [SerializeField] Material _pathTravelledLine;
    [SerializeField] Sound _journeyTheme;


    GameObject _startNode; // TODO: that's a bit confusing with scriptable object and node itself... 
    GameObject _endNode;

    List<JourneyPath> _journeyPaths;
    public JourneyNode CurrentNode { get; private set; }
    LineRenderer _pathTravelledLineRenderer;

    [HideInInspector] public List<JourneyNode> AvailableNodes = new();

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _runManager = RunManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();

        _journeyMapUI = GetComponent<JourneyMapUI>();
        GenerateJourney();

        AudioManager.Instance.PlayMusic(_journeyTheme);
    }

    /* JOURNEY GENERATION */
    public void GenerateJourney()
    {
        InitialSetup();
        CreatePaths();
        DisplayNodes();
        DrawConnections();
        AddJourneyBridges();
        SetBackground();
        SetUpTraversal();
        LoadData();
    }

    void InitialSetup()
    {
        _seed = _runManager.JourneySeed;
        Random.InitState(_seed);

        var tempList = _journeyHolder.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList)
            Destroy(child.gameObject);
    }

    void CreatePaths()
    {
        // use paths stored in object that is not destroyed between scenes, if there are any (cat comment)
        if (_runManager.WasJourneySetUp)
        {
            _journeyPaths = _runManager.JourneyPaths;
            return;
        }

        _journeyPaths = new();
        for (int i = 0; i < _numberOfPaths; i++)
        {
            JourneyPath jp = ScriptableObject.CreateInstance<JourneyPath>();
            jp.CreatePath(_numberOfRows, _journeyNodes, _basicConfigs);
            _journeyPaths.Add(jp);
            _runManager.JourneyPaths.Add(jp);
        }
    }
    void DisplayNodes()
    {
        // start node
        float centerX = _numberOfPaths * 30 * 0.5f; // TODO: magic number in between what I add to x for each path
        JourneyNode startNodeInstance = Instantiate(_startNodeScriptableObject);
        InstantiateNode(startNodeInstance, new Vector3(centerX, 0f));
        _startNode = startNodeInstance.GameObject;

        if (_runManager.WasJourneySetUp)
            _startNode.GetComponent<JourneyNodeBehaviour>().MarkAsVisited();

        int x = 0;
        int y = Random.Range(30, 60);
        int maxY = 0;
        foreach (JourneyPath p in _journeyPaths)
        {
            y = Random.Range(30, 60);
            foreach (JourneyNode n in p.Nodes)
            {
                // x + Random.Range to make it less generic
                InstantiateNode(n, new Vector3(x + Random.Range(0, 5), y, 0f));
                y += Random.Range(30, 60);
                if (y > maxY)
                    maxY = y;
            }
            x += Random.Range(20, 40);
        }

        // end node 
        JourneyNode endNodeInstance = Instantiate(_endNodeScriptableObject);
        InstantiateNode(endNodeInstance, new Vector3(centerX, maxY + Random.Range(30, 60)));
        _endNode = endNodeInstance.GameObject;
    }

    void DrawConnections()
    {
        // start gets line renderer per path and renders a line
        foreach (JourneyPath p in _journeyPaths)
        {
            GameObject g = new GameObject("LineRenderer");
            g.transform.parent = _startNode.gameObject.transform;
            LineRenderer lr = g.AddComponent<LineRenderer>();
            lr.material = _journeyLine;
            lr.textureMode = LineTextureMode.Tile;
            lr.positionCount = p.Nodes.Count + 2; // start + end
            //lr.startWidth = 0.5f;
            lr.SetPosition(0, _startNode.gameObject.transform.position);

            for (int i = 0; i < p.Nodes.Count; i++)
                lr.SetPosition(i + 1, p.Nodes[i].GameObject.transform.position); // coz position 0 is start node

            lr.SetPosition(p.Nodes.Count + 1, _endNode.gameObject.transform.position);
        }
    }

    void AddJourneyBridges()
    {
        // we are coming back to journey - bridges should stay the same
        if (_runManager.WasJourneySetUp)
        {
            RecreateBridges();
            return;
        }

        if (_numberOfPaths == 1)
            return;

        for (int i = 0; i < _numberOfBridges; i++)
        {
            int fromPathIndex = Random.Range(0, _journeyPaths.Count);
            JourneyPath fromPath = _journeyPaths[fromPathIndex];
            int fromNodeIndex = Random.Range(0, fromPath.Nodes.Count - 1); // -1 should not be the last one
            JourneyNode fromNode = fromPath.Nodes[fromNodeIndex];
            // TODO: dunno if correct for choosing path next to it
            JourneyPath toPath;
            if (fromPathIndex - 1 >= 0)
                toPath = _journeyPaths[fromPathIndex - 1];
            else
                toPath = _journeyPaths[fromPathIndex + 1];

            // TODO: is there a better way? 
            if (toPath.Nodes.Count <= fromNodeIndex + 1)
                continue;
            JourneyNode toNode = toPath.Nodes[fromNodeIndex + 1];

            JourneyBridge bridge = new JourneyBridge();
            bridge.Initialize(fromNode, toNode, _journeyLine);
            fromPath.Bridges.Add(bridge);
        }
    }

    void RecreateBridges()
    {
        foreach (JourneyPath p in _runManager.JourneyPaths)
        {
            foreach (JourneyBridge b in p.Bridges)
            {
                JourneyBridge bridge = new JourneyBridge();
                bridge.Initialize(b.From, b.To, _journeyLine);
            }
        }
    }

    void SetBackground()
    {
        GameObject g = new GameObject("Background");
        g.transform.parent = _journeyHolder.transform;
        g.layer = 2; // TODO: magic number, although it should always stay the same
        g.AddComponent<SpriteRenderer>().sprite = _backgrounds[Random.Range(0, _backgrounds.Length)];
        float x = GetMostRightNode().GameObject.transform.position.x * 0.5f;
        g.transform.position = new Vector3(x, _endNode.transform.position.y * 0.5f, 1f); // TODO: magic numbers
        g.transform.localScale = new Vector3(_numberOfPaths + 2f, _endNode.transform.position.y * 0.05f); // TODO: magic numbers
    }

    void SetUpTraversal()
    {
        // line renderer
        GameObject g = new GameObject("PathTravelledLineRenderer");
        g.transform.parent = _startNode.gameObject.transform;
        _pathTravelledLineRenderer = g.AddComponent<LineRenderer>();
        _pathTravelledLineRenderer.material = _pathTravelledLine;
        _pathTravelledLineRenderer.startWidth = 1f;
        _pathTravelledLineRenderer.material.color = Color.red;
        _pathTravelledLineRenderer.positionCount = 1;
        _pathTravelledLineRenderer.SetPosition(0, _startNode.gameObject.transform.position);
    }

    void LoadData()
    {
        // TODO: this can be improved
        if (_runManager.VisitedJourneyNodes.Count == 0)
        {
            CurrentNode = _startNode.GetComponent<JourneyNodeBehaviour>().JourneyNode;
            CurrentNode.Select();

            // available nodes
            AvailableNodes = new();
            foreach (JourneyPath p in _journeyPaths)
                AvailableNodes.Add(p.Nodes[0]);
            AnimateAvailableNodes();

            return;
        }

        _startNode.GetComponent<JourneyNodeBehaviour>().MarkAsVisited();

        JourneyNodeData data = _runManager.CurrentJourneyNode;
        CurrentNode = _journeyPaths[data.PathIndex].Nodes[data.NodeIndex];

        foreach (JourneyNodeData n in _runManager.VisitedJourneyNodes)
        {
            JourneyNode node = _journeyPaths[n.PathIndex].Nodes[n.NodeIndex];
            node.JourneyNodeBehaviour.MarkAsVisited();

            // render path
            _pathTravelledLineRenderer.positionCount++;
            _pathTravelledLineRenderer.SetPosition(_pathTravelledLineRenderer.positionCount - 1, node.GameObject.transform.position);
        }

        ResolveBackToJourney();
    }

    public void ResolveBackToJourney()
    {
        // and when I come back, I would like to run this:
        Invoke("ResolveRewards", 1f); // TODO: better way - waiting for the level loader to transition
        UpdateAvailableNodes();
        AnimateAvailableNodes();
    }

    void ResolveRewards()
    {
        if (_runManager.Reward == null)
            return;

        ChangeGold(_runManager.Reward.gold);
        _runManager.SetNodeReward(null);
    }

    void UpdateAvailableNodes()
    {
        AvailableNodes.Clear();
        // first node is set up in SetUpTraversal
        // if we are on the last node of the path we can travel only to the end node
        if (IsLastNodeOnPath(CurrentNode))
        {
            AvailableNodes.Add(_endNode.GetComponent<JourneyNodeBehaviour>().JourneyNode);
            return;
        }

        JourneyPath p = GetCurrentPath(CurrentNode);
        if (p == null)
            return;

        AvailableNodes.Add(p.Nodes[p.Nodes.IndexOf(CurrentNode) + 1]);

        JourneyNode bridgeNode = p.CheckBridge(CurrentNode);
        if (bridgeNode != null)
            AvailableNodes.Add(bridgeNode);
    }

    public void NodeClick(JourneyNodeBehaviour _node)
    {
        if (!AvailableNodes.Contains(_node.JourneyNode))
        {
            _node.transform.DOShakePosition(1f, new Vector3(2f, 2f, 0f)); // Vector3 to not vibrate on Z - coz it can be 'hidden' 
            return;
        }

        SelectNode(_node);
    }

    void SelectNode(JourneyNodeBehaviour _node)
    {
        Camera.main.GetComponent<JourneyCameraController>().UnsubscribeInputActions();

        _node.DrawCircle();
        _node.JourneyNode.Select(); // after n.journeyNodeBehaviour.StopAnimating(); to keep the color
        CurrentNode = _node.JourneyNode;

        JourneyNodeData data = new JourneyNodeData();
        JourneyPath currentPath = GetCurrentPath(_node.JourneyNode);
        data.PathIndex = _journeyPaths.IndexOf(currentPath);
        data.NodeIndex = currentPath.Nodes.IndexOf(CurrentNode);
        _runManager.SetCurrentJourneyNode(data);

        // render path
        _pathTravelledLineRenderer.positionCount++;
        _pathTravelledLineRenderer.SetPosition(_pathTravelledLineRenderer.positionCount - 1, _node.gameObject.transform.position);

        // So, here I would like to transition to a scene depending on the node
        // I also need to make sure this journey and all data is remembered between scene transitions 
        _runManager.LoadLevelFromNode(CurrentNode);
    }

    /* Helpers */
    void InstantiateNode(JourneyNode _n, Vector3 _pos)
    {
        GameObject g = Instantiate(_journeyNodePrefab, _pos, Quaternion.identity);
        g.transform.parent = _journeyHolder.transform;
        _n.Initialize(g);
        g.GetComponent<JourneyNodeBehaviour>().Initialize(_n); // Game Object knows it's node ... TODO: maybe unnecessary
    }

    // TODO: this is a very lazy approximation... 
    JourneyNode GetMostRightNode()
    {
        return _journeyPaths[_journeyPaths.Count - 1].Nodes[0];
    }

    bool IsLastNodeOnPath(JourneyNode _node)
    {
        foreach (JourneyPath p in _journeyPaths)
            if (p.Nodes[p.Nodes.Count - 1] == _node)
                return true;

        return false;
    }

    JourneyPath GetCurrentPath(JourneyNode _node)
    {
        foreach (JourneyPath p in _journeyPaths)
            if (p.Nodes.Contains(CurrentNode))
                return p;
        return null;
    }

    void AnimateAvailableNodes()
    {
        foreach (JourneyNode n in AvailableNodes)
            n.JourneyNodeBehaviour.AnimateAvailableNode();
    }

    public void ChangeGold(int _amount)
    {
        int gold = _runManager.Gold;
        Mathf.Clamp(gold, 0, Mathf.Infinity);
        _runManager.ChangeGoldValue(_amount);
        _journeyMapUI.ChangeGold(gold - _amount, _amount);
    }
}
