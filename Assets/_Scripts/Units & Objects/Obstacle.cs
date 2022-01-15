using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public void Initialise(TilemapObject _obj)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = _obj.sprite;
        GetComponentInChildren<BoxCollider2D>().size = _obj.size;

        if (Random.Range(0f, 1f) > 0.5f)
            sr.flipX = true;
    }
}
