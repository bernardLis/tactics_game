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
    SpriteRenderer sr;

    public void Initialize(GameObject _self)
    {
        gameObject = _self;
        gameObject.name = name;
        gameObject.transform.localScale = new Vector3(3f, 3f);

        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = icon;
    }
    
    // TODO: do I want to do it from scriptable Object?
    public void Select()
    {
        sr.color = Color.magenta;
    }

}
