using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JourneyNodeType { Start, End, Battle, Knowledge, Chest, Blacksmith, Fire, Boss }
[CreateAssetMenu(menuName = "ScriptableObject/Journey/Node")]
public class JourneyNode : BaseScriptableObject
{
    public Sprite icon;
    public JourneyNodeType nodeType;
    public int nodeObols;

    [HideInInspector] public GameObject gameObject;
    SpriteRenderer sr;
    [HideInInspector] public JourneyNodeBehaviour journeyNodeBehaviour;

    public void Initialize(GameObject _self)
    {
        gameObject = _self;
        gameObject.name = name;
        gameObject.transform.localScale = new Vector3(3f, 3f);

        nodeObols = Random.Range(0, 10);

        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        journeyNodeBehaviour = gameObject.GetComponent<JourneyNodeBehaviour>();
        sr.sprite = icon;
    }

    // TODO: do I want to do it from scriptable Object?
    public void Select()
    {
        sr.color = Color.black;
    }

}
