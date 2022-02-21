using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Chest")]
public class JourneyChest : BaseScriptableObject
{
    public Sprite closedChest;
    public Sprite emptyChest;
    public Sprite fullChest;

    [Range(0, 1)]
    public float chanceToBeEmpty;

    public GameObject gameObject;
    SpriteRenderer sr;

    public void Initialize(GameObject _obj)
    {
        gameObject = _obj;
        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = closedChest;
    }

    public void Won()
    {
        sr.sprite = fullChest;
    }

    public void Lost()
    {
        sr.sprite = emptyChest;
    }

}
