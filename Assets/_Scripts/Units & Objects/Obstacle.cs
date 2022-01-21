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

    }
}
