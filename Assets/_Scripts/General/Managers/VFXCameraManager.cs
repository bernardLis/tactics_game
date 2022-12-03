using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXCameraManager : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        RenderTexture texture = cam.targetTexture;
        texture.Release();
        texture.width = Screen.width;
        texture.height = Screen.height;

        cam.targetTexture = texture;
    }

}
