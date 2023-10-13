using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLandBorder : MonoBehaviour
{
    [SerializeField] ParticleSystem _auraParticleSystem;
    [SerializeField] Color defaultColor;
    public void EnableBorder(Color color)
    {
        gameObject.SetActive(true);
        if (color == default) color = defaultColor;

        ParticleSystem.MainModule main = _auraParticleSystem.main;
        main.startColor = color;
    }
}
