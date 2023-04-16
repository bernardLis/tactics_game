using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeEntityEffect : MonoBehaviour
{
    [SerializeField] GameObject _box; // set not active
    [SerializeField] GameObject _crystalAnimation; // set active

    // TODO: solve audio too...
    void OnEnable()
    {
        _box.SetActive(true);
        _crystalAnimation.SetActive(false);
    }

    public void SetDelays(float delay)
    {
        Invoke("Run", delay);
    }

    void Run()
    {
        _box.SetActive(false);
        _crystalAnimation.SetActive(true);
    }
}
