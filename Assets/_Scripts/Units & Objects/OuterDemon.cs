using UnityEngine;

public class OuterDemon : MonoBehaviour
{
    SpriteRenderer sr;
    void Start()
    {
    }
    public void Initialize(TilemapObject _so)
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = _so.sprite;
    }
}
