using UnityEngine;

public class Obstacle : MonoBehaviour
{
    BoxCollider2D col;
    void Start()
    {
        col = GetComponentInChildren<BoxCollider2D>();

    }
    public void Initialise(TilemapObject _obj)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = _obj.sprite;

        if (col != null)
            col.size = _obj.size;

        if (Random.Range(0f, 1f) > 0.5f)
            sr.flipX = true;
    }
}
