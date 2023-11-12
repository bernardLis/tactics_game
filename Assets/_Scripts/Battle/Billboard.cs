using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera _cam;
    // Start is called before the first frame update
    void Start() { _cam = Camera.main; }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + _cam.transform.forward);
    }
}
