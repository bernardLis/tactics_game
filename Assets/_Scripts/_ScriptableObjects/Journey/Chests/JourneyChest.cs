using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Journey/Chest")]
public class JourneyChest : BaseScriptableObject
{
    public Sprite ClosedChest;
    public Sprite EmptyChest;
    public Sprite FullChest;

    [HideInInspector] public GameObject GameObject;
    SpriteRenderer _spriteRenderer;


    public void Initialize(GameObject obj)
    {
        GameObject = obj;
        _spriteRenderer = GameObject.GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = ClosedChest;
    }

    public void Won()
    {
        _spriteRenderer.sprite = FullChest;
    }

    public void Lost()
    {
        _spriteRenderer.sprite = EmptyChest;

    }
}
