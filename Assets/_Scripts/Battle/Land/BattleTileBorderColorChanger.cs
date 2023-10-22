using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileBorderColorChanger : MonoBehaviour
{
    public void ChangeColor(Color color)
    {
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startColor = color;
    }
}
