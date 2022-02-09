using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class JourneyMapManager : MonoBehaviour
{
    public int numberOfRows = 7;
    public int numberOfPaths = 5;

    int seed;

    [Header("Unity Setup")]
    public JourneyNode[] journeyNodes;
    public JourneyNode startNodeScriptableObject;
    public JourneyNode endNodeScriptableObject;

    public GameObject journeyHolder;
    public GameObject journeyNodePrefab;


    GameObject startNode; // TODO: that's a bit confusing with scriptable object and node itself... 
    GameObject endNode;

    List<JourneyPath> journeyPaths;


    public void GenerateJourney()
    {
        Debug.Log("GenerateJourney");
        InitialSetup();
        CreatePaths();
        DisplayNodes();
        DrawConnections();
        AddJourneyBridges();
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
            jp.CreatePath(numberOfRows, journeyNodes);
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
            GameObject g = new GameObject();
            g.transform.parent = startNode.gameObject.transform;
            LineRenderer lr = g.AddComponent<LineRenderer>();
            lr.positionCount = p.nodes.Count + 2; // start + end
            lr.startWidth = 0.2f;
            lr.SetPosition(0, startNode.gameObject.transform.position);

            for (int i = 0; i < p.nodes.Count; i++)
                lr.SetPosition(i + 1, p.nodes[i].gameObject.transform.position); // coz position 0 is start node

            lr.SetPosition(p.nodes.Count + 1, endNode.gameObject.transform.position);
        }
    }

    void AddJourneyBridges()
    {

        // the connection can only be to +-1 one path - it cannot cross through a path
        // who keeps track of bridges?
        // bridge needs to go upwards
        int numberOfBridges = 5;

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

            // ok, number of nodes in paths varies, so there is a chance that there won't as many nodes as I need
            // TODO: there may be a better way? 
            if (toPath.nodes.Count <= fromNodeIndex + 1)
                continue;
            JourneyNode toNode = toPath.nodes[fromNodeIndex + 1];

            JourneyBridge bridge = new JourneyBridge();
            bridge.Initialize(fromNode, toNode);
            fromPath.bridges.Add(bridge);
        }

        // choose a random path, 
        // choose a random node, 
        // choose path +- to it
        // choose a node with index of random node +1
        // create a bridge
        // add bridge to list of path of from node

    }


    /* Helpers */
    void InstantiateNode(JourneyNode _n, Vector3 _pos)
    {
        GameObject g = Instantiate(journeyNodePrefab, _pos, Quaternion.identity);
        g.transform.parent = journeyHolder.transform;
        _n.Initialize(g);
        g.GetComponent<JourneyNodeBehaviour>().Initialize(_n); // Game Object knows it's node ... TODO: maybe unnecessary
    }

}
