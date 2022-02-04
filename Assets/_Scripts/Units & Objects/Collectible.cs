using UnityEngine;

public class Collectible : MonoBehaviour
{
    bool collected;
    SpriteRenderer sr;
    BoxCollider2D boxCollider2D;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        boxCollider2D = GetComponentInChildren<BoxCollider2D>();
    }

    void OnTriggerExit2D(Collider2D _other)
    {
        if (!collected && _other.CompareTag("PushableObstacle"))
            Collect();
    }

    void Collect()
    {
        Debug.Log("Collected!");
        collected = true;
        sr.color = Color.red;
        boxCollider2D.enabled = false;
    }
}
