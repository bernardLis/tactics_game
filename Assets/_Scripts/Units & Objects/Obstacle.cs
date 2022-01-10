using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public void Initialise(TilemapObject _obj)
    {
        GetComponentInChildren<SpriteRenderer>().sprite = _obj.sprite;
        GetComponentInChildren<BoxCollider2D>().size = _obj.size;
    }
}
