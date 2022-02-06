using UnityEngine;

public class Chest : MonoBehaviour, IItemUsable<UtilityAbility>
{
    public Sprite openedChest;
    SpriteRenderer sr;
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public bool UseItem(UtilityAbility _ability)
    {
        Debug.Log("use item in chest");
        if (_ability.utilityType == UtilityType.Key)
        {
            Interact();
            return true;
        }
        return false;
    }

    void Interact()
    {
        sr.sprite = openedChest;
        // TODO: maybe disable the light?
    }
}
