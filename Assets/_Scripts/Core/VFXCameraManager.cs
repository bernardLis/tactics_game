using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Core
{
    public class VFXCameraManager : Singleton<VFXCameraManager>
    {
        protected override void Awake()
        {
            base.Awake();

            Camera cam = GetComponent<Camera>();
            RenderTexture texture = cam.targetTexture;
            texture.Release();
            texture.width = Screen.width;
            texture.height = Screen.height;

            cam.targetTexture = texture;
        }
    }
}