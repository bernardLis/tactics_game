using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class JourneyMapManager : MonoBehaviour
{
    public int numberOfRows = 7;
    public int numberOfNodes = 5;

    int seed;

    [Header("Unity Setup")]
    public JourneyNode[] journeyNodes;
    public JourneyNode startNode;
    public JourneyNode endNode;

    public GameObject journeyHolder;
    public GameObject journeyNodePrefab;

    List<JourneyRow> journeyRows;

    public void GenerateJourney()
    {
        Debug.Log("GenerateJourney");
        InitialSetup();
        CreateRows();
        CreateNodes();
        CreateConnections();
        RemoveSomeConnections();
    }

    void InitialSetup()
    {
        seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);

        var tempList = journeyHolder.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList)
            DestroyImmediate(child.gameObject); // TODO: destory
    }

    void CreateRows()
    {
        journeyRows = new();

        JourneyRow startRow = ScriptableObject.CreateInstance<JourneyRow>();
        startRow.AddNode(startNode);
        journeyRows.Add(startRow);

        for (int i = 0; i < numberOfRows; i++)
        {
            JourneyRow r = ScriptableObject.CreateInstance<JourneyRow>();
            r.CreateRow(numberOfNodes, journeyNodes);
            r.RemoveRandomNodes();
            journeyRows.Add(r);
        }

        JourneyRow endRow = ScriptableObject.CreateInstance<JourneyRow>();
        endRow.AddNode(endNode);
        journeyRows.Add(endRow);
    }

    void CreateNodes()
    {
        int y = 0;
        foreach (JourneyRow r in journeyRows)
        {
            int x = (numberOfNodes - r.journeyNodes.Count) * 45; // TODO: magic number! 10/2 to center the rows nodes somewhat
            foreach (JourneyNode n in r.journeyNodes)
            {
                GameObject g = Instantiate(journeyNodePrefab, new Vector3(x, y, 0f), Quaternion.identity);
                n.Initialize(g);
                g.GetComponentInChildren<SpriteRenderer>().sprite = n.icon;
                g.transform.parent = journeyHolder.transform;
                x += Random.Range(30, 60);
            }
            y += Random.Range(20, 40);
        }
    }

    void CreateConnections()
    {
        for (int i = 0; i < journeyRows.Count - 1; i++) // -1 to skip the last one
            foreach (JourneyNode n in journeyRows[i].journeyNodes)
                n.Connect(journeyRows[i + 1]);
    }

    void RemoveSomeConnections()
    {
        // TODO: I need to make sure that there is at least one connection to the node from the previous row
        // maybe node needs to keep track who is connecting to it.
        foreach (JourneyRow r in journeyRows)
        {
            foreach (JourneyNode n in r.journeyNodes)
            {
                if (n.nodeType == JourneyNodeType.Start || n.nodeType == JourneyNodeType.End)
                    continue;
                n.RemoveSomeConnections();
            }

        }
    }

}
