using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JourneyNodeType { Start, End, Battle, Knowledge, Chest, Blacksmith, Fire, Boss }
[CreateAssetMenu(menuName = "ScriptableObject/Journey/Node")]
public class JourneyNode : BaseScriptableObject
{
    public Sprite icon;
    public JourneyNodeType nodeType;
    public GameObject gameObject;

    public void Initialize(GameObject _self)
    {
        gameObject = _self;
    }

    public void ChooseNode()
    {
        Debug.Log("this node was chosen: " + nodeType);
    }

}
