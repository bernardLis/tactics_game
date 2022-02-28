using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Chest")]
public class JourneyChest : BaseScriptableObject
{
    [SerializeField] Sprite _closedChest;
    [SerializeField] Sprite _emptyChest;
    [SerializeField] Sprite _fullChest;

    [SerializeField] GameObject _gameObject;
    SpriteRenderer _spriteRenderer;

    public void Initialize(GameObject obj)
    {
        _gameObject = obj;
        _spriteRenderer = _gameObject.GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = _closedChest;
    }

    public void Won()
    {
        _spriteRenderer.sprite = _fullChest;
    }

    public void Lost()
    {
        _spriteRenderer.sprite = _emptyChest;
    }

}
