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
    public GameObject journeyHolder;
    public GameObject journeyNodePrefab;

    List<JourneyRow> journeyRows;

    public void GenerateJourney()
    {
        Debug.Log("GenerateJourney");
        InitialSetup();
        CreateRows();
        CreateNodes();
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
        for (int i = 0; i < numberOfRows; i++)
        {
            JourneyRow r = new JourneyRow();
            r.CreateRow(numberOfNodes, journeyNodes);
            r.RemoveRandomNodes();
            journeyRows.Add(r);
        }
    }

    void CreateNodes()
    {
        int y = 0;
        foreach (JourneyRow r in journeyRows)
        {
            int x = 0;
            foreach (JourneyNode n in r.journeyNodes)
            {
                GameObject g = Instantiate(journeyNodePrefab, new Vector3(x, y, 0f), Quaternion.identity);
                g.GetComponentInChildren<SpriteRenderer>().sprite = n.icon;
                g.transform.parent = journeyHolder.transform;
                x += Random.Range(10, 20);
            }
            y += Random.Range(10, 20);
        }

    }

}
