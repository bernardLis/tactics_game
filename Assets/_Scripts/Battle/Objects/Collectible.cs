using UnityEngine;
using UnityEngine.UIElements;

public class Collectible : MonoBehaviour
{
    public bool IsCollected { get; private set; }
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;

    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _boxCollider2D = GetComponentInChildren<BoxCollider2D>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsCollected && other.CompareTag(Tags.PushableObstacle))
            Collect();
    }

    void Collect()
    {
        IsCollected = true;
        _spriteRenderer.color = Color.red;
        _boxCollider2D.enabled = false;

        BattleUI.Instance.DisplayBattleLog(new Label("Collectible fox found!"));
    }
}
