using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite openedChest;
    SpriteRenderer sr;
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.CompareTag("PlayerCollider") && TurnManager.battleState == BattleState.PlayerTurn)
        {
            Interact();
            Debug.Log("do you want to open the chest?");
        }
    }

    void Interact()
    {
        sr.sprite = openedChest;
        // TODO: maybe disable the light?
    }
}
