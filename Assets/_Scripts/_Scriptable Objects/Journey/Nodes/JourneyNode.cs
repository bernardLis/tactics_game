using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JourneyNodeType { Start, End, Battle, Knowledge, Chest, Blacksmith, Fire, Boss }
[CreateAssetMenu(menuName = "ScriptableObject/Journey/Node")]
public class JourneyNode : BaseScriptableObject
{
    public Sprite icon;
    public JourneyNodeType nodeType;
    public GameObject self;

    public List<JourneyConnection> journeyConnectionsFrom = new();
    public List<JourneyConnection> journeyConnectionsTo = new();


    public void Initialize(GameObject _self)
    {
        self = _self;
        // TODO: this is convoluted due to naming.
        self.GetComponent<JourneyNodeScript>().Initialize(this);
    }

    public void Connect(JourneyRow _r)
    {
        foreach (JourneyNode n in _r.journeyNodes)
        {
            JourneyConnection jc = new JourneyConnection();
            jc.CreateConnection(this, n);
            journeyConnectionsFrom.Add(jc);
            n.journeyConnectionsTo.Add(jc); // node that is being connected too is keeping track of connections 
        }
    }

    public void RemoveSomeConnections()
    {
        for (int i = 0; i < journeyConnectionsTo.Count; i++)
            if (Random.Range(0, 2) == 0)
                journeyConnectionsTo[i].RemoveConnection();

        

    }

    public void ChooseNode()
    {
        Debug.Log("this node was chosen: " + nodeType);
    }

}
