using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBattle : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"collision.gameObject.tag: {collision.gameObject.tag}");
    }
}
