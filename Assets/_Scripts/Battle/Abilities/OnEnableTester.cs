using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableTester : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log($"{gameObject.name} OnEnable  {Time.time}");
    }

    void OnDisable()
    {
        Debug.Log($"{gameObject.name} OnDisable {Time.time}");
    }
}
