using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("do you want to open the chest?");
    }

    void Interact()
    {
        sr.sprite = openedChest;
        // TODO: maybe disable the light?
    }
}
