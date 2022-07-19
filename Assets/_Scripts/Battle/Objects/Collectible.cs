using UnityEngine;

public class Collectible : MonoBehaviour
{
    bool _isCollected;
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;

    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _boxCollider2D = GetComponentInChildren<BoxCollider2D>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!_isCollected && other.CompareTag(Tags.PushableObstacle))
            Collect();
    }

    void Collect()
    {
        _isCollected = true;
        _spriteRenderer.color = Color.red;
        _boxCollider2D.enabled = false;

        BattleUI.Instance.DisplayBattleLog("Collectible fox found!");
    }
}
