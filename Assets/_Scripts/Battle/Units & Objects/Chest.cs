using UnityEngine;

public class Chest : MonoBehaviour, IItemUsable<UtilityAbility>
{
    [SerializeField] Sprite _openedChest;
    SpriteRenderer _spriteRenderer;
    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public bool UseItem(UtilityAbility ability)
    {
        if (ability.UtilityType == UtilityType.Key)
        {
            Interact();
            return true;
        }
        return false;
    }

    void Interact()
    {
        _spriteRenderer.sprite = _openedChest;
        // TODO: maybe disable the light?
    }
}
