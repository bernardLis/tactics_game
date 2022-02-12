using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.InputSystem;
using DG.Tweening;
public class JourneyMapManager : MonoBehaviour
{
    public int numberOfPaths = 5;
    public int numberOfRows = 7;
    public int numberOfBridges = 5;

    int seed;

    [Header("Config")]
    public JourneyPathConfig[] basicConfigs;


    [Header("Unity Setup")]
    public Sprite[] backgrounds;
    public JourneyNode[] journeyNodes;
    public JourneyNode startNodeScriptableObject;
    public JourneyNode endNodeScriptableObject;
    public GameObject journeyHolder;
    public GameObject journeyNodePrefab;
    public GameObject onClickParticlePrefab;

    PlayerInput playerInput;

    GameObject startNode; // TODO: that's a bit confusing with scriptable object and node itself... 
    GameObject endNode;

    List<JourneyPath> journeyPaths;
    public JourneyNode currentNode;
    LineRenderer pathTravelledLineRenderer;

    public List<JourneyNode> availableNodes = new();


    public static JourneyMapManager instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of TurnManager found");
            return;
        }
        instance = this;
        #endregion
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        GenerateJourney();
    }

    /* INPUT */
    void OnEnable()
    {
        // inputs
        playerInput = GetComponent<PlayerInput>();

        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (playerInput == null)
            return;

        UnsubscribeInputActions();
    }


    void SubscribeInputActions()
    {
    }

    void UnsubscribeInputActions()
    {
    }

    public void NodeClick(JourneyNodeBehaviour _node)
    {
        if (!availableNodes.Contains(_node.journeyNode))
        {
            _node.transform.DOShakePosition(1f, new Vector3(2f, 2f, 0f)); // Vector3 to not vibrate on Z - coz it can be 'hidden' 
            return;
        }

        currentNode = _node.journeyNode;
        _node.journeyNode.Select();
        UpdateAvailableNodes();
        AnimateAvailableNodes();

        // render path
        pathTravelledLineRenderer.positionCount++;
        pathTravelledLineRenderer.SetPosition(pathTravelledLineRenderer.positionCount - 1, _node.gameObject.transform.position);
    }

    void UpdateAvailableNodes()
    {
        foreach (JourneyNode n in availableNodes)
        {
            n.gameObject.transform.DOKill();
            n.gameObject.transform.localScale = new Vector3(3f, 3f, 1f); //TODO: magic number
        }

        availableNodes.Clear();
        // first node is set up in SetUpTraversal
        // if we are on the last node of the path we can travel only to the end node
        if (IsLastNodeOnPath(currentNode))
        {
            availableNodes.Add(endNode.GetComponent<JourneyNodeBehaviour>().journeyNode);
            return;
        }

        JourneyPath p = GetCurrentPath(currentNode);
        if (p == null)
            return;

        availableNodes.Add(p.nodes[p.nodes.IndexOf(currentNode) + 1]);

        JourneyNode bridgeNode = p.CheckBridge(currentNode);
        if (bridgeNode != null)
            availableNodes.Add(bridgeNode);
    }

    void AnimateAvailableNodes()
    {
        foreach (JourneyNode n in availableNodes)
            n.gameObject.transform.DOScale(n.gameObject.transform.localScale * 1.7f, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    bool IsLastNodeOnPath(JourneyNode _node)
    {
        foreach (JourneyPath p in journeyPaths)
            if (p.nodes[p.nodes.Count - 1] == _node)
                return true;

        return false;
    }

    JourneyPath GetCurrentPath(JourneyNode _node)
    {
        foreach (JourneyPath p in journeyPaths)
            if (p.nodes.Contains(currentNode))
                return p;
        return null;
    }

    /* JOURNEY GENERATION */
    public void GenerateJourney()
    {
        Debug.Log("GenerateJourney");
        InitialSetup();
        CreatePaths();
        DisplayNodes();
        DrawConnections();
        AddJourneyBridges();
        SetBackground();
        SetUpTraversal();
    }

    void InitialSetup()
    {
        seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);

        var tempList = journeyHolder.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList)
            DestroyImmediate(child.gameObject); // TODO: destory
    }

    void CreatePaths()
    {
        journeyPaths = new();

        for (int i = 0; i < numberOfPaths; i++)
        {
            JourneyPath jp = ScriptableObject.CreateInstance<JourneyPath>();
            jp.CreatePath(numberOfRows, journeyNodes, basicConfigs);
            journeyPaths.Add(jp);
        }
    }
    void DisplayNodes()
    {
        // start node
        float centerX = numberOfPaths * 30 * 0.5f; // TODO: magic number in between what I add to x for each path
        JourneyNode startNodeInstance = Instantiate(startNodeScriptableObject);
        InstantiateNode(startNodeInstance, new Vector3(centerX, 0f));
        startNode = startNodeInstance.gameObject;
        currentNode = startNodeInstance;
        currentNode.Select();

        int x = 0;
        int y = Random.Range(30, 60);
        int maxY = 0;
        foreach (JourneyPath p in journeyPaths)
        {
            y = Random.Range(30, 60);
            foreach (JourneyNode n in p.nodes)
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
        JourneyNode endNodeInstance = Instantiate(endNodeScriptableObject);
        InstantiateNode(endNodeInstance, new Vector3(centerX, maxY + Random.Range(30, 60)));
        endNode = endNodeInstance.gameObject;
    }

    void DrawConnections()
    {

        // start gets line renderer per path and renders a line
        foreach (JourneyPath p in journeyPaths)
        {
            GameObject g = new GameObject("LineRenderer");
            g.transform.parent = startNode.gameObject.transform;
            LineRenderer lr = g.AddComponent<LineRenderer>();
            lr.positionCount = p.nodes.Count + 2; // start + end
            lr.startWidth = 0.5f;
            lr.SetPosition(0, startNode.gameObject.transform.position);

            for (int i = 0; i < p.nodes.Count; i++)
                lr.SetPosition(i + 1, p.nodes[i].gameObject.transform.position); // coz position 0 is start node

            lr.SetPosition(p.nodes.Count + 1, endNode.gameObject.transform.position);
        }
    }

    void AddJourneyBridges()
    {
        if (numberOfPaths == 1)
            return;

        for (int i = 0; i < numberOfBridges; i++)
        {
            int fromPathIndex = Random.Range(0, journeyPaths.Count);
            JourneyPath fromPath = journeyPaths[fromPathIndex];
            int fromNodeIndex = Random.Range(0, fromPath.nodes.Count - 1); // -1 should not be the last one
            JourneyNode fromNode = fromPath.nodes[fromNodeIndex];
            // TODO: dunno if correct for choosing path next to it
            JourneyPath toPath;
            if (fromPathIndex - 1 >= 0)
                toPath = journeyPaths[fromPathIndex - 1];
            else
                toPath = journeyPaths[fromPathIndex + 1];

            // TODO: is there a better way? 
            if (toPath.nodes.Count <= fromNodeIndex + 1)
                continue;
            JourneyNode toNode = toPath.nodes[fromNodeIndex + 1];

            JourneyBridge bridge = new JourneyBridge();
            bridge.Initialize(fromNode, toNode);
            fromPath.bridges.Add(bridge);
        }
    }

    void SetBackground()
    {
        GameObject g = new GameObject("Background");
        g.transform.parent = journeyHolder.transform;
        g.layer = 2; // TODO: magic number, although it should always stay the same
        g.AddComponent<SpriteRenderer>().sprite = backgrounds[Random.Range(0, backgrounds.Length)];
        float x = GetMostRightNode().gameObject.transform.position.x * 0.5f;
        g.transform.position = new Vector3(x, endNode.transform.position.y * 0.5f, 1f); // TODO: magic numbers
        g.transform.localScale = new Vector3(numberOfPaths + 2f, endNode.transform.position.y * 0.05f); // TODO: magic numbers
    }

    void SetUpTraversal()
    {
        // line renderer
        GameObject g = new GameObject("PathTravelledLineRenderer");
        g.transform.parent = startNode.gameObject.transform;
        pathTravelledLineRenderer = g.AddComponent<LineRenderer>();
        pathTravelledLineRenderer.startWidth = 1f;
        pathTravelledLineRenderer.material.color = Color.red;
        pathTravelledLineRenderer.positionCount = 1;
        pathTravelledLineRenderer.SetPosition(0, startNode.gameObject.transform.position);

        // available nodes
        availableNodes = new();
        foreach (JourneyPath p in journeyPaths)
            availableNodes.Add(p.nodes[0]);
        AnimateAvailableNodes();
    }

    /* Helpers */
    void InstantiateNode(JourneyNode _n, Vector3 _pos)
    {
        GameObject g = Instantiate(journeyNodePrefab, _pos, Quaternion.identity);
        g.transform.parent = journeyHolder.transform;
        _n.Initialize(g);
        g.GetComponent<JourneyNodeBehaviour>().Initialize(_n); // Game Object knows it's node ... TODO: maybe unnecessary
    }

    // TODO: this is a very lazy approximation... 
    JourneyNode GetMostRightNode()
    {
        return journeyPaths[journeyPaths.Count - 1].nodes[0];
    }
}
