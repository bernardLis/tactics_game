using UnityEngine;


public enum JourneyNodeType { Start, End, Battle, Knowledge, Chest, Shop, Fire, Boss, Event }
[CreateAssetMenu(menuName = "ScriptableObject/Journey/Node")]
public class JourneyNode : BaseScriptableObject
{
    public Sprite Icon;
    public JourneyNodeType NodeType;
    public string SceneToLoad;

    [HideInInspector] public GameObject GameObject;
    SpriteRenderer _spriteRenderer;
    [HideInInspector] public JourneyNodeBehaviour JourneyNodeBehaviour;

    public virtual void Initialize(GameObject self)
    {
        GameObject = self;
        GameObject.name = name;
        GameObject.transform.localScale = new Vector3(3f, 3f);

        _spriteRenderer = GameObject.GetComponentInChildren<SpriteRenderer>();
        JourneyNodeBehaviour = GameObject.GetComponent<JourneyNodeBehaviour>();
        _spriteRenderer.sprite = Icon;
    }

    // TODO: do I want to do it from scriptable Object?
    public void Select()
    {
        _spriteRenderer.color = Color.black;
    }
}

[System.Serializable]
public struct JourneyNodeData
{
    public int PathIndex;
    public int NodeIndex;
}


